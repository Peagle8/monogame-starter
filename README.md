# MonoGame Starter Skeleton

This starter keeps MonoGame at the edges and pushes game rules into plain C#.

## Shape

- `MyGame` contains the host, scenes, input, rendering, diagnostics, and gameplay code.
- `MyGame.Tests` contains fast tests for pure logic.

## First milestones

1. Get the solution restoring and launching.
2. Replace the placeholder rectangle drawing with sprites.
3. Add a pause menu scene.
4. Add a `World` object and move gameplay state out of the scene.
5. Add JSON-backed tuning for player movement and enemy stats.
6. Add deterministic recordings for bug repro and tests.

## Notes

- `GameRoot` is intentionally boring.
- `SceneManager` is simple on purpose.
- `IInputService` maps hardware input to `GameAction`.
- `DebugOverlay` and `GameRecorder` are first-class from day one.
- `MenuItem` uses `Action` so scene behavior stays code-first and easy to review.

## Suggested next files

- `World`
- `PlayerRenderer`
- `Camera2D`
- `JsonFileLoader<T>`
- `SaveGameService`
- `RecordedFrame`
- `ReplayInputService`
