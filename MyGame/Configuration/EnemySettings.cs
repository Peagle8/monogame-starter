namespace MyGame.Configuration;

public sealed class EnemySettings
{
    public int MaxHealth { get; init; } = 3;

    public float MoveSpeed { get; init; } = 120f;

    public float ChaseRange { get; init; } = 160f;

    public float RecoverySeconds { get; init; } = 0.65f;

    public float DefeatedVisibleSeconds { get; init; } = 0.8f;
}
