using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.Props;

namespace MyGame.Gameplay.World;

public sealed class EnemyProjectileResolver
{
    public bool Resolve(IReadOnlyList<EnemyProjectile> projectiles, PlayerActor player, IReadOnlyList<IWorldProp> props)
    {
        if (player.IsDead)
        {
            return false;
        }

        var hitPlayer = false;

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

            if (!projectile.Bounds.Intersects(player.Bounds))
            {
                continue;
            }

            projectile.Deactivate();
            if (player.TryAbsorbShieldHit())
            {
                continue;
            }

            player.TakeDamage(projectile.Damage);
            hitPlayer = true;
        }

        return hitPlayer;
    }

    private static bool HitsBlockingProp(EnemyProjectile projectile, IReadOnlyList<IWorldProp> props)
    {
        return props.Any(prop => prop.BlocksMovement && projectile.Bounds.Intersects(prop.CollisionBounds));
    }
}
