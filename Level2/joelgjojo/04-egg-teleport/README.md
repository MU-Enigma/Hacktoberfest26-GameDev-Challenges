# Level 2 - Egg Teleport (joelgjojo)

One new mechanic for the blue chicken, built with plain HTML5 canvas and JavaScript (no engine, nothing to install).

## How to run
Open `index.html` in any modern browser. Click the game once if the keys don't respond.

## Controls
| Key | Action |
|---|---|
| A/D or arrow keys | Move |
| Space / W / Up | Jump |
| **Q** | **Egg Teleport** |
| R | Reset the chicken |

## The mechanic
- Press Q to lob an egg in an arc; it lands and rests on whatever it hits
- Press Q again to teleport to the egg, cracking it open
- A dotted line shows where you'll land
- Wait too long (4 s) and the egg wobbles, then hatches on its own
- Safe landing: if the target spot overlaps a platform, the chicken is nudged clear

## Files
- `core.js` - a small platformer base: input, gravity, platform collisions, particles, HUD
- `egg-teleport.js` - the egg teleport mechanic, plugged into the base with `Game.addMechanic()`
- `blue_chicken.png` - sprite from the repo's `Resources` folder
