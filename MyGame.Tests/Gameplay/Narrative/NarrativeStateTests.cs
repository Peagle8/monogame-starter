using MyGame.Gameplay.Narrative;

namespace MyGame.Tests.Gameplay.Narrative;

public sealed class NarrativeStateTests
{
    [Fact]
    public void SetTownState_StoresAlertLevelAndClampsReputation()
    {
        var state = new NarrativeState();

        state.SetTownState(TownAlertLevel.Alarmed, 250);

        Assert.Equal(TownAlertLevel.Alarmed, state.TownAlertLevel);
        Assert.Equal(NarrativeState.MaximumReputation, state.PlayerReputation);
    }

    [Fact]
    public void AdjustPlayerReputation_ClampsToMinimumAndMaximum()
    {
        var state = new NarrativeState();

        state.AdjustPlayerReputation(-250);
        Assert.Equal(NarrativeState.MinimumReputation, state.PlayerReputation);

        state.AdjustPlayerReputation(300);
        Assert.Equal(NarrativeState.MaximumReputation, state.PlayerReputation);
    }
}
