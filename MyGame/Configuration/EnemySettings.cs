using MyGame.Gameplay.Enemies;

namespace MyGame.Configuration;

public sealed class EnemySettings
{
    public EnemyKind Kind { get; init; } = EnemyKind.Crab;

    public int MaxHealth { get; init; } = 8;

    public float MoveSpeed { get; init; } = 96f;

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

    public int AttackHitboxPadding { get; init; } = 0;

    public int BoundsWidth { get; init; } = 28;

    public int BoundsHeight { get; init; } = 28;

    public float MaxAbilityPoints { get; init; } = 0f;

    public float AbilityPointRegenPerSecond { get; init; } = 0f;

    public float ShieldActivationCost { get; init; } = 0f;

    public int ShieldMaxCharges { get; init; } = 0;

    public int ProjectileDamage { get; init; } = 0;

    public float ProjectileSpeed { get; init; } = 0f;

    public float ProjectileLifetimeSeconds { get; init; } = 0f;

    public int ProjectileSize { get; init; } = 0;

    public float ProjectileAttackRange { get; init; } = 0f;

    public float PreferredRange { get; init; } = 0f;

    public int SpecialAttackDamage { get; init; } = 0;

    public float SpecialAttackRange { get; init; } = 0f;

    public float SpecialAttackPauseSeconds { get; init; } = 0f;

    public float SpecialAttackStunSeconds { get; init; } = 0f;

    public float SpecialAttackConeHalfAngleDegrees { get; init; } = 35f;
}
