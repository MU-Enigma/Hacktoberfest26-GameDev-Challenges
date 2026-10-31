// Mechanic: Ground pound.
// In mid-air, press S (or Down) to freeze for a moment, then slam straight down.
// Landing sends out a shockwave, and slamming onto a crate smashes it to bits.
(() => {
  const { player, input, platforms, puff, floatText, meta } = Game;
  meta.title = 'Level 2 - Ground Pound';
  meta.controls = 'A/D or arrows: move   Space/W: jump   S/Down in the air: GROUND POUND   R: reset';

  const HANG_TIME = 0.12;     // the little pause in the air before slamming
  const SLAM_SPEED = 1400;    // px/s straight down
  const STUN = 0.18;          // can't move for a moment after landing

  // Wooden crates sitting on the platforms. Only a ground pound breaks them.
  const CRATE_SPOTS = [
    { x: 196, y: 312 }, { x: 456, y: 232 }, { x: 724, y: 312 }, { x: 300, y: 137 }, { x: 750, y: 152 },
  ];
  const crates = [];
  function placeCrates() {
    for (const c of crates) { const i = platforms.indexOf(c); if (i >= 0) platforms.splice(i, 1); }
    crates.length = 0;
    for (const s of CRATE_SPOTS) { const c = { x: s.x, y: s.y, w: 48, h: 48, crate: true }; crates.push(c); platforms.push(c); }
  }
  placeCrates();

  const state = { phase: 'none', timer: 0, slams: 0, smashed: 0, rings: [], shake: 0 };

  Game.addMechanic({
    update(p, dt) {
      state.shake = Math.max(0, state.shake - dt);
      if (state.phase === 'none' && !p.onGround && p.gravityDir > 0 && input.pressed('KeyS', 'ArrowDown')) {
        state.phase = 'hang'; state.timer = HANG_TIME;
      }
      if (state.phase === 'hang') {
        state.timer -= dt;
        p.vx = 0; p.vy = 0; p.ignoreGravity = true; p.lockControls = true;
        p.rotation = (1 - state.timer / HANG_TIME) * Math.PI * 2 * p.facing; // quick spin before the slam
        if (state.timer <= 0) { state.phase = 'slam'; p.rotation = 0; }
      } else if (state.phase === 'slam') {
        p.vx = 0; p.vy = SLAM_SPEED; p.ignoreGravity = true; p.lockControls = true;
        p.squash = -0.3; // stretched tall while falling
      } else if (state.phase === 'stun') {
        state.timer -= dt; p.lockControls = true; p.vx = 0;
        if (state.timer <= 0) state.phase = 'none';
      }
    },
    onLand(p) {
      if (state.phase !== 'slam') return;
      state.phase = 'stun'; state.timer = STUN; state.slams++;
      p.squash = 0.45; state.shake = 0.2;
      const fx = p.x + p.w / 2, fy = p.y + p.h;
      state.rings.push({ x: fx, y: fy, age: 0 });
      puff(fx, fy, '#c8a27a', 16, 260);

      // Smash any crate we landed on, then keep falling.
      const under = crates.filter(c => Math.abs(c.y - fy) < 2 && c.x < p.x + p.w && c.x + c.w > p.x);
      for (const c of under) {
        platforms.splice(platforms.indexOf(c), 1);
        crates.splice(crates.indexOf(c), 1);
        state.smashed++;
        puff(c.x + c.w / 2, c.y + c.h / 2, '#a8743f', 24, 300);
        puff(c.x + c.w / 2, c.y + c.h / 2, '#6b4423', 12, 220);
        floatText(c.x + c.w / 2, c.y - 10, 'CRUNCH', '#ffd166');
      }
      if (under.length) { state.phase = 'slam'; p.onGround = false; }
      else floatText(fx, p.y - 10, 'BOOM', '#fff');
    },
    afterPhysics(p, dt) {
      for (let i = state.rings.length - 1; i >= 0; i--) { state.rings[i].age += dt; if (state.rings[i].age > 0.4) state.rings.splice(i, 1); }
      if (state.shake > 0) Game.ctx.translate((Math.random() - 0.5) * 10, (Math.random() - 0.5) * 6);
    },
    onRespawn(p) { state.phase = 'none'; p.rotation = 0; placeCrates(); },
    drawWorld(ctx) {
      for (const c of crates) {
        ctx.fillStyle = '#b07a43'; ctx.fillRect(c.x, c.y, c.w, c.h);
        ctx.strokeStyle = '#6b4423'; ctx.lineWidth = 4; ctx.strokeRect(c.x + 2, c.y + 2, c.w - 4, c.h - 4);
        ctx.beginPath(); ctx.moveTo(c.x + 4, c.y + 4); ctx.lineTo(c.x + c.w - 4, c.y + c.h - 4);
        ctx.moveTo(c.x + c.w - 4, c.y + 4); ctx.lineTo(c.x + 4, c.y + c.h - 4); ctx.stroke();
      }
    },
    drawFront(ctx) {
      for (const r of state.rings) {
        const t = r.age / 0.4;
        ctx.strokeStyle = `rgba(255,255,255,${1 - t})`; ctx.lineWidth = 4;
        ctx.beginPath(); ctx.ellipse(r.x, r.y, 20 + t * 120, 6 + t * 18, 0, 0, Math.PI * 2); ctx.stroke();
      }
      ctx.setTransform(1, 0, 0, 1, 0, 0); // undo this frame's shake before the HUD is drawn
    },
    hud() { return `Slams: ${state.slams}   Crates smashed: ${state.smashed}/${CRATE_SPOTS.length}`; },
  });

  window.poundState = state;
  window.crates = crates;
  Game.start();
})();
