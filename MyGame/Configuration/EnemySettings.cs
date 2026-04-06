using MyGame.Gameplay.Enemies;

namespace MyGame.Configuration;

public sealed class EnemySettings
{
    public EnemyKind Kind { get; init; } = EnemyKind.Crab;

    public int MaxHealth { get; init; } = 3;

    public float MoveSpeed { get; init; } = 120f;

    public float ChaseRange { get; init; } = 160f;

    public float RecoverySeconds { get; init; } = 0.65f;

    public float DefeatedVisibleSeconds { get; init; } = 0.8f;

    public float PlayerHitKnockbackDistance { get; init; } = 24f;

    public float PlayerHitKnockbackSeconds { get; init; } = 0.12f;

    public float PlayerHitPauseSeconds { get; init; } = 0.065f;

    public float DashSpeed { get; init; } = 0f;

    public float DashSeconds { get; init; } = 0f;

    public float DashPauseSeconds { get; init; } = 0f;

    public float InitialDashPauseMinSeconds { get; init; } = 0f;

    public float InitialDashPauseMaxSeconds { get; init; } = 0f;
}
