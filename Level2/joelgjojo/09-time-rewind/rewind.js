// Mechanic: Time rewind.
// The game keeps the last 4 seconds of the chicken's movement.
// Hold T to rewind time: the chicken flies backwards along its own path, VHS style.
// Let go and play resumes from that moment.
(() => {
  const { player, input, meta, W, H } = Game;
  meta.title = 'Level 2 - Time Rewind';
  meta.controls = 'A/D or arrows: move   Space/W: jump   HOLD T: rewind time   R: reset';

  const SECONDS = 4;
  const MAX_FRAMES = SECONDS * 60;  // one snapshot per frame at 60 fps
  const history = [];
  const state = { rewinding: false, rewoundFrames: 0 };

  const snapshot = p => ({ x: p.x, y: p.y, vx: p.vx, vy: p.vy, facing: p.facing, gravityDir: p.gravityDir, onGround: p.onGround });

  Game.addMechanic({
    update(p) {
      state.rewinding = input.down('KeyT') && history.length > 1;
      if (state.rewinding) { p.lockControls = true; p.ignoreGravity = true; }
    },
    afterPhysics(p) {
      if (state.rewinding) {
        // Step back one frame. This overrides whatever the physics just did.
        const s = history.pop();
        Object.assign(p, s);
        state.rewoundFrames++;
      } else {
        history.push(snapshot(p));
        if (history.length > MAX_FRAMES) history.shift();
      }
    },
    onRespawn() { history.length = 0; },
    drawBehind(ctx) {
      // The remembered path, fading out the older it gets.
      if (history.length < 2) return;
      ctx.save();
      ctx.lineWidth = 3;
      for (let i = 1; i < history.length; i += 2) {
        const a = history[i - 1], b = history[i];
        ctx.strokeStyle = `rgba(160,110,255,${(i / history.length) * 0.5})`;
        ctx.beginPath();
        ctx.moveTo(a.x + player.w / 2, a.y + player.h / 2);
        ctx.lineTo(b.x + player.w / 2, b.y + player.h / 2);
        ctx.stroke();
      }
      ctx.restore();
    },
    drawFront(ctx) {
      if (!state.rewinding) return;
      // VHS look: purple wash, scanlines, a rewind symbol and a wobbling tracking bar.
      ctx.fillStyle = 'rgba(70,30,120,.25)'; ctx.fillRect(0, 0, W, H);
      ctx.fillStyle = 'rgba(0,0,0,.12)';
      for (let y = 0; y < H; y += 4) ctx.fillRect(0, y, W, 1);
      const barY = (performance.now() / 4) % H;
      ctx.fillStyle = 'rgba(255,255,255,.08)'; ctx.fillRect(0, barY, W, 18);
      ctx.fillStyle = '#fff';
      ctx.font = '800 26px monospace';
      ctx.fillText('◀◀ REWIND', W - 210, 60);
    },
    hud() {
      const secs = (history.length / 60).toFixed(1);
      return `Rewind buffer: ${secs}s / ${SECONDS}s   Frames rewound: ${state.rewoundFrames}`;
    },
  });

  window.rewindState = state;
  window.rewindHistory = history;
  Game.start();
})();
