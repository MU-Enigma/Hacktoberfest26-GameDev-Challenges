# Level 2 - Time Rewind (joelgjojo)

One new mechanic for the blue chicken, built with plain HTML5 canvas and JavaScript (no engine, nothing to install).

## How to run
Open `index.html` in any modern browser. Click the game once if the keys don't respond.

## Controls
| Key | Action |
|---|---|
| A/D or arrow keys | Move |
| Space / W / Up | Jump |
| **Hold T** | **Time Rewind** |
| R | Reset the chicken |

## The mechanic
- The game remembers the last 4 seconds of the chicken's movement, drawn as a fading purple trail
- Hold T and the chicken plays back in reverse, one frame at a time
- Let go and play continues from that moment, so you can undo a bad jump
- VHS rewind effect while rewinding: scanlines, a purple wash and a moving tracking bar

## Files
- `core.js` - a small platformer base: input, gravity, platform collisions, particles, HUD (same as my other Level 2 mechanics)
- `rewind.js` - the time rewind mechanic, plugged into the base with `Game.addMechanic()`
- `blue_chicken.png` - sprite from the repo's `Resources` folder
