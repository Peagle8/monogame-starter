namespace MyGame.Gameplay.Player;

public sealed record PlayerDefenseAbilityState(
    PlayerDefenseAbilityKind EquippedAbility,
    bool IsActive,
    int RemainingCharges)
{
    public static readonly PlayerDefenseAbilityState Default = new(
        PlayerDefenseAbilityKind.Shield,
        false,
        0);
}
