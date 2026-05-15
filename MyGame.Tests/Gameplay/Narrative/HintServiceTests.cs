using MyGame.Gameplay.Narrative;

namespace MyGame.Tests.Gameplay.Narrative;

public sealed class HintServiceTests
{
    [Fact]
    public void SelectHint_WhenEntriesMatch_ReturnsHighestPriorityHint()
    {
        var service = CreateService(
        [
            CreateEntry("low", "Low priority.", priority: 1),
            CreateEntry("high", "High priority.", priority: 10)
        ]);

        var hint = service.SelectHint(
            NarrativeIds.ZoneOverworld,
            new NarrativeState(),
            new RecentSelectionHistory());

        Assert.Equal("high", hint.Id);
        Assert.Equal("High priority.", hint.Text);
    }

    [Fact]
    public void SelectHint_WhenHighestPriorityWasRecent_ReturnsFreshLowerPriorityHint()
    {
        var history = new RecentSelectionHistory();
        history.Record(HintService.HistorySystemName, "high");
        var service = CreateService(
        [
            CreateEntry("low", "Low priority.", priority: 1),
            CreateEntry("high", "High priority.", priority: 10)
        ]);

        var hint = service.SelectHint(NarrativeIds.ZoneOverworld, new NarrativeState(), history);

        Assert.Equal("low", hint.Id);
    }

    [Fact]
    public void SelectHint_WhenNoEntryMatches_ReturnsFallback()
    {
        var service = CreateService([]);

        var hint = service.SelectHint(
            NarrativeIds.ZoneOverworld,
            new NarrativeState(),
            new RecentSelectionHistory());

        Assert.Equal("hint_fallback", hint.Id);
    }

    private static HintService CreateService(HintEntry[] entries)
    {
        return new HintService(
            new HintDataFile { Entries = entries.ToList() },
            new RecentSelectionHistory());
    }

    private static HintEntry CreateEntry(string id, string text, int priority)
    {
        return new HintEntry
        {
            Id = id,
            ZoneId = NarrativeIds.ZoneOverworld,
            ObjectiveId = NarrativeIds.ObjectiveMeetTownsfolk,
            Priority = priority,
            Text = text
        };
    }
}
