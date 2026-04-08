namespace MyGame.Gameplay.Enemies;

internal static class EnemyBehaviorFactory
{
    public static IEnemyBehavior Create(EnemyKind kind, EnemyAxisPreference axisPreference, float initialDashPauseSeconds)
    {
        return kind switch
        {
            EnemyKind.Crab => new CrabEnemyBehavior(),
            EnemyKind.HornedRabbit => new HornedRabbitEnemyBehavior(axisPreference, initialDashPauseSeconds),
            EnemyKind.Bat => new BatEnemyBehavior(initialDashPauseSeconds),
            EnemyKind.Grasshopper => new GrasshopperEnemyBehavior(initialDashPauseSeconds),
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unsupported enemy kind.")
        };
    }
}
