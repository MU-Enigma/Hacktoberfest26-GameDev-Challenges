// Mechanic: Wall slide + wall jump.
// Push into a wall while falling and the chicken slides down it slowly, claws scraping.
// Press jump while sliding to kick off the wall. Chain kicks between two walls to climb a shaft.
(() => {
  const { player, input, platforms, puff, floatText, PHYS, meta } = Game;
  meta.title = 'Level 2 - Wall Jump';
  meta.controls = 'A/D or arrows: move (push into a wall to slide)   Space/W: jump, and jump off walls   R: reset';

  const SLIDE_SPEED = 90;    // max fall speed while sliding
  const KICK_X = 430;        // sideways push off the wall
  const KICK_Y = 0.92;       // fraction of a normal jump
  const KICK_LOCK = 0.16;    // seconds you can't steer after a kick, so you actually leave the wall

  // A tall shaft in the bottom-right corner. Walk in through the gap at the bottom of the
  // left wall, then kick back and forth between the walls to reach the star at the top.
  const walls = [
    { x: 852, y: 110, w: 24, h: 290, wall: true },
    { x: 940, y: 30, w: 20, h: 450, wall: true },
  ];
  platforms.push(...walls);
  const goal = { x: 893, y: 60, w: 30, h: 30 };

  const state = { wallDir: 0, sliding: false, lock: 0, kicks: 0, reachedTop: false };

  // Is there a solid platform right beside the chicken on this side?
  const touching = (p, dir) => {
    const probe = { x: dir < 0 ? p.x - 1 : p.x + p.w, y: p.y + 4, w: 1, h: p.h - 8 };
    return platforms.some(s => !s.ground && !s.ceiling && Game.overlaps(probe, s));
  };

  Game.addMechanic({
    update(p, dt) {
      if (state.lock > 0) { state.lock -= dt; p.lockControls = true; }
    },
    onAirJump(p) {
      if (!state.wallDir) return false;
      // Kick away from the wall.
      p.vy = -p.gravityDir * PHYS.jump * KICK_Y;
      p.vx = -state.wallDir * KICK_X;
      p.facing = -state.wallDir;
      state.lock = KICK_LOCK; state.kicks++;
      p.squash = -0.2;
      puff(state.wallDir > 0 ? p.x + p.w : p.x, p.y + p.h / 2, '#ffffff', 10, 160);
      return true;
    },
    afterPhysics(p, dt) {
      const holding = (input.down('KeyD', 'ArrowRight') ? 1 : 0) - (input.down('KeyA', 'ArrowLeft') ? 1 : 0);
      state.wallDir = 0;
      if (!p.onGround) {
        if (touching(p, 1)) state.wallDir = 1; else if (touching(p, -1)) state.wallDir = -1;
      }
      // Slide only when pushing into the wall and falling.
      state.sliding = state.wallDir !== 0 && holding === state.wallDir && p.vy * p.gravityDir > 0;
      if (state.sliding) {
        p.vy = Math.min(p.vy, SLIDE_SPEED);
        p.facing = state.wallDir;
        if (Math.random() < 0.4) puff(state.wallDir > 0 ? p.x + p.w : p.x, p.y + p.h - 6, '#d8d0c0', 1, 50);
      }
      if (!state.reachedTop && Game.overlaps(p, goal)) {
        state.reachedTop = true;
        puff(goal.x + 15, goal.y + 15, '#ffd23f', 30, 240);
        floatText(goal.x - 30, goal.y + 10, 'TOP OF THE SHAFT!', '#ffd23f');
      }
    },
    onLand() { state.lock = 0; },
    onRespawn() { state.lock = 0; state.reachedTop = false; },
    drawWorld(ctx) {
      for (const w of walls) {
        ctx.fillStyle = '#8b5e3c'; ctx.fillRect(w.x, w.y, w.w, w.h);
        ctx.fillStyle = 'rgba(0,0,0,.15)';
        for (let y = w.y + 10; y < w.y + w.h; y += 26) ctx.fillRect(w.x + 3, y, w.w - 6, 3);
      }
      if (!state.reachedTop) {
        ctx.fillStyle = '#ffd23f';
        ctx.beginPath(); ctx.arc(goal.x + 15, goal.y + 15 + Math.sin(performance.now() / 200) * 3, 11, 0, Math.PI * 2); ctx.fill();
        ctx.fillStyle = '#e6b400'; ctx.font = '700 13px system-ui'; ctx.textAlign = 'center';
        ctx.fillText('★', goal.x + 15, goal.y + 20); ctx.textAlign = 'left';
      }
    },
    hud() {
      const s = state.sliding ? 'Sliding down the wall' : state.wallDir ? 'Touching a wall: jump to kick off' : 'Find a wall';
      return `${s}   Wall kicks: ${state.kicks}${state.reachedTop ? '   Star collected!' : ''}`;
    },
  });

  window.wallState = state;
  Game.start();
})();
