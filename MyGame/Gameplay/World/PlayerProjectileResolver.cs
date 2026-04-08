using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.Props;

namespace MyGame.Gameplay.World;

public sealed class PlayerProjectileResolver
{
    public bool Resolve(IReadOnlyList<PlayerProjectile> projectiles, IReadOnlyList<EnemyActor> enemies, IReadOnlyList<IWorldProp> props)
    {
        var hitEnemy = false;

        foreach (var projectile in projectiles)
        {
            if (!projectile.IsActive)
            {
                continue;
            }

            if (HitsBlockingProp(projectile, props))
            {
                projectile.Deactivate();
                continue;
            }

            foreach (var enemy in enemies)
            {
                if (enemy.State == EnemyState.Dead || !projectile.Bounds.Intersects(enemy.Bounds))
                {
                    continue;
                }

                enemy.TakeDamage(projectile.Damage);
                enemy.ApplyKnockback(DirectionHelper.ToVector(projectile.Direction));
                projectile.Deactivate();
                hitEnemy = true;
                break;
            }
        }

        return hitEnemy;
    }

    private static bool HitsBlockingProp(PlayerProjectile projectile, IReadOnlyList<IWorldProp> props)
    {
        return props.Any(prop => prop.BlocksMovement && projectile.Bounds.Intersects(prop.CollisionBounds));
    }
}
