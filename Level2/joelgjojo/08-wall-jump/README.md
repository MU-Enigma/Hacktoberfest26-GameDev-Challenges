# Level 2 - Wall Jump (joelgjojo)

One new mechanic for the blue chicken, built with plain HTML5 canvas and JavaScript (no engine, nothing to install).

## How to run
Open `index.html` in any modern browser. Click the game once if the keys don't respond.

## Controls
| Key | Action |
|---|---|
| A/D or arrow keys | Move |
| Space / W / Up | Jump |
| **Space / W / Up (against a wall)** | **Wall Jump** |
| R | Reset the chicken |

## The mechanic
- Push into a wall while falling and the chicken slides down slowly, scraping dust
- Press jump while touching a wall to kick off it, away from the wall
- Steering is locked for a split second after a kick, so you actually leave the wall
- A tall shaft in the corner: walk in through the gap at the bottom and wall-jump back and forth to reach the star at the top

## Files
- `core.js` - a small platformer base: input, gravity, platform collisions, particles, HUD (same as my other Level 2 mechanics)
- `wall-jump.js` - the wall jump mechanic, plugged into the base with `Game.addMechanic()`
- `blue_chicken.png` - sprite from the repo's `Resources` folder
