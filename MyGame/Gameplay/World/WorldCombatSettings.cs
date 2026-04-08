namespace MyGame.Gameplay.World;

public sealed class WorldCombatSettings
{
    public int ContactDamage { get; init; } = 1;

    public float ContactDamageCooldownSeconds { get; init; } = 0.5f;

    public float EnemySeparationDistance { get; init; } = 28f;

    public int EnemySeparationIterations { get; init; } = 2;

    public int EnemyObstacleResolutionIterations { get; init; } = 2;

    public int PlayerObstacleResolutionIterations { get; init; } = 2;
}
