using MyGame.Gameplay.Enemies;

namespace MyGame.Infrastructure.Save;

public sealed class EnemySaveData
{
    public required EnemyKind Kind { get; init; }

    public required EnemyAxisPreference AxisPreference { get; init; }

    public required float PositionX { get; init; }

    public required float PositionY { get; init; }

    public required int CurrentHealth { get; init; }
}
