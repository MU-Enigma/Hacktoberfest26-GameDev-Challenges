// Mechanic: Egg teleport.
// Press Q to lob an egg. Press Q again to teleport to wherever the egg is,
// which cracks it open. An egg that sits too long hatches on its own.
(() => {
  const { player, input, moveBox, puff, floatText, PHYS, meta, H } = Game;
  meta.title = 'Level 2 - Egg Teleport';
  meta.controls = 'A/D or arrows: move   Space/W: jump   Q: throw egg, Q again: teleport to it   R: reset';

  const THROW_VX = 520, THROW_VY = 560;
  const EGG_LIFETIME = 4;          // seconds before it hatches on its own
  const state = { egg: null, teleports: 0 };

  function throwEgg(p) {
    state.egg = {
      x: p.x + p.w / 2 - 9, y: p.y + 6, w: 18, h: 22,
      vx: p.facing * THROW_VX + p.vx * 0.3, vy: -THROW_VY * p.gravityDir,
      age: 0, spin: 0, resting: false,
    };
    floatText(p.x + p.w / 2, p.y - 6, 'yeet', '#fff');
  }

  function teleportToEgg(p) {
    const e = state.egg;
    puff(p.x + p.w / 2, p.y + p.h / 2, '#c9b8ff', 18, 200);   // poof where we were
    // Put the chicken's feet where the egg's bottom was, centred on the egg.
    p.x = e.x + e.w / 2 - p.w / 2;
    p.y = e.y + e.h - p.h;
    p.x = Math.max(0, Math.min(Game.W - p.w, p.x));
    // If that spot is inside a platform, step up until we're clear.
    for (let tries = 0; tries < 40 && Game.platforms.some(s => Game.overlaps(p, s)); tries++) p.y -= 2;
    p.vx = 0; p.vy = 0;
    puff(e.x + e.w / 2, e.y + e.h / 2, '#fff8e1', 14, 160);   // shell bits
    puff(e.x + e.w / 2, e.y + e.h / 2, '#ffd166', 8, 120);    // yolk
    floatText(p.x + p.w / 2, p.y - 6, '*crack*', '#ffd166');
    state.egg = null;
    state.teleports++;
  }

  Game.addMechanic({
    update(p, dt) {
      if (input.pressed('KeyQ')) {
        if (state.egg) teleportToEgg(p); else throwEgg(p);
      }
      const e = state.egg;
      if (!e) return;
      e.age += dt;
      if (!e.resting) {
        e.vy = Math.max(-PHYS.maxFall, Math.min(PHYS.maxFall, e.vy + PHYS.gravity * p.gravityDir * dt));
        e.spin += e.vx * dt * 0.05;
        const hit = moveBox(e, dt, p.gravityDir);
        if (hit.landed) { e.resting = true; e.vx = 0; e.spin = 0; }
      }
      if (e.y > H + 100 || e.y < -100) { state.egg = null; return; }   // fell out of the world
      if (e.age > EGG_LIFETIME) {
        // Hatched on its own. You missed your chance.
        puff(e.x + e.w / 2, e.y + e.h / 2, '#fff8e1', 12, 140);
        floatText(e.x + e.w / 2, e.y - 8, 'peep!', '#ffd166');
        state.egg = null;
      }
    },
    onRespawn() { state.egg = null; },
    drawWorld(ctx) {
      const e = state.egg;
      if (!e) return;
      // Wobble faster as it gets close to hatching.
      const warn = e.age > EGG_LIFETIME - 1.2 ? Math.sin(e.age * 40) * 0.25 : 0;
      ctx.save();
      ctx.translate(e.x + e.w / 2, e.y + e.h / 2);
      ctx.rotate(e.spin + warn);
      ctx.fillStyle = '#fffaf0';
      ctx.strokeStyle = '#c9b38f';
      ctx.lineWidth = 2;
      ctx.beginPath(); ctx.ellipse(0, 0, e.w / 2, e.h / 2, 0, 0, Math.PI * 2); ctx.fill(); ctx.stroke();
      ctx.fillStyle = '#9ee6dc';
      ctx.fillRect(-3, -4, 3, 3); ctx.fillRect(3, 3, 3, 3);   // little blue speckles
      ctx.restore();
      // Dotted line from chicken to egg so you can see where you'll land.
      ctx.save();
      ctx.setLineDash([4, 6]);
      ctx.strokeStyle = 'rgba(120,90,200,.55)';
      ctx.beginPath();
      ctx.moveTo(player.x + player.w / 2, player.y + player.h / 2);
      ctx.lineTo(e.x + e.w / 2, e.y + e.h / 2);
      ctx.stroke();
      ctx.restore();
    },
    hud() {
      const e = state.egg;
      const status = !e ? 'Egg: in hand (press Q)' : `Egg: out! Q to teleport (hatches in ${(EGG_LIFETIME - e.age).toFixed(1)}s)`;
      return `${status}   Teleports: ${state.teleports}`;
    },
  });

  window.eggState = state;
  Game.start();
})();
