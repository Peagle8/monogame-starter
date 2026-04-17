namespace MyGame.Configuration;

public sealed class PlayerAttackSettings
{
    public int Damage { get; init; } = 1;

    public float ActiveSeconds { get; init; } = 0.144f;

    public float CooldownSeconds { get; init; } = 0.28f;

    public int Range { get; init; } = 30;
}
