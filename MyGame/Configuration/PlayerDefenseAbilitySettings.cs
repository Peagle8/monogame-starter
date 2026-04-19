namespace MyGame.Configuration;

public sealed class PlayerDefenseAbilitySettings
{
    public float ShieldActivationCost { get; init; } = 3f;

    public int ShieldMaxCharges { get; init; } = 3;

    public float FireShieldActivationCost { get; init; } = 3f;

    public float FireShieldDurationSeconds { get; init; } = 9f;

    public int FireShieldDamage { get; init; } = 1;

    public float FireShieldDamageTickSeconds { get; init; } = 3f;

    public float FireShieldRadiusMultiplier { get; init; } = 4f;

    public int FireShieldRingThickness { get; init; } = 12;

    public float FireShieldPulseAmplitude { get; init; } = 6f;
}
