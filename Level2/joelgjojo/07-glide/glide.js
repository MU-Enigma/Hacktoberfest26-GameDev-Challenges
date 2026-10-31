// Mechanic: Glide.
// Hold jump while falling to spread your wings and drift down slowly.
// Wind gusts blow across the level: glide through one and it carries you sideways.
(() => {
  const { player, input, puff, meta, W } = Game;
  meta.title = 'Level 2 - Glide';
  meta.controls = 'A/D or arrows: move   Space/W: jump, HOLD while falling to glide   R: reset';

  const GLIDE_FALL = 110;     // max fall speed while gliding (normal is much faster)
  const GLIDE_STEER = 1.25;   // a little faster sideways while gliding
  const WIND_SPEED = 170;     // px/s the wind carries you sideways inside a gust

  // Gusts of wind: rectangles that push a gliding chicken left or right.
  const gusts = [
    { x: 330, y: 60, w: 260, h: 110, dir: 1 },
    { x: 560, y: 400, w: 330, h: 70, dir: -1 },
  ];
  const state = { gliding: false, glideTime: 0, flap: 0 };
  const JUMP = ['Space', 'KeyW', 'ArrowUp'];

  Game.addMechanic({
    update(p) {
      // Glide when: in the air, holding jump, and already falling.
      const falling = p.vy * p.gravityDir > 0;
      state.gliding = !p.onGround && falling && input.down(...JUMP);
      p.speedMul = state.gliding ? GLIDE_STEER : 1;
    },
    afterPhysics(p, dt) {
      if (state.gliding) {
        state.glideTime += dt;
        state.flap += dt * 14;
        p.vy = Math.sign(p.vy) * Math.min(Math.abs(p.vy), GLIDE_FALL);
        for (const g of gusts) {
          if (!Game.overlaps(p, g)) continue;
          // Carry the chicken sideways directly (friction would cancel a velocity push),
          // but never blow it into a platform or off the screen.
          const moved = { x: Math.max(0, Math.min(W - p.w, p.x + g.dir * WIND_SPEED * dt)), y: p.y, w: p.w, h: p.h };
          if (!Game.platforms.some(s => Game.overlaps(moved, s))) p.x = moved.x;
        }
        if (Math.random() < 0.25) puff(p.x + p.w / 2, p.y + p.h / 2, '#ffffff', 1, 40);
        p.rotation = Math.sin(state.flap) * 0.08;
      } else {
        p.rotation = 0;
      }
    },
    onRespawn() { state.gliding = false; },
    drawBehind(ctx) {
      // Wind gusts: faint bands with streaks drifting in the wind's direction.
      const t = performance.now() / 1000;
      for (const g of gusts) {
        ctx.fillStyle = 'rgba(255,255,255,.18)';
        ctx.fillRect(g.x, g.y, g.w, g.h);
        ctx.strokeStyle = 'rgba(255,255,255,.7)'; ctx.lineWidth = 2;
        for (let i = 0; i < 6; i++) {
          const yy = g.y + 12 + ((i * 37) % (g.h - 20));
          const xx = g.x + (((t * 160 * g.dir + i * 71) % g.w) + g.w) % g.w;
          ctx.beginPath(); ctx.moveTo(xx, yy); ctx.lineTo(Math.max(g.x, Math.min(g.x + g.w, xx + 26 * g.dir)), yy); ctx.stroke();
        }
      }
    },
    drawWorld(ctx) {
      if (!state.gliding) return;
      // Wings: two flapping triangles behind the chicken.
      const cx = player.x + player.w / 2, cy = player.y + player.h * 0.45;
      const lift = Math.sin(state.flap) * 8;
      ctx.fillStyle = '#e9fffb'; ctx.strokeStyle = '#7fd3c6'; ctx.lineWidth = 2;
      for (const side of [-1, 1]) {
        ctx.beginPath();
        ctx.moveTo(cx + side * player.w * 0.3, cy);
        ctx.lineTo(cx + side * player.w * 1.25, cy - 12 - lift);
        ctx.lineTo(cx + side * player.w * 0.95, cy + 10);
        ctx.closePath(); ctx.fill(); ctx.stroke();
      }
    },
    hud(p) { return `${state.gliding ? 'Gliding!' : p.onGround ? 'On the ground' : 'Hold jump to glide'}   Time in the air gliding: ${state.glideTime.toFixed(1)}s`; },
  });

  window.glideState = state;
  Game.start();
})();
