namespace MyGame.Configuration;

public sealed class PlayerCombatSettings
{
    public int MaxHealth { get; init; } = 20;

    public float MaxAbilityPoints { get; init; } = 3f;

    public float AbilityPointRegenPerSecond { get; init; } = 0.05f;
}
