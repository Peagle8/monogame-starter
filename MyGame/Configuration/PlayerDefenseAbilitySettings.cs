namespace MyGame.Configuration;

public sealed class PlayerDefenseAbilitySettings
{
    public float ShieldActivationCost { get; init; } = 3f;

    public int ShieldMaxCharges { get; init; } = 3;
}
