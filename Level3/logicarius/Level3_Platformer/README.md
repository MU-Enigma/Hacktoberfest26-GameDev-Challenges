# Chicken Climb (Level 3, platformer) - logicarius

Unity 6000.3.26f1 (Universal 2D). Open `Level3_Platformer` in Unity Hub, open `Assets/Scenes/Level1` and press Play.
The whole level is built by script when the game starts, so the scene itself stays small.

## Goal
Climb six floors of a walled, candy-coloured arena and touch the **golden egg** at the top. Lose all 3 hearts and it is game over. Both endings go back to the start screen.

## Controls
- Move: A / D or Left / Right
- Jump: W or Up
- Super jump: Space
- Fast fall (in the air): S or Down
- Enter: Play / skip the end screen. Esc: back to the start screen

## How it works
- **Arena**: sealed by walls and a ceiling. Floors are full width and the stairs between floors come up through one-way trapdoors, so there are no cliffs to fall off. Step sizes are computed from the chicken's real jump height, so every climb is possible.
- **Hazards**: fire rows (up to 4 on the upper floors), fire hidden under bridges, and flames that slide back and forth. Fire only burns on a real touch.
- **Death**: the chicken turns into a roasted drumstick, loses a heart and all its rocks, and comes back at the last egg with 1 second of protection. The lost rocks reappear ahead of you.
- **Eggs**: every 15 seconds alive the chicken lays an egg. The latest egg is the checkpoint. An egg that lands on fire slides to a safe spot first.
- **Rocks and worms**: carry up to 3 rocks. Worms only give points while you hold 3 rocks. Both are placed at random on every run.
- **Hearts**: 3 extra heart pickups on the floating platforms restore a heart.
- **Look**: every floor has its own colours and the sky fades from sunset to night as you climb.

## Scripts (`Assets/`)
ArenaBuilder, PlayerMovement, CameraFollow, PlayerLife, PlayerHealth, PlayerStats, EggLayer, Egg, Hazard, Patrol, Collectible, Goal, TitleScreen.