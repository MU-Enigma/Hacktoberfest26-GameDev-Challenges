// Level 3 - Chicken Run: a three-stage platformer for the blue chicken.
// Uses the Level 2 mechanics: double jump (flap) and dash.
(() => {
  const TILE = 32, COLS = 30, ROWS = 17;
  const W = COLS * TILE, H = ROWS * TILE;
  const canvas = document.getElementById('game');
  const ctx = canvas.getContext('2d');
  ctx.imageSmoothingEnabled = false;

  // ---- Tuning (same feel as the Level 2 mechanics) --------------------------
  const PHYS = {
    accel: 2600, friction: 2400, maxSpeed: 290, gravity: 1900, maxFall: 1000,
    jump: 690, flap: 0.9, coyote: 0.09,
    dashSpeed: 860, dashTime: 0.16, dashCooldown: 0.5,
    spring: 1080,
  };
  const PW = 26, PH = 28; // player hitbox, a bit smaller than a tile

  // ---- Level parsing -------------------------------------------------------
  function parseLevel(def) {
    const grid = def.map.map(r => r.padEnd(COLS, '.').slice(0, COLS).split(''));
    while (grid.length < ROWS) grid.push(Array(COLS).fill('.'));
    let start = { x: TILE, y: TILE }, flag = null;
    const corn = [];
    grid.forEach((row, r) => row.forEach((ch, c) => {
      if (ch === 'P') { start = { x: c * TILE + (TILE - PW) / 2, y: (r + 1) * TILE - PH }; row[c] = '.'; }
      if (ch === 'F') { flag = { x: c * TILE + 8, y: r * TILE - TILE, w: 16, h: TILE * 2 }; row[c] = '.'; }
      if (ch === 'o') { corn.push({ x: c * TILE + 8, y: r * TILE + 6, w: 16, h: 20, taken: false }); row[c] = '.'; }
    }));
    return { name: def.name, tip: def.tip, grid, start, flag, corn };
  }

  // Outside the map: walls on the sides and an invisible ceiling on top (so you can't skip
  // a level by jumping over the screen), open at the bottom so you can fall into pits.
  const tileAt = (lvl, c, r) => (c < 0 || c >= COLS || r < 0) ? '#' : r >= ROWS ? '.' : lvl.grid[r][c];
  const isSolid = ch => ch === '#' || ch === 'S';

  // ---- Pure player physics ---------------------------------------------------
  // stepPlayer(lvl, s, inp, dt) updates state `s` in place and returns events.
  // inp = { dir: -1|0|1, jumpPressed, jumpHeld, dashPressed }
  // Keeping this free of drawing/input code lets us test levels automatically.
  function newPlayerState(lvl) {
    return { x: lvl.start.x, y: lvl.start.y, vx: 0, vy: 0, facing: 1, onGround: false, coyote: 0,
             airJumps: 1, dashT: 0, dashCd: 0, airDashUsed: false, dashDir: 1 };
  }

  function stepPlayer(lvl, s, inp, dt) {
    const ev = { flapped: false, dashed: false, landed: false, sprung: false, dead: false, won: false };
    s.dashCd = Math.max(0, s.dashCd - dt);

    // Start a dash.
    if (inp.dashPressed && s.dashCd === 0 && s.dashT === 0 && !(s.airDashUsed && !s.onGround)) {
      s.dashDir = inp.dir || s.facing;
      s.dashT = PHYS.dashTime; s.dashCd = PHYS.dashCooldown;
      if (!s.onGround) s.airDashUsed = true;
      ev.dashed = true;
    }

    if (s.dashT > 0) {
      s.dashT = Math.max(0, s.dashT - dt);
      s.vx = s.dashDir * PHYS.dashSpeed; s.vy = 0; s.facing = s.dashDir;
      if (s.dashT === 0) s.vx = s.dashDir * PHYS.maxSpeed;
    } else {
      if (inp.dir) { s.vx += inp.dir * PHYS.accel * dt; s.facing = inp.dir; }
      else { const f = PHYS.friction * dt; s.vx = Math.abs(s.vx) <= f ? 0 : s.vx - Math.sign(s.vx) * f; }
      s.vx = Math.max(-PHYS.maxSpeed, Math.min(PHYS.maxSpeed, s.vx));

      s.coyote = s.onGround ? PHYS.coyote : Math.max(0, s.coyote - dt);
      if (inp.jumpPressed) {
        if (s.onGround || s.coyote > 0) { s.vy = -PHYS.jump; s.onGround = false; s.coyote = 0; }
        else if (s.airJumps > 0) { s.airJumps--; s.vy = -PHYS.jump * PHYS.flap; ev.flapped = true; }
      }
      if (!inp.jumpHeld && s.vy < -PHYS.jump * 0.45) s.vy *= 0.85; // short hop when released early
      s.vy = Math.min(PHYS.maxFall, s.vy + PHYS.gravity * dt);
    }

    // Move and collide with the tile grid, one axis at a time.
    s.x += s.vx * dt;
    collide(lvl, s, 'x');
    s.y += s.vy * dt;
    const was = s.onGround;
    s.onGround = false;
    const sprung = collide(lvl, s, 'y');
    if (sprung) { s.vy = -PHYS.spring; s.onGround = false; s.airJumps = 1; s.airDashUsed = false; ev.sprung = true; }
    if (s.onGround) { s.airJumps = 1; s.airDashUsed = false; if (!was) ev.landed = true; }

    // Hazards and goal.
    if (s.y > H + 64 || touchesSpikes(lvl, s)) ev.dead = true;
    if (lvl.flag && overlap(s, lvl.flag)) ev.won = true;
    return ev;
  }

  function collide(lvl, s, axis) {
    let sprung = false;
    const c0 = Math.floor(s.x / TILE), c1 = Math.floor((s.x + PW - 0.001) / TILE);
    const r0 = Math.floor(s.y / TILE), r1 = Math.floor((s.y + PH - 0.001) / TILE);
    for (let r = r0; r <= r1; r++) for (let c = c0; c <= c1; c++) {
      const ch = tileAt(lvl, c, r);
      if (!isSolid(ch)) continue;
      if (axis === 'x') {
        if (s.vx > 0) s.x = c * TILE - PW; else if (s.vx < 0) s.x = (c + 1) * TILE;
        s.vx = 0;
      } else {
        if (s.vy > 0) { s.y = r * TILE - PH; s.onGround = true; if (ch === 'S') sprung = true; }
        else if (s.vy < 0) s.y = (r + 1) * TILE;
        s.vy = 0;
      }
      return sprung;
    }
    return sprung;
  }

  // Spikes only hurt in their pointy top part, so grazing the edge is forgiving.
  function touchesSpikes(lvl, s) {
    const c0 = Math.floor((s.x + 4) / TILE), c1 = Math.floor((s.x + PW - 4) / TILE);
    const r0 = Math.floor(s.y / TILE), r1 = Math.floor((s.y + PH - 1) / TILE);
    for (let r = r0; r <= r1; r++) for (let c = c0; c <= c1; c++) {
      if (tileAt(lvl, c, r) === '^' && s.y + PH > r * TILE + 12) return true;
    }
    return false;
  }

  const overlap = (a, b) => a.x < b.x + b.w && a.x + PW > b.x && a.y < b.y + b.h && a.y + PH > b.y;

  // ---- Game state ----------------------------------------------------------
  const levels = window.LEVELS.map(parseLevel);
  const game = {
    index: 0, lvl: null, p: null, deaths: 0, cornTotal: 0, time: 0,
    mode: 'play',        // 'play' | 'transition' | 'won'
    transition: 0, banner: 2.2, shake: 0,
    ghosts: [], particles: [], spin: 0, squash: 0,
  };

  function loadLevel(i) {
    game.index = i;
    game.lvl = parseLevel(window.LEVELS[i]);
    game.p = newPlayerState(game.lvl);
    game.banner = 2.2;
    game.ghosts.length = 0;
  }

  function restartRun() {
    game.deaths = 0; game.cornTotal = 0; game.time = 0; game.mode = 'play';
    loadLevel(0);
  }

  // ---- Input ---------------------------------------------------------------
  const held = new Set(), pressed = new Set();
  const GAME_KEYS = ['ArrowLeft', 'ArrowRight', 'ArrowUp', 'Space', 'KeyW', 'KeyA', 'KeyD', 'ShiftLeft', 'ShiftRight', 'KeyK', 'KeyR', 'Enter'];
  addEventListener('keydown', e => {
    if (GAME_KEYS.includes(e.code)) e.preventDefault();
    if (!held.has(e.code)) pressed.add(e.code);
    held.add(e.code);
  });
  addEventListener('keyup', e => held.delete(e.code));
  addEventListener('blur', () => held.clear());
  const isDown = (...k) => k.some(c => held.has(c));
  const wasPressed = (...k) => k.some(c => pressed.has(c));

  // ---- Effects -------------------------------------------------------------
  function puff(x, y, color, n = 10, speed = 160) {
    for (let i = 0; i < n; i++) {
      const a = Math.random() * Math.PI * 2, v = speed * (0.4 + Math.random() * 0.6);
      game.particles.push({ x, y, vx: Math.cos(a) * v, vy: Math.sin(a) * v, age: 0, life: 0.35 + Math.random() * 0.35, color, size: 3 + Math.random() * 3 });
    }
  }

  // ---- Update ----------------------------------------------------------------
  function update(dt) {
    if (game.mode === 'won') {
      if (wasPressed('Enter', 'KeyR')) restartRun();
      return;
    }
    if (game.mode === 'transition') {
      game.transition -= dt;
      if (game.transition <= 0) {
        if (game.index + 1 < levels.length) { loadLevel(game.index + 1); game.mode = 'play'; }
        else game.mode = 'won';
      }
      return;
    }

    game.time += dt;
    game.banner = Math.max(0, game.banner - dt);
    if (wasPressed('KeyR')) { game.deaths++; game.p = newPlayerState(game.lvl); }

    const p = game.p;
    const dir = (isDown('KeyD', 'ArrowRight') ? 1 : 0) - (isDown('KeyA', 'ArrowLeft') ? 1 : 0);
    const ev = stepPlayer(game.lvl, p, {
      dir,
      jumpPressed: wasPressed('Space', 'KeyW', 'ArrowUp'),
      jumpHeld: isDown('Space', 'KeyW', 'ArrowUp'),
      dashPressed: wasPressed('ShiftLeft', 'ShiftRight', 'KeyK'),
    }, dt);

    const cx = p.x + PW / 2, cy = p.y + PH / 2;
    if (ev.flapped) { game.spin = 0.35; puff(cx, p.y + PH, '#ffffff', 10, 170); }
    if (ev.dashed) puff(cx, cy, '#bff3ff', 12, 220);
    if (ev.sprung) { game.squash = 0.4; puff(cx, p.y + PH, '#ffd166', 10, 200); }
    if (ev.landed) game.squash = 0.25;
    if (p.dashT > 0) game.ghosts.push({ x: p.x, y: p.y, age: 0 });

    for (const c of game.lvl.corn) {
      if (!c.taken && overlap(p, c)) { c.taken = true; game.cornTotal++; puff(c.x + 8, c.y + 10, '#ffd23f', 10, 140); }
    }

    if (ev.dead) {
      game.deaths++;
      game.shake = 0.25;
      puff(cx, cy, '#ffffff', 26, 300);
      puff(cx, cy, '#9ee6dc', 14, 240);
      game.p = newPlayerState(game.lvl);
      game.ghosts.length = 0;
    } else if (ev.won) {
      game.mode = 'transition';
      game.transition = 1.2;
      puff(cx, cy, '#ffd23f', 30, 260);
    }
  }

  function updateEffects(dt) {
    game.spin = Math.max(0, game.spin - dt);
    game.squash *= Math.exp(-12 * dt);
    game.shake = Math.max(0, game.shake - dt);
    for (let i = game.ghosts.length - 1; i >= 0; i--) { game.ghosts[i].age += dt; if (game.ghosts[i].age > 0.25) game.ghosts.splice(i, 1); }
    for (let i = game.particles.length - 1; i >= 0; i--) {
      const q = game.particles[i];
      q.age += dt; q.x += q.vx * dt; q.y += q.vy * dt; q.vy += 300 * dt;
      if (q.age > q.life) game.particles.splice(i, 1);
    }
  }

  // ---- Drawing ---------------------------------------------------------------
  const sprite = new Image();
  sprite.src = 'blue_chicken.png';
  const SKIES = [['#7cc8ff', '#e3f5ff'], ['#ffb37a', '#ffe7c4'], ['#3a3266', '#9d7bd8']];

  function draw() {
    const lvl = game.lvl;
    ctx.save();
    if (game.shake > 0) ctx.translate((Math.random() - 0.5) * 8, (Math.random() - 0.5) * 8);

    const [top, bottom] = SKIES[game.index % SKIES.length];
    const g = ctx.createLinearGradient(0, 0, 0, H);
    g.addColorStop(0, top); g.addColorStop(1, bottom);
    ctx.fillStyle = g; ctx.fillRect(-10, -10, W + 20, H + 20);

    for (let r = 0; r < ROWS; r++) for (let c = 0; c < COLS; c++) drawTile(lvl, lvl.grid[r][c], c, r);

    for (const k of lvl.corn) if (!k.taken) drawCorn(k);
    if (lvl.flag) drawFlag(lvl.flag);

    for (const gh of game.ghosts) drawChicken(gh.x, gh.y, { alpha: 0.4 * (1 - gh.age / 0.25), tint: 'rgba(90,200,255,.6)' });
    if (game.mode !== 'won') drawChicken(game.p.x, game.p.y, {});

    for (const q of game.particles) {
      ctx.globalAlpha = 1 - q.age / q.life; ctx.fillStyle = q.color;
      ctx.fillRect(q.x - q.size / 2, q.y - q.size / 2, q.size, q.size);
    }
    ctx.globalAlpha = 1;
    ctx.restore();

    drawHud();
    if (game.mode === 'transition') {
      ctx.fillStyle = `rgba(15,20,35,${Math.min(1, (1.2 - game.transition) / 0.6)})`;
      ctx.fillRect(0, 0, W, H);
      centerText(game.index + 1 < levels.length ? 'Level complete!' : 'You made it!', H / 2, 34, '#fff');
    }
    if (game.mode === 'won') drawWinScreen();
  }

  function drawTile(lvl, ch, c, r) {
    const x = c * TILE, y = r * TILE;
    if (ch === '#') {
      ctx.fillStyle = '#7a5235'; ctx.fillRect(x, y, TILE, TILE);
      if (!isSolid(tileAt(lvl, c, r - 1))) { ctx.fillStyle = '#5cbf4a'; ctx.fillRect(x, y, TILE, 8); ctx.fillStyle = '#4aa83a'; ctx.fillRect(x, y + 8, TILE, 3); }
      ctx.fillStyle = 'rgba(0,0,0,.12)'; ctx.fillRect(x + 6, y + 16, 5, 4); ctx.fillRect(x + 20, y + 24, 4, 3);
    } else if (ch === '^') {
      ctx.fillStyle = '#d9dee8';
      for (let i = 0; i < 4; i++) {
        ctx.beginPath(); ctx.moveTo(x + i * 8, y + TILE); ctx.lineTo(x + i * 8 + 4, y + 12); ctx.lineTo(x + i * 8 + 8, y + TILE); ctx.fill();
      }
      ctx.fillStyle = '#9aa3b5'; ctx.fillRect(x, y + TILE - 4, TILE, 4);
    } else if (ch === 'S') {
      ctx.fillStyle = '#555'; ctx.fillRect(x + 4, y + 18, TILE - 8, 14);
      ctx.strokeStyle = '#ccc'; ctx.lineWidth = 2;
      ctx.beginPath(); for (let i = 0; i < 4; i++) { ctx.moveTo(x + 6, y + 18 - i * 3); ctx.lineTo(x + TILE - 6, y + 16 - i * 3); } ctx.stroke();
      ctx.fillStyle = '#ff6b6b'; ctx.fillRect(x + 2, y + 4, TILE - 4, 6);
    }
  }

  function drawCorn(k) {
    const bob = Math.sin(performance.now() / 250 + k.x) * 2;
    ctx.fillStyle = '#6ccf4f';
    ctx.beginPath(); ctx.ellipse(k.x + 4, k.y + 13 + bob, 4, 9, -0.4, 0, Math.PI * 2); ctx.fill();
    ctx.fillStyle = '#ffd23f';
    ctx.beginPath(); ctx.ellipse(k.x + 9, k.y + 10 + bob, 6, 10, 0, 0, Math.PI * 2); ctx.fill();
    ctx.fillStyle = '#e6b400';
    for (let i = 0; i < 4; i++) ctx.fillRect(k.x + 6, k.y + 3 + i * 4 + bob, 6, 1);
  }

  function drawFlag(f) {
    ctx.fillStyle = '#444'; ctx.fillRect(f.x, f.y, 4, f.h);
    const wave = Math.sin(performance.now() / 200) * 3;
    ctx.fillStyle = '#ff6b6b';
    ctx.beginPath(); ctx.moveTo(f.x + 4, f.y + 2); ctx.lineTo(f.x + 30, f.y + 10 + wave); ctx.lineTo(f.x + 4, f.y + 20); ctx.fill();
  }

  function drawChicken(x, y, { alpha = 1, tint = null }) {
    const w = 34, h = 36; // drawn slightly bigger than the hitbox so it reads well
    const sq = game.squash;
    ctx.save();
    ctx.globalAlpha = alpha;
    ctx.translate(x + PW / 2, y + PH - h / 2 + 2);
    if (!tint && game.spin > 0) ctx.rotate((1 - game.spin / 0.35) * Math.PI * 2 * game.p.facing);
    ctx.scale(1 + sq, 1 - sq);
    if (sprite.complete && sprite.naturalWidth) ctx.drawImage(sprite, -w / 2, -h / 2, w, h);
    else { ctx.fillStyle = '#7fe3d2'; ctx.fillRect(-w / 2, -h / 2, w, h); }
    if (tint) { ctx.globalCompositeOperation = 'source-atop'; ctx.fillStyle = tint; ctx.fillRect(-w / 2, -h / 2, w, h); }
    ctx.restore();
  }

  function centerText(text, y, size, color) {
    ctx.font = `800 ${size}px system-ui, sans-serif`;
    ctx.textAlign = 'center';
    ctx.fillStyle = 'rgba(0,0,0,.35)'; ctx.fillText(text, W / 2 + 2, y + 2);
    ctx.fillStyle = color; ctx.fillText(text, W / 2, y);
    ctx.textAlign = 'left';
  }

  const fmtTime = t => `${Math.floor(t / 60)}:${(t % 60).toFixed(1).padStart(4, '0')}`;

  function drawHud() {
    const total = game.lvl.corn.length, got = game.lvl.corn.filter(c => c.taken).length;
    ctx.fillStyle = 'rgba(20,30,45,.6)';
    ctx.fillRect(10, 10, 430, 30);
    ctx.fillStyle = '#fff';
    ctx.font = '600 15px system-ui, sans-serif';
    ctx.fillText(`Level ${game.index + 1}/${levels.length}: ${game.lvl.name}   Corn ${got}/${total}   Deaths ${game.deaths}   ${fmtTime(game.time)}`, 20, 30);
    if (game.mode === 'play' && game.banner > 0) {
      ctx.globalAlpha = Math.min(1, game.banner);
      centerText(game.lvl.name, 150, 40, '#fff');
      ctx.font = '600 17px system-ui, sans-serif';
      ctx.textAlign = 'center'; ctx.fillStyle = '#fff'; ctx.fillText(game.lvl.tip, W / 2, 182); ctx.textAlign = 'left';
      ctx.globalAlpha = 1;
    }
  }

  function drawWinScreen() {
    ctx.fillStyle = 'rgba(15,20,35,.85)'; ctx.fillRect(0, 0, W, H);
    const cornAll = window.LEVELS.reduce((n, l) => n + l.map.join('').split('o').length - 1, 0);
    centerText('Chicken Run complete!', 190, 40, '#ffd23f');
    centerText(`Time ${fmtTime(game.time)}   Deaths ${game.deaths}   Corn ${game.cornTotal}/${cornAll}`, 250, 22, '#fff');
    centerText('Press Enter or R to run it again', 310, 18, '#cfd8ea');
  }

  // ---- Loop ------------------------------------------------------------------
  let last = performance.now();
  function frame(now) {
    const dt = Math.min((now - last) / 1000, 1 / 30);
    last = now;
    update(dt);
    updateEffects(dt);
    draw();
    pressed.clear();
    requestAnimationFrame(frame);
  }

  restartRun();
  requestAnimationFrame(t => { last = t; requestAnimationFrame(frame); });

  // Exposed so the levels can be checked automatically (see README).
  window.ChickenRun = { game, levels, parseLevel, newPlayerState, stepPlayer, TILE, PW, PH, loadLevel };
})();
