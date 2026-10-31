# Level 2 - Burger Time (joelgjojo)

One new mechanic for the blue chicken, built with plain HTML5 canvas and JavaScript (no engine, nothing to install).

## How to run
Open `index.html` in any modern browser. Click the game once if the keys don't respond.

## Controls
| Key | Action |
|---|---|
| A/D or arrow keys | Move |
| Space / W / Up | Jump |
| **E** | **Burger Time** |
| R | Reset the chicken |

## The mechanic
- Press E to summon a burger that pops out just ahead of the chicken
- Burgers fall with physics and land on platforms
- Walk into a burger to eat it: the chicken gets bigger, slower and jumps lower
- The 5th burger is one too many: the chicken bursts into feathers and pops back to normal size
- A fullness meter and burger counter in the HUD

## Files
- `core.js` - a small platformer base: input, gravity, platform collisions, particles, HUD
- `burger.js` - the burger time mechanic, plugged into the base with `Game.addMechanic()`
- `blue_chicken.png` - sprite from the repo's `Resources` folder
