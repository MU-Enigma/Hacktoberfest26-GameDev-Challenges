// core.js - a tiny 2D platformer base for the blue chicken.
// Each Level 2 mechanic lives in its own file and plugs in with Game.addMechanic().
(() => {
  const W = 960, H = 540;
  const canvas = document.getElementById('game');
  const ctx = canvas.getContext('2d');
  ctx.imageSmoothingEnabled = false;

  // ---- Input -------------------------------------------------------------
  // `held` is true while a key is down; `pressed` is true only on the frame it went down.
  const held = new Set(), justPressed = new Set();
  const GAME_KEYS = ['ArrowLeft', 'ArrowRight', 'ArrowUp', 'ArrowDown', 'Space', 'KeyW', 'KeyA', 'KeyS', 'KeyD',
    'KeyE', 'KeyQ', 'KeyG', 'KeyR', 'ShiftLeft', 'ShiftRight', 'KeyK'];
  addEventListener('keydown', e => {
    if (GAME_KEYS.includes(e.code)) e.preventDefault();
    if (!held.has(e.code)) justPressed.add(e.code);
    held.add(e.code);
  });
  addEventListener('keyup', e => held.delete(e.code));
  addEventListener('blur', () => held.clear());
  const input = {
    down: (...codes) => codes.some(c => held.has(c)),
    pressed: (...codes) => codes.some(c => justPressed.has(c)),
  };
  const JUMP = ['Space', 'KeyW', 'ArrowUp'];

  // ---- World -------------------------------------------------------------
  const platforms = [
    { x: 0, y: 480, w: 960, h: 60, ground: true },
    { x: 0, y: 0, w: 960, h: 18, ceiling: true },
    { x: 130, y: 360, w: 180, h: 18 },
    { x: 400, y: 280, w: 160, h: 18 },
    { x: 650, y: 360, w: 200, h: 18 },
    { x: 260, y: 185, w: 130, h: 18 },
    { x: 700, y: 200, w: 150, h: 18 },
  ];

  const PHYS = { accel: 2600, friction: 2400, maxSpeed: 300, gravity: 1900, jump: 720, maxFall: 1100 };

  const player = {
    x: 70, y: 420, baseW: 46, baseH: 50, w: 46, h: 50,
    vx: 0, vy: 0, facing: 1,
    onGround: false, coyote: 0,
    gravityDir: 1,      // 1 = normal, -1 = upside down
    scale: 1,           // size multiplier (mechanics can change it)
    speedMul: 1, jumpMul: 1,
    ignoreGravity: false, lockControls: false, // set by mechanics for one frame at a time
    rotation: 0, squash: 0, alpha: 1, tint: null,
    spawn: { x: 70, y: 420 },
  };

  const overlaps = (a, b) => a.x < b.x + b.w && a.x + a.w > b.x && a.y < b.y + b.h && a.y + a.h > b.y;

  // Move any box through the world, resolving collisions one axis at a time.
  // Returns which sides were hit so callers can react (landing, bonking, etc).
  function moveBox(box, dt, gravityDir = 1) {
    const hit = { left: false, right: false, top: false, bottom: false };
    box.x += box.vx * dt;
    for (const s of platforms) {
      if (!overlaps(box, s)) continue;
      if (box.vx > 0) { box.x = s.x - box.w; hit.right = true; }
      else if (box.vx < 0) { box.x = s.x + s.w; hit.left = true; }
      box.vx = 0;
    }
    if (box.x < 0) { box.x = 0; box.vx = 0; hit.left = true; }
    if (box.x + box.w > W) { box.x = W - box.w; box.vx = 0; hit.right = true; }

    box.y += box.vy * dt;
    for (const s of platforms) {
      if (!overlaps(box, s)) continue;
      if (box.vy > 0) { box.y = s.y - box.h; hit.bottom = true; }
      else if (box.vy < 0) { box.y = s.y + s.h; hit.top = true; }
      box.vy = 0;
    }
    // "Landed" means we hit the surface that gravity is pulling us toward.
    hit.landed = gravityDir > 0 ? hit.bottom : hit.top;
    return hit;
  }

  // Change the chicken's size while keeping its feet planted.
  function setScale(s) {
    const p = player;
    const feetY = p.gravityDir > 0 ? p.y + p.h : p.y;
    const cx = p.x + p.w / 2;
    p.scale = s;
    p.w = p.baseW * s;
    p.h = p.baseH * s;
    p.x = Math.max(0, Math.min(W - p.w, cx - p.w / 2));
    p.y = p.gravityDir > 0 ? feetY - p.h : feetY;
    unstick(p);
  }

  // If a box ends up inside a platform (say, after growing), push it out the shortest way.
  function unstick(box) {
    for (let pass = 0; pass < 4; pass++) {
      const s = platforms.find(s => overlaps(box, s));
      if (!s) return;
      const pushes = [
        { dx: s.x - (box.x + box.w), dy: 0 },   // out to the left
        { dx: s.x + s.w - box.x, dy: 0 },       // out to the right
        { dx: 0, dy: s.y - (box.y + box.h) },   // out the top
        { dx: 0, dy: s.y + s.h - box.y },       // out the bottom
      ].filter(m => box.x + m.dx >= 0 && box.x + m.dx + box.w <= W);
      const best = pushes.reduce((a, b) => (Math.abs(a.dx) + Math.abs(a.dy) <= Math.abs(b.dx) + Math.abs(b.dy) ? a : b));
      box.x += best.dx; box.y += best.dy;
    }
  }

  function respawn() {
    const p = player;
    p.x = p.spawn.x; p.y = p.spawn.y; p.vx = p.vy = 0; p.gravityDir = 1;
    setScale(1);
    mechanics.forEach(m => m.onRespawn && m.onRespawn(p));
  }

  function updatePlayer(dt) {
    const p = player;
    let dir = 0;
    if (!p.lockControls) {
      if (input.down('KeyA', 'ArrowLeft')) dir -= 1;
      if (input.down('KeyD', 'ArrowRight')) dir += 1;
    }
    const maxSpeed = PHYS.maxSpeed * p.speedMul;
    if (dir) {
      p.vx += dir * PHYS.accel * dt;
      p.facing = dir;
    } else if (!p.lockControls) {
      const f = PHYS.friction * dt;
      p.vx = Math.abs(p.vx) <= f ? 0 : p.vx - Math.sign(p.vx) * f;
    }
    if (!p.lockControls) p.vx = Math.max(-maxSpeed, Math.min(maxSpeed, p.vx));

    // Jumping: a short "coyote time" lets you still jump just after walking off a ledge.
    p.coyote = p.onGround ? 0.09 : Math.max(0, p.coyote - dt);
    if (!p.lockControls && input.pressed(...JUMP)) {
      if (p.onGround || p.coyote > 0) {
        p.vy = -p.gravityDir * PHYS.jump * p.jumpMul;
        p.onGround = false; p.coyote = 0; p.squash = -0.25;
        mechanics.forEach(m => m.onJump && m.onJump(p));
      } else {
        // Let a mechanic (like double jump) handle a jump press in mid-air.
        mechanics.some(m => m.onAirJump && m.onAirJump(p));
      }
    }
    // Releasing jump early gives a shorter hop.
    if (!input.down(...JUMP) && p.vy * p.gravityDir < -PHYS.jump * 0.45) p.vy *= 0.85;

    if (!p.ignoreGravity) {
      p.vy += PHYS.gravity * p.gravityDir * dt;
      p.vy = Math.max(-PHYS.maxFall, Math.min(PHYS.maxFall, p.vy));
    }

    const wasGround = p.onGround;
    const fallSpeed = p.vy * p.gravityDir;
    const hit = moveBox(p, dt, p.gravityDir);
    p.onGround = hit.landed;
    if (p.onGround && !wasGround) {
      p.squash = Math.min(0.35, fallSpeed / 2400);
      mechanics.forEach(m => m.onLand && m.onLand(p));
    }
    p.squash *= Math.exp(-12 * dt);

    if (p.y > H + 300 || p.y < -300) respawn();
  }

  // ---- Particles & floating text ------------------------------------------
  const particles = [];
  function puff(x, y, color = '#fff', count = 10, speed = 160) {
    for (let i = 0; i < count; i++) {
      const a = Math.random() * Math.PI * 2, v = speed * (0.4 + Math.random() * 0.6);
      particles.push({ x, y, vx: Math.cos(a) * v, vy: Math.sin(a) * v, life: 0.4 + Math.random() * 0.4, age: 0, size: 3 + Math.random() * 4, color });
    }
  }
  const texts = [];
  function floatText(x, y, text, color = '#fff') { texts.push({ x, y, text, color, age: 0 }); }

  function updateEffects(dt) {
    for (let i = particles.length - 1; i >= 0; i--) {
      const q = particles[i];
      q.age += dt; q.x += q.vx * dt; q.y += q.vy * dt; q.vy += 300 * dt;
      if (q.age > q.life) particles.splice(i, 1);
    }
    for (let i = texts.length - 1; i >= 0; i--) {
      texts[i].age += dt; texts[i].y -= 40 * dt;
      if (texts[i].age > 1) texts.splice(i, 1);
    }
  }

  // ---- Drawing -------------------------------------------------------------
  const sprite = new Image();
  sprite.src = 'blue_chicken.png';

  const theme = { skyTop: '#6ec6ff', skyBottom: '#d6f1ff' };

  function drawBackground() {
    const g = ctx.createLinearGradient(0, 0, 0, H);
    g.addColorStop(0, theme.skyTop); g.addColorStop(1, theme.skyBottom);
    ctx.fillStyle = g; ctx.fillRect(0, 0, W, H);
    ctx.fillStyle = 'rgba(255,255,255,.7)';
    for (const [cx, cy, r] of [[150, 90, 28], [190, 80, 36], [230, 92, 26], [620, 120, 30], [665, 108, 40], [710, 122, 28]]) {
      ctx.beginPath(); ctx.arc(cx, cy, r, 0, Math.PI * 2); ctx.fill();
    }
  }

  function drawPlatforms() {
    for (const s of platforms) {
      if (s.ceiling) { ctx.fillStyle = '#5b4a3b'; ctx.fillRect(s.x, s.y, s.w, s.h); continue; }
      ctx.fillStyle = '#8b5e3c'; ctx.fillRect(s.x, s.y, s.w, s.h);
      ctx.fillStyle = '#5cbf4a'; ctx.fillRect(s.x, s.y, s.w, Math.min(10, s.h));
    }
  }

  // Draw the chicken at a given box. Used for the player and for effects like afterimages.
  function drawChicken(box, opts = {}) {
    const p = Object.assign({}, player, box, opts);
    const sx = 1 + p.squash, sy = 1 - p.squash;
    ctx.save();
    ctx.globalAlpha = p.alpha;
    ctx.translate(p.x + p.w / 2, p.y + p.h / 2);
    ctx.rotate(p.rotation + (p.vx / PHYS.maxSpeed) * 0.08);
    ctx.scale(sx, sy * p.gravityDir);
    if (sprite.complete && sprite.naturalWidth) {
      ctx.drawImage(sprite, -p.w / 2, -p.h / 2, p.w, p.h);
      if (p.tint) {
        ctx.globalCompositeOperation = 'source-atop';
        ctx.fillStyle = p.tint;
        ctx.fillRect(-p.w / 2, -p.h / 2, p.w, p.h);
      }
    } else {
      ctx.fillStyle = '#7fe3d2'; ctx.fillRect(-p.w / 2, -p.h / 2, p.w, p.h);
    }
    ctx.restore();
  }

  function drawEffects() {
    for (const q of particles) {
      ctx.globalAlpha = 1 - q.age / q.life;
      ctx.fillStyle = q.color;
      ctx.fillRect(q.x - q.size / 2, q.y - q.size / 2, q.size, q.size);
    }
    ctx.globalAlpha = 1;
    ctx.textAlign = 'center';
    ctx.font = '700 18px system-ui, sans-serif';
    for (const t of texts) {
      ctx.globalAlpha = 1 - t.age;
      ctx.fillStyle = '#0006'; ctx.fillText(t.text, t.x + 2, t.y + 2);
      ctx.fillStyle = t.color; ctx.fillText(t.text, t.x, t.y);
    }
    ctx.globalAlpha = 1;
    ctx.textAlign = 'left';
  }

  const meta = { title: 'Level 2', controls: '' };
  function drawHud() {
    const lines = [meta.title, ...mechanics.flatMap(m => (m.hud ? [m.hud(player)] : [])).filter(Boolean)];
    ctx.font = '600 15px system-ui, sans-serif';
    const width = Math.max(...lines.map(l => ctx.measureText(l).width)) + 24;
    ctx.fillStyle = 'rgba(20,30,45,.6)';
    ctx.fillRect(12, 28, width, 12 + lines.length * 21);
    ctx.fillStyle = '#fff';
    lines.forEach((l, i) => ctx.fillText(l, 24, 50 + i * 21));
    ctx.fillStyle = 'rgba(20,30,45,.75)';
    ctx.font = '500 14px system-ui, sans-serif';
    ctx.fillText(meta.controls, 16, H - 16);
  }

  // ---- Mechanics & loop ------------------------------------------------------
  const mechanics = [];
  function addMechanic(m) { mechanics.push(m); return m; }

  let last = performance.now();
  function frame(now) {
    const dt = Math.min((now - last) / 1000, 1 / 30);
    last = now;
    step(dt);
    requestAnimationFrame(frame);
  }

  function step(dt) {
    player.ignoreGravity = false;
    player.lockControls = false;
    if (input.pressed('KeyR')) respawn();
    mechanics.forEach(m => m.update && m.update(player, dt));
    updatePlayer(dt);
    mechanics.forEach(m => m.afterPhysics && m.afterPhysics(player, dt));
    updateEffects(dt);

    drawBackground();
    mechanics.forEach(m => m.drawBehind && m.drawBehind(ctx));
    drawPlatforms();
    mechanics.forEach(m => m.drawWorld && m.drawWorld(ctx));
    drawChicken(player);
    mechanics.forEach(m => m.drawFront && m.drawFront(ctx));
    drawEffects();
    drawHud();
    justPressed.clear();
  }

  window.Game = {
    W, H, ctx, input, platforms, PHYS, player, theme, meta,
    addMechanic, moveBox, overlaps, setScale, respawn, puff, floatText, drawChicken, step,
    start() { setScale(1); requestAnimationFrame(t => { last = t; requestAnimationFrame(frame); }); },
  };
})();
