using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Core;

namespace MyGame.Gameplay.Player;

public sealed class PlayerRangedAttackController
{
    private readonly PlayerRangedAttackSettings _settings;

    public PlayerRangedAttackController(PlayerRangedAttackSettings settings)
    {
        _settings = settings;
    }

    public PlayerRangedAttackUpdateResult Update(
        PlayerRangedAttackState currentState,
        Vector2 playerPosition,
        Direction facing,
        bool rangedAttackJustPressed,
        bool canUseEquippedAttack,
        FrameTime frameTime)
    {
        var remainingCooldownSeconds = Math.Max(0f, currentState.RemainingCooldownSeconds - frameTime.DeltaSeconds);
        PlayerProjectile? projectile = null;

        if (rangedAttackJustPressed && remainingCooldownSeconds <= 0f && canUseEquippedAttack)
        {
            projectile = CreateProjectile(currentState.EquippedAttack, playerPosition, facing);
            remainingCooldownSeconds = _settings.CooldownSeconds;
        }

        return new PlayerRangedAttackUpdateResult(
            new PlayerRangedAttackState(currentState.EquippedAttack, remainingCooldownSeconds),
            projectile);
    }

    private PlayerProjectile CreateProjectile(PlayerRangedAttackKind equippedAttack, Vector2 playerPosition, Direction facing)
    {
        return equippedAttack switch
        {
            PlayerRangedAttackKind.Fireball => new PlayerProjectile(
                equippedAttack,
                CreateProjectilePosition(playerPosition, facing),
                facing,
                _settings.ProjectileSpeed,
                _settings.ProjectileLifetimeSeconds,
                _settings.ProjectileSize,
                _settings.Damage),
            _ => throw new ArgumentOutOfRangeException(nameof(equippedAttack), equippedAttack, null)
        };
    }

    private Vector2 CreateProjectilePosition(Vector2 playerPosition, Direction facing)
    {
        var playerBounds = new Rectangle((int)playerPosition.X, (int)playerPosition.Y, 32, 32);
        var projectileSize = _settings.ProjectileSize;

        return facing switch
        {
            Direction.Up => new Vector2(playerBounds.Center.X - (projectileSize / 2f), playerBounds.Y - projectileSize),
            Direction.Down => new Vector2(playerBounds.Center.X - (projectileSize / 2f), playerBounds.Bottom),
            Direction.Left => new Vector2(playerBounds.X - projectileSize, playerBounds.Center.Y - (projectileSize / 2f)),
            Direction.Right => new Vector2(playerBounds.Right, playerBounds.Center.Y - (projectileSize / 2f)),
            _ => playerPosition
        };
    }
}
