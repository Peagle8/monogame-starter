using Microsoft.Xna.Framework;
using MyGame.Gameplay.Props;

namespace MyGame.Gameplay.World;

public sealed record WildernessSceneDefinition(
    Rectangle Bounds,
    IReadOnlyList<IWorldProp> Props,
    IReadOnlyList<EnemySpawnDefinition> Spawns,
    IReadOnlyList<WorldSceneTransition> SceneTransitions);
