using Microsoft.Xna.Framework;

namespace MyGame.Gameplay.Player;

public sealed record PlayerAttackState(
    bool IsAttacking,
    Rectangle? AttackBounds,
    int AttackSequence,
    float RemainingActiveSeconds,
    float RemainingCooldownSeconds)
{
    public static readonly PlayerAttackState Idle = new(
        false,
        null,
        0,
        0f,
        0f);
}
