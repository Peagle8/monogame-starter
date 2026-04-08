namespace MyGame.Configuration;

public sealed class PlayerRangedAttackSettings
{
    public int Damage { get; init; } = 1;

    public float CooldownSeconds { get; init; } = 0.35f;

    public float ProjectileSpeed { get; init; } = 280f;

    public float ProjectileLifetimeSeconds { get; init; } = 0.9f;

    public int ProjectileSize { get; init; } = 24;
}
