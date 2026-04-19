# MonoGame Starter Skeleton

This repo is a top-down action RPG prototype built in MonoGame with one strong architectural rule:
keep MonoGame types at the edges and keep gameplay rules in plain C# wherever practical.

It is no longer just a bare skeleton. The project now has a playable town-and-wilderness world, an arena encounter ladder, a shop interaction shell, a pause-map, an ability/loadout shell, save/load, replay diagnostics, and a growing enemy roster.

## Current Snapshot

The game currently includes:

- main menu, gameplay, pause menu, save/load flow, and return-to-menu flow
- a town hub plus north, south, east, and west wilderness scenes connected by transitions
- a dedicated arena interior and a dedicated shop interior connected from the town hub
- three shop exteriors in the town center, with one functional interior currently wired up
- a dungeon exterior/entrance prop in town that currently serves as world-shape scaffolding rather than a live dungeon
- a reusable `World` simulation model that owns player, enemies, props, combat resolution, scene transitions, banners, toasts, and objective state
- player movement, contact recoil, death, melee attack, ranged attack, defense ability, dash, and bomb-dash bomb trail behavior
- health and ability-point resource systems, including passive AP regeneration
- pause-menu inventory tabs for weapons, armor, items, and abilities/loadout
- a pause-map panel for overworld scenes
- an ability/loadout shell with slot-based equip flow for dash, defense, ranged, and melee abilities
- save/load support for unlocked abilities and currently equipped loadout choices
- enemy roster:
  - crab
  - horned rabbit
  - horned rabbit elite
  - horned rabbit boss
  - bat
  - bat miniboss
  - grasshopper
- always-visible world-space enemy health bars, with larger boss bars
- doubled enemy health totals across the current roster for longer encounter reads
- four arena waves with banners, inter-wave pacing, and staged boss/miniboss pressure
- enemy contact damage, separation, obstacle collision, defeat tracking, and ability-point rewards
- replay and recording diagnostics
- animated shopkeeper talk indicator, proximity prompt, and a first-pass buy/sell dialogue modal
- JSON-backed configuration for player, enemies, diagnostics, and world combat tuning
- fast unit tests around gameplay rules, rendering/layout helpers, input, saves, and scene behavior

## Controls

- move: `WASD`, arrow keys, left stick, or D-pad
- interact: `E` or `B`
- attack: `J`, `Left Ctrl`, or `X`
- ranged attack: `K`, `Left Alt`, or `Right Trigger`
- defense ability: `L`, `Right Ctrl`, or `Y`
- dash: `Left Shift`, `Right Shift`, or `Right Shoulder`
- map: `Tab`, `M`, or `Back`
- switch shop tabs: `Q` / `R` or `Left Shoulder` / `Right Shoulder`
- switch inventory tabs: `Q` / `R` or `Left Shoulder` / `Right Shoulder`
- confirm: `Enter`, `Space`, or `A`
- cancel/back: `Esc`, `Backspace`, `B`, or `Back`
- pause: `Esc`, `P`, or `Start`

`Load Game` stays disabled until a save exists.

## What You Can Do Right Now

- start a run from the main menu
- explore the town hub and wilderness scenes
- open the overworld map from supported scenes
- fight the current enemy roster with melee, ranged, defense, and dash-based movement
- enter the arena and play through the current four-wave encounter set
- fight the horned rabbit boss and the bat miniboss encounter set
- enter the first shop interior through the town hub
- approach the counter to open the current buy/sell dialogue shell
- open the pause-menu inventory and inspect the ability/loadout tab
- equip implemented loadout options such as base dash, bomb dash, base shield, and fireball
- save from the pause menu and load the latest save
- record and replay runs through the diagnostics menu

## Project Shape

- `MyGame`
  MonoGame host, scenes, rendering, gameplay, input, diagnostics, and save/load code.
- `MyGame.Tests`
  Fast tests for gameplay rules, transitions, config, input, save/load, and layout behavior.
- `docs`
  Design notes and longer-term direction.
- `TODO.md`
  Near-term implementation chunks and current follow-up work.

## Architecture Notes

- `GameRoot` wires up services, scene creation, and game boot flow.
- `SceneManager` owns the active scene and handles scene changes.
- `GameplayScene` wraps a `World` and handles pause flow, map/inventory/shop UI state, and transition callbacks.
- `World` owns simulation state such as player, enemies, props, projectiles, bombs, toasts, banners, and scene transitions.
- focused gameplay interaction rules live in classes like:
  - `PlayerAttackHitResolver`
  - `PlayerBombResolver`
  - `PlayerProjectileResolver`
  - `WorldObstacleResolver`
  - `EnemyContactResolver`
  - `EnemySeparationResolver`
- `ArenaEncounterController` manages wave sequencing, banners, inter-wave recovery, and objective completion in the arena.
- `GameplayLevelBuilder` assembles the town hub, wilderness scenes, shop interior, and arena layout.
- input is normalized through `IInputService` and `GameAction`.
- gameplay tuning is loaded from JSON config objects instead of being buried in runtime code.
- save/load uses DTOs rather than serializing live game objects directly.

This is still an active prototype, so some systems are intentionally lightweight while the broader game spine is still being discovered.

## Arena, Shop, And Loadout Snapshot

The arena, shop, and pause-menu loadout flow are the current test beds for future-facing systems:

- entering sub-areas through scene transitions
- preserving important player state across room changes
- boss/miniboss encounter sequencing
- encounter locking and unlock conditions inside interior spaces
- wave banners and inter-wave pacing rules
- NPC interaction prompts and dialogue-shell UI
- buy/sell tabs that will later connect to wares, currency, and inventory
- a slot-based ability/loadout presentation that can later connect to real progression and upgrades

The current implementations are intentionally modest, but they establish patterns we can later reuse for dungeons, caves, castles, NPC interiors, and fuller progression systems.

## Save And Replay Notes

Current save/load support restores the active gameplay scene and core world state, including:

- scene name
- player position
- player health
- player ability points
- unlocked player abilities
- equipped dash, defense, ranged, and melee abilities
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

1. turning the inventory, shop, and loadout shells into real progression systems
2. adding the first reusable NPC interaction and dialogue layer to the town hub
3. building dungeon entrance and floor plumbing off the current overworld/arena structure
4. adding arena rewards and encounter selection that tie back into progression
5. continuing to tune combat readability, balance, and encounter pressure

See `docs/GameDesign.md` for broader design direction, and `TODO.md` for the current working list of follow-ups.
