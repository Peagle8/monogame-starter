# MonoGame Starter Skeleton

This project is a small 2D MonoGame game built with a simple rule:
keep MonoGame at the edges and keep gameplay logic in plain C# where possible.

## Current State

The game currently includes:

- scene flow with a main menu, gameplay scene, and pause menu
- a `World` object that owns gameplay state instead of the scene doing everything
- a controllable player with movement, facing, health, death, and restart flow
- a first enemy type: a crab with chase, recovery, hit flash, defeat linger, and simple combat
- player attack input and hit resolution
- JSON-backed tuning for player movement and enemy stats
- save/load through the menus
- save data that currently restores:
  - scene name
  - player position
  - player health
  - enemy positions
  - enemy health/dead state
  - defeated enemy count
- a debug overlay and basic recording support
- fast unit tests for gameplay and infrastructure logic

## Controls

- move: `WASD` or arrow keys
- attack: `J` or `Left Ctrl`
- confirm: `Enter`
- cancel/back: `Esc`
- pause: `Esc` or `P`

`Load Game` is disabled in menus until a save file exists.

## Project Shape

- `MyGame`
  - MonoGame host, scenes, rendering, input, diagnostics, save/load, and gameplay code
- `MyGame.Tests`
  - fast tests for gameplay rules, state transitions, save/load, and layout logic

## Architecture Notes

- `GameRoot` stays intentionally small and mostly wires services and scenes together.
- `SceneManager` handles scene changes and forwards update/draw calls.
- `World` owns active gameplay simulation state.
- `IInputService` maps hardware input to `GameAction`.
- gameplay tuning is loaded from JSON config objects rather than hidden magic numbers
- save/load uses explicit DTOs instead of serializing live runtime objects directly

## What Is Working

- launch into the main menu
- start a gameplay run
- move the player around the world
- fight a crab enemy
- die and restart
- pause and access save/load/controls
- load from the main menu after a save exists

## What Still Needs Work

Near-term priorities:

1. add multiple save slots
2. improve save UX further now that single-save flow works
3. continue improving presentation and art beyond the current placeholder visuals
4. expand enemy variety and world content beyond the single crab test loop
5. keep extracting clean gameplay services where `World` is still doing too much directly

Likely future systems:

1. additional enemies and combat behaviors
2. more room/world content
3. HUD polish beyond the current functional pass
4. stronger graphics and animation pass
5. replay input / deterministic debugging improvements

## Running Tests

Use:

```powershell
dotnet test MyGame.Tests\MyGame.Tests.csproj --no-restore
```

That is the recommended fast path for this repo.
