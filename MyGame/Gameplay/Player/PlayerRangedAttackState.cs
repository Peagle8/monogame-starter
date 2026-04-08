namespace MyGame.Gameplay.Player;

public sealed record PlayerRangedAttackState(
    PlayerRangedAttackKind EquippedAttack,
    float RemainingCooldownSeconds)
{
    public static readonly PlayerRangedAttackState Default = new(
        PlayerRangedAttackKind.Fireball,
        0f);
}
