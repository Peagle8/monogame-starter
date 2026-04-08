using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;

namespace MyGame.Gameplay.World;

public sealed class EnemyContactResolver
{
    private readonly WorldCombatSettings _settings;
    private float _remainingContactDamageCooldown;

    public EnemyContactResolver(WorldCombatSettings settings)
    {
        _settings = settings;
    }

    public void Resolve(PlayerActor player, IReadOnlyList<EnemyActor> enemies, FrameTime frameTime)
    {
        _remainingContactDamageCooldown = Math.Max(0f, _remainingContactDamageCooldown - frameTime.DeltaSeconds);

        if (_remainingContactDamageCooldown > 0f || player.IsDead)
        {
            return;
        }

        foreach (var enemy in enemies)
        {
            if (!enemy.CanDealContactDamage)
            {
                continue;
            }

            if (!enemy.ContactBounds.Intersects(player.Bounds))
            {
                continue;
            }

            var knockbackDirection = GetPlayerKnockbackDirection(player, enemy);

            if (!player.TryAbsorbShieldHit())
            {
                player.TakeDamage(_settings.ContactDamage);
            }

            player.ApplyKnockback(knockbackDirection);
            enemy.BeginRecovery();
            _remainingContactDamageCooldown = _settings.ContactDamageCooldownSeconds;
            break;
        }
    }

    public void Reset()
    {
        _remainingContactDamageCooldown = 0f;
    }

    private static Vector2 GetPlayerKnockbackDirection(PlayerActor player, EnemyActor enemy)
    {
        var knockbackDirection = player.Position - enemy.Position;
        if (knockbackDirection.LengthSquared() > 0.0001f)
        {
            return knockbackDirection;
        }

        return -DirectionHelper.ToVector(player.Facing);
    }
}
