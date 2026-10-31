# Level 2 - Glide (joelgjojo)

One new mechanic for the blue chicken, built with plain HTML5 canvas and JavaScript (no engine, nothing to install).

## How to run
Open `index.html` in any modern browser. Click the game once if the keys don't respond.

## Controls
| Key | Action |
|---|---|
| A/D or arrow keys | Move |
| Space / W / Up | Jump |
| **Hold Space / W / Up (while falling)** | **Glide** |
| R | Reset the chicken |

## The mechanic
- Hold jump while falling and the chicken spreads its wings, dropping slowly instead of plummeting
- You steer a little faster while gliding
- Two wind gusts (the streaky bands) push a gliding chicken left or right
- Flapping wing animation and a trail of feathers, plus a glide-time counter in the HUD

## Files
- `core.js` - a small platformer base: input, gravity, platform collisions, particles, HUD (same as my other Level 2 mechanics)
- `glide.js` - the glide mechanic, plugged into the base with `Game.addMechanic()`
- `blue_chicken.png` - sprite from the repo's `Resources` folder
