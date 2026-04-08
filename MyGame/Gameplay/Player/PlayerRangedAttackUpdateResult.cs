namespace MyGame.Gameplay.Player;

public sealed record PlayerRangedAttackUpdateResult(
    PlayerRangedAttackState State,
    PlayerProjectile? Projectile);
