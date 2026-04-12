# MonoGame Starter Skeleton

This project is a small top-down action game prototype built in MonoGame with one strong architectural rule:
keep MonoGame types at the edges and keep gameplay rules in plain C# wherever practical.

The project is no longer just a bare skeleton. It now has a playable overworld structure, an arena encounter prototype, multiple enemy archetypes, and the first pass of interior, inventory-shell, and boss-fight plumbing.

## Current Snapshot

The game currently includes:

- main menu, gameplay, pause menu, save/load flow, and return-to-menu flow
- a town hub plus north/south/east/west wilderness scenes connected by transitions
- dedicated shop and arena interior scenes connected by transitions
- a reusable `World` simulation model that owns gameplay state
- player movement, facing, health, death, dash, melee attack, ranged attack, and shield ability
- pause-menu inventory shell with tabbed categories for weapons, armor, items, and abilities
- multiple enemy types:
  - crab
  - horned rabbit
  - horned rabbit elite
  - horned rabbit boss
  - bat
  - bat miniboss
  - grasshopper
- multi-wave arena encounters with top-center wave banners and configurable between-wave full heal
- enemy contact damage, separation, obstacle collision, defeat tracking, and ability-point rewards
- replay and recording diagnostics
- animated shopkeeper talk indicator, proximity prompt, and a first-pass buy/sell dialogue modal
- JSON-backed configuration for player, enemies, diagnostics, and world combat tuning
- fast unit tests around gameplay rules, rendering layout helpers, input, saves, and scene behavior

## Controls

- move: `WASD`, arrow keys, left stick, or D-pad
- interact: `E` or `B`
- attack: `J`, `Left Ctrl`, or `X`
- ranged attack: `K`, `Left Alt`, or `Right Trigger`
- defense ability: `L` or `Y`
- dash: `Shift` or `Right Shoulder`
- switch shop tabs: `Q` / `R` or `Left Shoulder` / `Right Shoulder`
- switch inventory tabs: `Q` / `R` or `Left Shoulder` / `Right Shoulder`
- confirm: `Enter`, `Space`, or `A`
- cancel/back: `Esc`, `B`, or `Back`
- pause: `Esc`, `P`, or `Start`

`Load Game` stays disabled until a save exists.

## What You Can Do Right Now

- start a run from the main menu
- explore the town hub and wilderness scenes
- fight several enemy types with melee, dash, ranged attacks, and shield play
- enter the arena and test the current multi-wave encounter flow
- fight a staged horned rabbit boss and the bat miniboss encounter set
- enter the shop through the overworld door
- approach the counter to open the current buy/sell dialogue shell
- open the inventory shell from the pause menu
- save from the pause menu and load the latest save
- record and replay runs through the diagnostics menu

## Project Shape

- `MyGame`
  - MonoGame host, scenes, rendering, gameplay, input, diagnostics, and save/load code
- `MyGame.Tests`
  - fast tests for gameplay rules, transitions, config, input, save/load, and layout behavior
- `docs`
  - design notes and longer-term direction
- `TODO.md`
  - near-term implementation chunks and current follow-up work

## Architecture Notes

- `GameRoot` wires up services and scene flow.
- `SceneManager` owns the active scene and handles scene changes.
- `GameplayScene` wraps a `World` and handles pause flow, rendering, inventory UI state, and transition callbacks.
- `World` owns simulation state such as player, enemies, props, projectiles, toasts, banners, and scene transitions.
- gameplay interaction rules live in focused classes like:
  - `PlayerAttackHitResolver`
  - `PlayerProjectileResolver`
  - `WorldObstacleResolver`
  - `EnemyContactResolver`
  - `EnemySeparationResolver`
- `ArenaEncounterController` manages arena wave sequencing, banners, and inter-wave recovery
- `GameplayLevelBuilder` currently assembles the town hub, wilderness scenes, shop interior, and arena layout
- input is normalized through `IInputService` and `GameAction`
- gameplay tuning is loaded from JSON config objects instead of being buried in runtime code
- save/load uses DTOs rather than serializing live game objects directly

This is still an active prototype, so some systems are intentionally lightweight while the overall game spine is being discovered.

## Arena And Interior Snapshot

The arena and shop are the current test beds for a few future-facing systems:

- entering sub-structures through a scene transition
- preserving important player state across room changes
- NPC interaction prompts and dialogue shell UI
- buy/sell tabs that will later connect to wares and inventory
- encounter locking and unlock conditions inside interior spaces
- staged boss fights and encounter sequencing
- wave banners and inter-wave pacing rules

The current implementation is intentionally modest, but it is meant to establish patterns we can later reuse for caves, dungeons, castles, and other interiors.

## Save And Replay Notes

Current save/load support restores the active gameplay scene and core world state, including:

- scene name
- player position
- player health
- player ability points
- enemy positions
- enemy health / dead state
- defeated enemy count

The replay menu is currently available through the pause menu when diagnostics are enabled.

## Running The Project

Build the game project with:

```powershell
dotnet build MyGame\MyGame.csproj --no-restore
```

Run tests with:

```powershell
dotnet test MyGame.Tests\MyGame.Tests.csproj --no-restore
```

That test command is the recommended fast path for this repo.

## Direction

Near-term work is focused on:

1. continuing to improve combat feel and encounter pressure
2. turning the shop and inventory shells into real progression systems
3. adding the first NPC interaction and town-life layer to the hub
4. building dungeon entrance and floor plumbing off the current overworld/arena structure
5. growing the gameplay spine gradually instead of overbuilding too early

See `docs/GameDesign.md` for broader design direction, and `TODO.md` for the current working list of follow-ups.
