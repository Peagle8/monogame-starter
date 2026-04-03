namespace MyGame.Configuration;

public sealed class PlayerAttackSettings
{
    public int Damage { get; init; } = 1;

    public float ActiveSeconds { get; init; } = 0.18f;

    public float CooldownSeconds { get; init; } = 0.35f;

    public int Range { get; init; } = 22;
}
