// Mechanic: Double jump.
// Press jump again in mid-air for a second, flappier jump.
// The chicken does a full spin and sheds a burst of feathers.
(() => {
  const { input, puff, PHYS, meta } = Game;
  meta.title = 'Level 2 - Double Jump';
  meta.controls = 'A/D or arrows: move   Space/W: jump (press again in the air to flap)   R: reset';

  const EXTRA_JUMPS = 1;
  const FLAP_STRENGTH = 0.9;   // fraction of the normal jump speed
  const SPIN_TIME = 0.35;

  const state = { jumpsLeft: EXTRA_JUMPS, spin: 0, flaps: 0 };

  Game.addMechanic({
    onAirJump(p) {
      if (state.jumpsLeft <= 0) return false;
      state.jumpsLeft--;
      state.flaps++;
      p.vy = -p.gravityDir * PHYS.jump * FLAP_STRENGTH * p.jumpMul;
      state.spin = SPIN_TIME;
      p.squash = -0.2;
      // Feathers! Mostly white with a few of the chicken's own blue.
      puff(p.x + p.w / 2, p.y + p.h, '#ffffff', 12, 180);
      puff(p.x + p.w / 2, p.y + p.h, '#9ee6dc', 6, 140);
      Game.floatText(p.x + p.w / 2, p.y - 6, 'flap!', '#fff');
      return true;
    },
    onJump() { state.jumpsLeft = EXTRA_JUMPS; },
    onLand() { state.jumpsLeft = EXTRA_JUMPS; },
    onRespawn() { state.jumpsLeft = EXTRA_JUMPS; state.spin = 0; },
    update(p, dt) {
      // Walking off a ledge still leaves the air jump available.
      if (p.onGround) state.jumpsLeft = EXTRA_JUMPS;
      if (state.spin > 0) {
        state.spin = Math.max(0, state.spin - dt);
        p.rotation = (1 - state.spin / SPIN_TIME) * Math.PI * 2 * p.facing;
      } else {
        p.rotation = 0;
      }
    },
    hud(p) { return `Air jumps left: ${p.onGround ? EXTRA_JUMPS : state.jumpsLeft}   Total flaps: ${state.flaps}`; },
  });

  window.doubleJumpState = state;
  Game.start();
})();
