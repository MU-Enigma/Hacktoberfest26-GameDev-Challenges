# Level 3 - Chicken Run (joelgjojo)

A three-stage platformer for the blue chicken, built with plain HTML5 canvas and JavaScript (no engine, nothing to install). It uses two of my Level 2 mechanics, **double jump** and **dash**.

## How to run
Open `index.html` in any modern browser. Click the game once if the keys don't respond.

## Controls
| Key | Action |
|---|---|
| A/D or arrow keys | Move |
| Space / W / Up | Jump (press again in the air to flap) |
| Shift / K | Dash (once per jump in the air) |
| R | Restart the current level |

## The levels
1. **Hop To It** - learn to jump and flap over spike pits up to a walled-in flag
2. **Mind The Gap** - spike gaps too wide to jump, so you have to dash across, plus a spring
3. **The Coop Tower** - climb a tower of ledges over a floor of spikes, using springs, flaps and dashes

Each level has corn to collect. Touching spikes or falling sends you back to the start of the level. Reaching the flag moves you to the next level, and finishing all three shows your time, deaths and corn.

## How it's built
- Levels are ASCII maps in `levels.js` (`#` ground, `^` spikes, `o` corn, `S` spring, `F` flag, `P` start), so new levels are easy to add
- `game.js` keeps the player physics in one pure function, `stepPlayer()`, separate from drawing and input
- Because of that, I checked every level is beatable with a small breadth-first search over inputs that runs on the same physics. It also caught a bug: you could jump above the screen and skip a level, so there's now an invisible ceiling
