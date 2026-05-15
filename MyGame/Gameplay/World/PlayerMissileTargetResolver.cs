using Microsoft.Xna.Framework;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;

namespace MyGame.Gameplay.World;

public sealed class PlayerMissileTargetResolver
{
    public void AssignTarget(PlayerProjectile projectile, IReadOnlyList<EnemyActor> enemies)
    {
        if (projectile.Kind != PlayerRangedAttackKind.Missile)
        {
            return;
        }

        projectile.AssignTarget(FindTarget(projectile, enemies));
    }

    public EnemyActor? FindTarget(PlayerProjectile projectile, IReadOnlyList<EnemyActor> enemies)
    {
        var projectileCenter = GetCenter(projectile.Bounds);
        var forward = DirectionHelper.ToVector(projectile.Direction);
        var forwardTarget = FindClosestEnemy(projectileCenter, enemies, enemy => IsTargetable(enemy, projectileCenter, forward));
        return forwardTarget ?? FindClosestEnemy(projectileCenter, enemies, enemy => CanTarget(enemy));
    }

    private static EnemyActor? FindClosestEnemy(Vector2 projectileCenter, IReadOnlyList<EnemyActor> enemies, Func<EnemyActor, bool> predicate)
    {
        EnemyActor? closestEnemy = null;
        var closestDistanceSquared = float.MaxValue;

        foreach (var enemy in enemies)
        {
            if (!predicate(enemy))
            {
                continue;
            }

            var distanceSquared = Vector2.DistanceSquared(projectileCenter, GetCenter(enemy.Bounds));
            if (distanceSquared >= closestDistanceSquared)
            {
                continue;
            }

            closestDistanceSquared = distanceSquared;
            closestEnemy = enemy;
        }

        return closestEnemy;
    }

    private static bool IsTargetable(EnemyActor enemy, Vector2 projectileCenter, Vector2 forward)
    {
        if (!CanTarget(enemy))
        {
            return false;
        }

        var toEnemy = GetCenter(enemy.Bounds) - projectileCenter;
        return Vector2.Dot(toEnemy, forward) > 0f;
    }

    private static bool CanTarget(EnemyActor enemy)
    {
        return enemy.State != EnemyState.Dead && !enemy.IsBossStageTransitioning;
    }

    private static Vector2 GetCenter(Rectangle bounds)
    {
        return new Vector2(bounds.Center.X, bounds.Center.Y);
    }
}
