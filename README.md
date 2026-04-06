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
- a debug overlay plus replay/record diagnostics support
- fast unit tests for gameplay and infrastructure logic

## Controls

- move: `WASD`, arrow keys, left stick, or D-pad
- attack: `J`, `Left Ctrl`, or `X`
- dash: `Shift` or `Right Shoulder`
- confirm: `Enter`, `Space`, or `A`
- cancel/back: `Esc`, `B`, or `Back`
- pause: `Esc`, `P`, or `Start`

`Load Game` is disabled in menus until a save file exists.

## Project Shape

- `MyGame`
  - MonoGame host, scenes, rendering, input, diagnostics, save/load, and gameplay code
- `MyGame.Tests`
  - fast tests for gameplay rules, state transitions, save/load, and layout logic
- `docs`
  - design notes and longer-term direction

## Architecture Notes

- `GameRoot` stays intentionally small and mostly wires services and scenes together.
- `SceneManager` handles scene changes and forwards update/draw calls.
- `World` owns active gameplay simulation state.
- `IInputService` maps hardware input to `GameAction`.
- `IPlayerAbilityService` gates unlockable actions like dash without pushing progression rules into input code.
- gameplay tuning is loaded from JSON config objects rather than hidden magic numbers
- save/load uses explicit DTOs instead of serializing live runtime objects directly

See `docs/GameDesign.md` for the current higher-level design direction.

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
3. add controller support so combat iteration is comfortable and easier to judge
4. focus hard on combat feel, enemy interactions, and the core "is this fun?" loop
5. keep extracting clean gameplay services where `World` is still doing too much directly

High-level roadmap after the current agenda:

1. deepen combat before investing heavily in presentation
2. build multiple world "areas" with clearer progression
3. gate each area with a boss before the next area opens
4. transition between areas by having the player exit along a path and load into the next area
5. expand enemies, bosses, and area-specific content once the core combat loop is fun

Presentation philosophy:

1. placeholder visuals are fine for now
2. gameplay feel and combat fun come before a major graphics pass
3. art polish should follow once the core loop and progression are proving out

## Running Tests

Use:

```powershell
dotnet test MyGame.Tests\MyGame.Tests.csproj --no-restore
```

That is the recommended fast path for this repo.
