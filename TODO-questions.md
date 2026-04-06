# TODO Questions

This file collects the TODOs that need your direction before I should implement them.

## 1. Enemy behavior architecture

Files:
- `MyGame/Gameplay/Enemies/EnemyActor.cs:186`
- `MyGame/Gameplay/Enemies/EnemyActor.cs:187`
- `MyGame/Rendering/Enemies/EnemyRenderer.cs:31`

Current situation:
- `EnemyActor` still contains per-enemy update methods like `UpdateCrab` and `UpdateHornedRabbit`.
- `EnemyRenderer` still branches by enemy kind and has enemy-specific drawing logic in one class.

Question:
- How do you want enemy-specific behavior/rendering organized before we add a third enemy?

Possible direction:
- Split gameplay behavior into one strategy/service per enemy kind.
- Split rendering into one renderer per enemy kind with a small dispatcher.

Why I paused:
- Your TODO explicitly says to pause and ask for your opinion before refactoring this area.

Answer:
- Yes let's split this into types per enemy, perhaps with a base class if there is a lot of common behavior

## 2. Player max health config location

File:
- `MyGame/Gameplay/Player/PlayerActor.cs:10`

Current situation:
- `PlayerActor` still uses a hard-coded `DefaultMaxHealth = 5`.

Question:
- Where do you want player max health to live?

Possible direction:
- Add it to `PlayerMovementSettings`.
- Create a broader `PlayerSettings` type.
- Create a combat-focused player settings type.

Why I paused:
- The implementation is straightforward, but the right config home is a design choice.

Answer:
- Let's create a combat-focused player settings type. I think we will need to also up the health a lot from where it's at now. It's a value we will want to have the player expand thru success in the game per run.

## 3. World contact/combat extraction

Files:
- `MyGame/Gameplay/World/World.cs:13`
- `MyGame/Gameplay/World/World.cs:153`
- `MyGame/Gameplay/World/World.cs:154`
- `MyGame/Gameplay/World/World.cs:209`

Current situation:
- `World` still owns contact damage constants and private combat/contact resolution methods.

Questions:
- Do you want contact damage and cooldown moved into config now?
- Do you want attack hit resolution and enemy contact resolution extracted into small services now, or left in `World` until gameplay expands?
- If extracted, do you prefer one combined combat resolver or two focused services?

Possible direction:
- Add a small world/combat config object.
- Introduce one or two focused services that `World` invokes.

Why I paused:
- This is a boundary/ownership decision, not just a cleanup.

Answer:
- Do you want contact damage and cooldown moved into config now? - yes
- Do you want attack hit resolution and enemy contact resolution extracted into small services now, or left in `World` until gameplay expands? - Lets move these now
- If extracted, do you prefer one combined combat resolver or two focused services? - two focused services

## 4. Enemy spawning and map ownership

Files:
- `MyGame/Infrastructure/DependencyInjection/ServiceRegistration.cs:52`
- `MyGame/Infrastructure/DependencyInjection/ServiceRegistration.cs:64`

Current situation:
- Horned rabbit instances are still created directly in DI.
- Randomized initial dash pause is also still defined in service registration.

Questions:
- How do you want enemy spawn data owned once we have multiple maps or levels?
- Should enemy creation come from a map definition, a factory, a level builder, or something else?
- Where should horned rabbit-specific spawn/randomization logic live?

Possible direction:
- Introduce map-specific spawn definitions plus an enemy factory.
- Move horned rabbit spawn logic out of DI into a dedicated builder/factory/type.

Why I paused:
- This depends on how you want future maps/levels represented.

Answers:
- How do you want enemy spawn data owned once we have multiple maps or levels? - I think there should be a enemy spawn map type that details how each map should spawn enemies. That was we can encapsulate that as well as other 
aspects we haven't hit yet with enemy spawning like frequency,re-spawn rate etc
- Should enemy creation come from a map definition, a factory, a level builder, or something else? - I think it should funnel thru a level builder
- Where should horned rabbit-specific spawn/randomization logic live? - think the last two points answered that

## 5. Gameplay pause menu construction ownership

File:
- `MyGame/Scenes/Gameplay/GameplayScene.cs:47`

Current situation:
- `GameplayScene` still assembles the pause menu with several callbacks and feature flags.

Question:
- Do you want pause menu construction to stay in `GameplayScene`, move into `GameplayPauseMenu`, or move into a separate factory/builder?

Possible direction:
- Keep it in scene coordination.
- Move wiring into a factory for cleaner scene construction.
- Push more construction into `GameplayPauseMenu` itself if you want the menu to own more of its setup.

Why I paused:
- This is mostly a preference about object ownership and scene responsibility.

Answer:
- Lets always follow the KISS principle and: Push more construction into `GameplayPauseMenu` itself if you want the menu to own more of its setup.

## Suggested order for next pass

If you want a recommended order, I’d tackle them like this:

1. Enemy behavior/rendering architecture
2. World combat/contact extraction
3. Enemy spawning/map ownership
4. Player max health config placement
5. Gameplay pause menu construction ownership
