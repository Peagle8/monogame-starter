namespace MyGame.Gameplay.Enemies;

internal static class EnemyBehaviorFactory
{
    public static IEnemyBehavior Create(EnemyKind kind, EnemyAxisPreference axisPreference, float initialDashPauseSeconds)
    {
        return kind switch
        {
            EnemyKind.Crab => new CrabEnemyBehavior(),
            EnemyKind.HornedRabbit => new HornedRabbitEnemyBehavior(axisPreference, initialDashPauseSeconds),
            EnemyKind.HornedRabbitBoss => new HornedRabbitBossEnemyBehavior(),
            EnemyKind.HornedRabbitElite => new HornedRabbitEliteEnemyBehavior(axisPreference, initialDashPauseSeconds),
            EnemyKind.Bat => new BatEnemyBehavior(initialDashPauseSeconds),
            EnemyKind.BatMiniBoss => new BatMiniBossEnemyBehavior(),
            EnemyKind.Grasshopper => new GrasshopperEnemyBehavior(initialDashPauseSeconds),
            EnemyKind.Skeleton => new SkeletonEnemyBehavior(initialDashPauseSeconds),
            EnemyKind.SkeletonElite => new SkeletonEnemyBehavior(
                initialDashPauseSeconds,
                projectilesPerVolley: 2,
                projectileSpreadDegrees: 14f,
                usesBackstepLeap: true),
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unsupported enemy kind.")
        };
    }
}
