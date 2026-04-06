using Microsoft.Xna.Framework;
using MyGame.Gameplay.Enemies;

namespace MyGame.Gameplay.World;

public sealed record EnemySpawnDefinition(
    EnemyKind Kind,
    Vector2 Position,
    EnemyAxisPreference AxisPreference = EnemyAxisPreference.None);
