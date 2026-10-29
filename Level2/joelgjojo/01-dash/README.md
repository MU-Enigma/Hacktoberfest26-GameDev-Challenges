# Level 2 - Dash (joelgjojo)

One new mechanic for the blue chicken, built with plain HTML5 canvas and JavaScript (no engine, nothing to install).

## How to run
Open `index.html` in any modern browser. Click the game once if the keys don't respond.

## Controls
| Key | Action |
|---|---|
| A/D or arrow keys | Move |
| Space / W / Up | Jump |
| **Shift or K** | **Dash** |
| R | Reset the chicken |

## The mechanic
- Press Shift (or K) to burst forward at 3x running speed for a split second
- Dashes go perfectly straight: gravity is paused for the duration
- Dash toward the direction you're holding, or the way you're facing
- One dash in the air until you land again, plus a short cooldown
- Leaves a trail of fading blue ghost chickens

## Files
- `core.js` - a small platformer base: input, gravity, platform collisions, particles, HUD
- `dash.js` - the dash mechanic, plugged into the base with `Game.addMechanic()`
- `blue_chicken.png` - sprite from the repo's `Resources` folder
