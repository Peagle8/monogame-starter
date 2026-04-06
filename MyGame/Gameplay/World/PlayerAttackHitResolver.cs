using Microsoft.Xna.Framework;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;

namespace MyGame.Gameplay.World;

public sealed class PlayerAttackHitResolver
{
    private readonly Dictionary<EnemyActor, int> _enemyLastHitByAttackSequence = new();

    public bool Resolve(PlayerActor player, IReadOnlyList<EnemyActor> enemies)
    {
        if (!player.IsAttacking || player.AttackBounds is null)
        {
            return false;
        }

        var hitEnemy = false;

        foreach (var enemy in enemies)
        {
            if (enemy.State == EnemyState.Dead)
            {
                continue;
            }

            if (_enemyLastHitByAttackSequence.TryGetValue(enemy, out var lastAttackSequence)
                && lastAttackSequence == player.AttackSequence)
            {
                continue;
            }

            if (!enemy.Bounds.Intersects(player.AttackBounds.Value))
            {
                continue;
            }

            enemy.TakeDamage(player.AttackDamage);
            enemy.ApplyKnockback(GetEnemyKnockbackDirection(player, enemy));
            _enemyLastHitByAttackSequence[enemy] = player.AttackSequence;
            hitEnemy = true;
        }

        return hitEnemy;
    }

    public void Reset()
    {
        _enemyLastHitByAttackSequence.Clear();
    }

    private static Vector2 GetEnemyKnockbackDirection(PlayerActor player, EnemyActor enemy)
    {
        var knockbackDirection = enemy.Position - player.Position;
        if (knockbackDirection.LengthSquared() > 0.0001f)
        {
            return knockbackDirection;
        }

        return DirectionHelper.ToVector(player.Facing);
    }
}
