// Mechanic: Burger time.
// Press E to summon a burger that drops from the sky.
// Walk into a burger to eat it: the chicken gets bigger, slower and heavier.
// Eat one too many and the chicken bursts into feathers and pops back to normal size.
(() => {
  const { player, input, moveBox, overlaps, setScale, puff, floatText, PHYS, meta, W } = Game;
  meta.title = 'Level 2 - Burger Time';
  meta.controls = 'A/D or arrows: move   Space/W: jump   E: summon burger   R: reset';

  const MAX_BURGERS_ON_SCREEN = 12;
  const GROWTH_PER_BURGER = 0.22;
  const BURST_AT = 5;            // the 5th burger is one too many

  const burgers = [];
  const state = { eaten: 0, totalEaten: 0, bursts: 0 };

  function applySize() {
    setScale(1 + state.eaten * GROWTH_PER_BURGER);
    // A bigger chicken is a slower, heavier chicken.
    player.speedMul = 1 - state.eaten * 0.1;
    player.jumpMul = 1 - state.eaten * 0.07;
  }

  function spawnBurger() {
    if (burgers.length >= MAX_BURGERS_ON_SCREEN) burgers.shift();
    // Pop it out just ahead of the chicken, a little above the floor it's standing on,
    // so it lands within reach instead of on some platform overhead.
    const b = { x: 0, y: 0, w: 34, h: 26, vx: 0, vy: -260, wobble: Math.random() * 6, age: 0 };
    b.x = Math.max(20, Math.min(W - 54, player.x + player.w / 2 + player.facing * 90 - b.w / 2));
    b.y = player.y + player.h - b.h - 40;
    // Don't spawn inside a platform.
    for (let i = 0; i < 40 && Game.platforms.some(s => overlaps(b, s)); i++) b.y -= 4;
    burgers.push(b);
    const { x, y } = b;
    floatText(x + 17, y - 6, 'order up!', '#ffd166');
  }

  Game.addMechanic({
    update(p, dt) {
      if (input.pressed('KeyE')) spawnBurger();

      for (let i = burgers.length - 1; i >= 0; i--) {
        const b = burgers[i];
        b.age += dt;
        b.vy = Math.min(b.vy + PHYS.gravity * 0.6 * dt, 700);
        const hit = moveBox(b, dt);
        if (hit.bottom) b.vx *= 0.9;   // a bit of friction once it lands

        if (b.age > 0.15 && overlaps(b, p)) {
          burgers.splice(i, 1);
          state.eaten++;
          state.totalEaten++;
          if (state.eaten >= BURST_AT) {
            // Too much burger. Kaboom.
            puff(p.x + p.w / 2, p.y + p.h / 2, '#ffffff', 40, 380);
            puff(p.x + p.w / 2, p.y + p.h / 2, '#9ee6dc', 25, 300);
            floatText(p.x + p.w / 2, p.y - 10, 'TOO MUCH BURGER', '#ff6b6b');
            state.eaten = 0;
            state.bursts++;
          } else {
            puff(b.x + b.w / 2, b.y + b.h / 2, '#f4a259', 8, 120);
            floatText(p.x + p.w / 2, p.y - 10, 'nom', '#ffd166');
          }
          applySize();
        }
      }
    },
    onRespawn() { state.eaten = 0; burgers.length = 0; applySize(); },
    drawWorld(ctx) {
      for (const b of burgers) drawBurger(ctx, b);
    },
    hud() {
      const meter = '#'.repeat(state.eaten) + '.'.repeat(BURST_AT - 1 - state.eaten);
      return `Fullness: [${meter}]   Burgers eaten: ${state.totalEaten}   Bursts: ${state.bursts}`;
    },
  });

  // A burger drawn from simple shapes: bottom bun, patty, cheese, lettuce, top bun, seeds.
  function drawBurger(ctx, b) {
    const { x, y, w, h } = b;
    ctx.save();
    ctx.translate(x + w / 2, y + h / 2);
    ctx.rotate(Math.sin(b.age * 4 + b.wobble) * 0.06);
    ctx.translate(-w / 2, -h / 2);
    const round = (rx, ry, rw, rh, r, color) => {
      ctx.fillStyle = color; ctx.beginPath(); ctx.roundRect(rx, ry, rw, rh, r); ctx.fill();
    };
    round(2, h - 7, w - 4, 7, 3, '#e0a458');        // bottom bun
    round(1, h - 13, w - 2, 7, 3, '#6b3e26');       // patty
    ctx.fillStyle = '#ffd23f';                       // melty cheese
    ctx.beginPath(); ctx.moveTo(2, h - 14); ctx.lineTo(w - 2, h - 14); ctx.lineTo(w - 8, h - 9); ctx.lineTo(w / 2, h - 12); ctx.lineTo(8, h - 9); ctx.closePath(); ctx.fill();
    round(0, h - 17, w, 4, 2, '#6ccf4f');           // lettuce
    ctx.fillStyle = '#e8a64f';                       // top bun
    ctx.beginPath(); ctx.ellipse(w / 2, h - 17, w / 2, 11, 0, Math.PI, 0); ctx.fill();
    ctx.fillStyle = '#fff6d8';                       // sesame seeds
    for (const [sx, sy] of [[10, 9], [17, 6], [24, 9], [14, 12], [21, 11]]) ctx.fillRect(sx, sy, 2, 3);
    ctx.restore();
  }

  window.burgerState = state;
  window.burgers = burgers;
  Game.start();
})();
