# Level 2 - Gravity Flip (joelgjojo)

One new mechanic for the blue chicken, built with plain HTML5 canvas and JavaScript (no engine, nothing to install).

## How to run
Open `index.html` in any modern browser. Click the game once if the keys don't respond.

## Controls
| Key | Action |
|---|---|
| A/D or arrow keys | Move |
| Space / W / Up | Jump |
| **G** | **Gravity Flip** |
| R | Reset the chicken |

## The mechanic
- Press G to flip gravity: the chicken falls up and walks on the ceiling upside down
- Jumping pushes you away from whichever surface you're standing on
- A half-spin animation instead of an instant snap
- The sky fades from day to dusk while gravity is flipped
- An arrow in the corner shows which way gravity is pulling

## Files
- `core.js` - a small platformer base: input, gravity, platform collisions, particles, HUD
- `gravity-flip.js` - the gravity flip mechanic, plugged into the base with `Game.addMechanic()`
- `blue_chicken.png` - sprite from the repo's `Resources` folder
