// Mechanic: Dash.
// Press Shift (or K) to burst forward, leaving a trail of ghost chickens.
// One dash in the air until you land again, with a short cooldown.
(() => {
  const { player, input, puff, drawChicken, meta } = Game;
  meta.title = 'Level 2 - Dash';
  meta.controls = 'A/D or arrows: move   Space/W: jump   Shift or K: DASH   R: reset';

  const DASH_SPEED = 900;     // px/s
  const DASH_TIME = 0.16;     // seconds
  const COOLDOWN = 0.55;      // seconds

  const state = { timeLeft: 0, cooldown: 0, airDashUsed: false, dir: 1 };
  const ghosts = [];

  Game.addMechanic({
    update(p, dt) {
      state.cooldown = Math.max(0, state.cooldown - dt);

      const canDash = state.cooldown === 0 && state.timeLeft === 0 && !(state.airDashUsed && !p.onGround);
      if (canDash && input.pressed('ShiftLeft', 'ShiftRight', 'KeyK')) {
        // Dash toward whatever you're holding, or the way you're facing.
        const held = (input.down('KeyD', 'ArrowRight') ? 1 : 0) - (input.down('KeyA', 'ArrowLeft') ? 1 : 0);
        state.dir = held || p.facing;
        state.timeLeft = DASH_TIME;
        state.cooldown = COOLDOWN;
        if (!p.onGround) state.airDashUsed = true;
        puff(p.x + p.w / 2, p.y + p.h / 2, '#bff3ff', 14, 220);
      }

      if (state.timeLeft > 0) {
        state.timeLeft = Math.max(0, state.timeLeft - dt);
        p.vx = state.dir * DASH_SPEED;
        p.vy = 0;
        p.ignoreGravity = true;   // dashes go perfectly straight
        p.lockControls = true;
        p.facing = state.dir;
        ghosts.push({ x: p.x, y: p.y, w: p.w, h: p.h, age: 0 });
        // Ease back to normal running speed at the end of the dash.
        if (state.timeLeft === 0) p.vx = state.dir * Game.PHYS.maxSpeed;
      }
    },
    afterPhysics(p, dt) {
      for (let i = ghosts.length - 1; i >= 0; i--) {
        ghosts[i].age += dt;
        if (ghosts[i].age > 0.28) ghosts.splice(i, 1);
      }
    },
    onLand() { state.airDashUsed = false; },
    onRespawn() { state.timeLeft = 0; state.cooldown = 0; state.airDashUsed = false; ghosts.length = 0; },
    drawBehind() {
      for (const g of ghosts) {
        drawChicken(g, { alpha: 0.45 * (1 - g.age / 0.28), tint: 'rgba(90,200,255,.65)', squash: 0.15, rotation: 0, vx: 0 });
      }
    },
    hud(p) {
      if (state.timeLeft > 0) return 'Dash: WHOOSH';
      if (state.cooldown > 0) return `Dash: recharging ${'|'.repeat(Math.ceil((1 - state.cooldown / COOLDOWN) * 10))}`;
      if (state.airDashUsed && !p.onGround) return 'Dash: land to recharge';
      return 'Dash: ready';
    },
  });

  // Exposed for tests in the browser console.
  window.dashState = state;
  Game.start();
})();
