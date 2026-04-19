namespace MyGame.Gameplay.Player;

public sealed record PlayerTransitionState(
    int CurrentHealth,
    float CurrentAbilityPoints,
    Direction Facing,
    PlayerDashAbilityKind EquippedDashAbility,
    PlayerDefenseAbilityState DefenseAbilityState,
    PlayerRangedAttackState RangedAttackState,
    PlayerMeleeAbilityKind EquippedMeleeAbility);
