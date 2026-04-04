# MonoGame Project Guidelines

- Prefer composition over inheritance
- Keep MonoGame types at the edges
- Use FrameTime instead of GameTime in core logic
- Write testable pure C# logic for gameplay systems
- Avoid ECS unless explicitly requested
- Prefer simple, readable code over abstraction
- Follow existing folder structure
- Add unit tests for all new logic

## Architecture

- Keep gameplay rules in plain C# classes with minimal framework coupling
- Scenes coordinate objects; they should not own all gameplay logic directly
- Rendering, input, file IO, and platform APIs should stay in adapter or service layers
- Prefer small focused services over large manager classes
- Pass dependencies explicitly through constructors

## State And Data

- Separate simulation state from rendering state
- Keep config and tuning data in plain data objects that can later be loaded from JSON
- Avoid hidden global state unless there is a strong reason
- Favor deterministic update logic where practical

## Code Style

- Prefer clear names over short names
- Keep methods small and single-purpose
- Avoid clever abstractions unless they remove real duplication
- Add comments only when intent is not obvious from the code
- Match the style of nearby files before introducing new patterns

## Testing

- Test gameplay rules, calculations, and state transitions in unit tests
- Avoid tests that depend on graphics devices, windows, or real-time timing when possible
- For bug fixes, add or update a test that proves the fix
- Prefer fast tests that run from `dotnet test`

## Changes And Safety

- Do not rewrite unrelated files
- Preserve public APIs unless the task requires changing them
- When introducing new folders or systems, follow the existing project shape first
- If a requested change conflicts with these guidelines, call it out before implementing
- In assistant responses, prefer plain file references like `MyGame/GameRoot.cs:46` instead of markdown links
- Do not run the unit tests at the same time as compiling the solution since that will result in a file lock.
