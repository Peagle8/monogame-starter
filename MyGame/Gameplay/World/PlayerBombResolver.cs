using Microsoft.Xna.Framework;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.Props;

namespace MyGame.Gameplay.World;

public sealed class PlayerBombResolver
{
    public bool Resolve(IReadOnlyList<PlayerBomb> bombs, List<IWorldProp> props, IReadOnlyList<EnemyActor> enemies)
    {
        var hitEnemy = false;

        foreach (var bomb in bombs)
        {
            if (!bomb.TryConsumeExplosion(out var explosionBounds, out var damage))
            {
                continue;
            }

            RemoveGrass(props, explosionBounds);
            var explosionCenter = new Vector2(explosionBounds.Center.X, explosionBounds.Center.Y);

            foreach (var enemy in enemies)
            {
                if (enemy.State == EnemyState.Dead
                    || enemy.IsBossStageTransitioning
                    || !enemy.Bounds.Intersects(explosionBounds))
                {
                    continue;
                }

                if (!enemy.TryTakeDamage(damage))
                {
                    continue;
                }

                enemy.ApplyKnockback(GetKnockbackDirection(explosionCenter, enemy));
                hitEnemy = true;
            }
        }

        return hitEnemy;
    }

    private static void RemoveGrass(List<IWorldProp> props, Rectangle explosionBounds)
    {
        props.RemoveAll(prop => prop is GrassProp grass && grass.Bounds.Intersects(explosionBounds));
    }

    private static Vector2 GetKnockbackDirection(Vector2 explosionCenter, EnemyActor enemy)
    {
        var enemyCenter = new Vector2(enemy.Bounds.Center.X, enemy.Bounds.Center.Y);
        var direction = enemyCenter - explosionCenter;
        if (direction.LengthSquared() > 0.0001f)
        {
            return direction;
        }

        return DirectionHelper.ToVector(enemy.DashDirection);
    }
}
