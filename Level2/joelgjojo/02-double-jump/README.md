# Level 2 - Double Jump (joelgjojo)

One new mechanic for the blue chicken, built with plain HTML5 canvas and JavaScript (no engine, nothing to install).

## How to run
Open `index.html` in any modern browser. Click the game once if the keys don't respond.

## Controls
| Key | Action |
|---|---|
| A/D or arrow keys | Move |
| Space / W / Up | Jump |
| **Space / W / Up (in mid-air)** | **Double Jump** |
| R | Reset the chicken |

## The mechanic
- Press jump again while airborne for a second, slightly weaker flap
- Still works after walking off a ledge
- The chicken does a full spin and sheds a burst of feathers
- Resets every time you land

## Files
- `core.js` - a small platformer base: input, gravity, platform collisions, particles, HUD
- `double-jump.js` - the double jump mechanic, plugged into the base with `Game.addMechanic()`
- `blue_chicken.png` - sprite from the repo's `Resources` folder
