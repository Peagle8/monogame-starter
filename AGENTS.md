# MonoGame Project Guidelines

- Prefer composition over inheritance
- Keep MonoGame types at the edges
- Use FrameTime instead of GameTime in core logic
- Write testable pure C# logic for gameplay systems
- Avoid ECS unless explicitly requested
- Prefer simple, readable code over abstraction
- Follow existing folder structure
- Add unit tests for all new logic
- A method should never be more than 100 lines. If it approaches 100 lines break it up into smaller methods.
- No class should be over 500 lines, if it approaches 500 lines break it up into smaller components.

## Architecture

- Keep gameplay rules in plain C# classes with minimal framework coupling
- Scenes coordinate objects; they should not own all gameplay logic directly
- Rendering, input, file IO, and platform APIs should stay in adapter or service layers
- Prefer small focused services over large manager classes
- Pass dependencies explicitly through constructors
- Simple is always better than complex (KISS)
- Cognitive complexity for methods should never go over 15

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
- If a DI registration block grows past roughly 3 related services of the same flavor, extract it into a dedicated `IServiceCollection` extension method in its own file

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
