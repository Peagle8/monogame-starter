namespace MyGame.Gameplay.World;

public sealed class WorldCombatSettings
{
    public int ContactDamage { get; init; } = 1;

    public float ContactDamageCooldownSeconds { get; init; } = 0.5f;
}
