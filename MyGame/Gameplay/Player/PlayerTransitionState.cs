namespace MyGame.Gameplay.Player;

public sealed record PlayerTransitionState(
    int CurrentHealth,
    float CurrentAbilityPoints,
    Direction Facing,
    PlayerDefenseAbilityState DefenseAbilityState,
    PlayerRangedAttackState RangedAttackState);
