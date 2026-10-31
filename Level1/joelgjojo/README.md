# Level 1 - Chicken Movement (joelgjojo)

A top-down 2D player controller for the blue chicken, built with plain HTML5 canvas and JavaScript (no engine, nothing to install).

## How to run
Open `index.html` in any modern browser. Click the game once if the keys don't respond.

## Controls
| Key | Action |
|---|---|
| WASD / arrow keys | Move |
| Shift (hold) | Waddle faster |

## What's in it
- 8-direction movement, with diagonals normalised so they aren't faster than straight lines
- Smooth acceleration and stopping (velocity eases toward the target)
- The chicken stays inside the screen
- Waddle animation: a small hop and tilt while walking, with a shadow on the ground

Sprite: `blue_chicken.png` from the repo's `Resources` folder (trimmed and scaled down).
