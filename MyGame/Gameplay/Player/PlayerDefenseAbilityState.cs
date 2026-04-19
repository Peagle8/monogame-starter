namespace MyGame.Gameplay.Player;

public sealed record PlayerDefenseAbilityState(
    PlayerDefenseAbilityKind EquippedAbility,
    bool IsActive,
    int RemainingCharges,
    float RemainingActiveSeconds)
{
    public static readonly PlayerDefenseAbilityState Default = new(
        PlayerDefenseAbilityKind.Shield,
        false,
        0,
        0f);
}
