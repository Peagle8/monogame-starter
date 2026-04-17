namespace MyGame.Configuration;

public sealed class PlayerMovementSettings
{
    public float MoveSpeed { get; init; } = 144f;

    public float DashDistance { get; init; } = 86.4f;

    public float DashSeconds { get; init; } = 0.18f;

    public float DashCooldownSeconds { get; init; } = 0.28f;

    public float ContactKnockbackDistance { get; init; } = 22f;

    public float ContactKnockbackSeconds { get; init; } = 0.12f;
}
