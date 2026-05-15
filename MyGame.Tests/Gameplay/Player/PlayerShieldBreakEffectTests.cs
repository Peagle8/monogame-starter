using MyGame.Gameplay.Player;

namespace MyGame.Tests.Gameplay.Player;

public sealed class PlayerShieldBreakEffectTests
{
    [Fact]
    public void Begin_StartsEffectWithFullAlpha()
    {
        var effect = new PlayerShieldBreakEffect();

        effect.Begin(PlayerDefenseAbilityKind.FireShield);

        Assert.True(effect.IsActive);
        Assert.Equal(PlayerDefenseAbilityKind.FireShield, effect.Kind);
        Assert.Equal(1f, effect.Alpha);
        Assert.Equal(0f, effect.Progress);
    }

    [Fact]
    public void Update_AfterDuration_EndsEffect()
    {
        var effect = new PlayerShieldBreakEffect();
        effect.Begin(PlayerDefenseAbilityKind.Shield);

        effect.Update(0.5f);

        Assert.False(effect.IsActive);
        Assert.Equal(0f, effect.Alpha);
        Assert.Equal(1f, effect.Progress);
    }
}
