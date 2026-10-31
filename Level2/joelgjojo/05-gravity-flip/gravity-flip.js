// Mechanic: Gravity flip.
// Press G to flip gravity. The chicken falls up, walks on the ceiling upside down,
// and jumping pushes it "down" toward the floor. The sky turns to dusk while flipped.
(() => {
  const { input, puff, floatText, theme, meta, W } = Game;
  meta.title = 'Level 2 - Gravity Flip';
  meta.controls = 'A/D or arrows: move   Space/W: jump   G: flip gravity   R: reset';

  const COOLDOWN = 0.35;
  const FLIP_ANIM = 0.25;
  const state = { cooldown: 0, flips: 0, anim: 0, blend: 0 };

  const normalSky = ['#6ec6ff', '#d6f1ff'];
  const flippedSky = ['#3b2a63', '#f39c6b'];
  const mix = (a, b, t) => {
    const pa = a.match(/\w\w/g).map(h => parseInt(h, 16)), pb = b.match(/\w\w/g).map(h => parseInt(h, 16));
    return '#' + pa.map((v, i) => Math.round(v + (pb[i] - v) * t).toString(16).padStart(2, '0')).join('');
  };

  Game.addMechanic({
    update(p, dt) {
      state.cooldown = Math.max(0, state.cooldown - dt);
      if (state.cooldown === 0 && input.pressed('KeyG')) {
        p.gravityDir *= -1;
        p.vy *= 0.3;             // soften the old momentum so the flip feels snappy
        p.onGround = false;
        state.cooldown = COOLDOWN;
        state.anim = FLIP_ANIM;
        state.flips++;
        puff(p.x + p.w / 2, p.y + p.h / 2, '#e0c3ff', 16, 220);
        floatText(p.x + p.w / 2, p.y - 6, p.gravityDir < 0 ? 'up is down!' : 'down is down', '#fff');
      }
      // Spin half a turn during the flip so it doesn't just snap upside down.
      state.anim = Math.max(0, state.anim - dt);
      p.rotation = state.anim > 0 ? (state.anim / FLIP_ANIM) * Math.PI * p.facing : 0;

      // Fade the sky between day and dusk.
      const target = p.gravityDir < 0 ? 1 : 0;
      state.blend += (target - state.blend) * Math.min(1, dt * 6);
      theme.skyTop = mix(normalSky[0], flippedSky[0], state.blend);
      theme.skyBottom = mix(normalSky[1], flippedSky[1], state.blend);
    },
    onRespawn() { state.cooldown = 0; state.anim = 0; },
    drawFront(ctx) {
      // An arrow in the corner showing which way gravity pulls.
      const down = Game.player.gravityDir > 0;
      ctx.save();
      ctx.translate(W - 44, 70);
      ctx.fillStyle = 'rgba(20,30,45,.6)';
      ctx.beginPath(); ctx.arc(0, 0, 22, 0, Math.PI * 2); ctx.fill();
      ctx.rotate(down ? 0 : Math.PI);
      ctx.fillStyle = '#fff';
      ctx.beginPath(); ctx.moveTo(0, 14); ctx.lineTo(-10, 0); ctx.lineTo(-4, 0); ctx.lineTo(-4, -13); ctx.lineTo(4, -13); ctx.lineTo(4, 0); ctx.lineTo(10, 0); ctx.closePath(); ctx.fill();
      ctx.restore();
    },
    hud(p) { return `Gravity: ${p.gravityDir > 0 ? 'normal' : 'FLIPPED'}   Flips: ${state.flips}`; },
  });

  window.flipState = state;
  Game.start();
})();
