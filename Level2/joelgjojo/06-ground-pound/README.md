# Level 2 - Ground Pound (joelgjojo)

One new mechanic for the blue chicken, built with plain HTML5 canvas and JavaScript (no engine, nothing to install).

## How to run
Open `index.html` in any modern browser. Click the game once if the keys don't respond.

## Controls
| Key | Action |
|---|---|
| A/D or arrow keys | Move |
| Space / W / Up | Jump |
| **S / Down (in mid-air)** | **Ground Pound** |
| R | Reset the chicken |

## The mechanic
- In mid-air, press S (or Down): the chicken freezes and spins for a split second, then slams straight down
- Landing sends out a shockwave ring and shakes the screen, with a short stun afterwards
- Wooden crates sit on the platforms. You can stand on them, but a ground pound smashes them and you keep falling
- The HUD counts slams and crates smashed, and R puts the crates back

## Files
- `core.js` - a small platformer base: input, gravity, platform collisions, particles, HUD (same as my other Level 2 mechanics)
- `ground-pound.js` - the ground pound mechanic, plugged into the base with `Game.addMechanic()`
- `blue_chicken.png` - sprite from the repo's `Resources` folder
