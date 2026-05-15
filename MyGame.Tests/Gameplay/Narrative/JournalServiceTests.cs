using MyGame.Gameplay.Narrative;

namespace MyGame.Tests.Gameplay.Narrative;

public sealed class JournalServiceTests
{
    [Fact]
    public void GetAvailableEntries_WhenFlagsMatch_ReturnsEntriesByPriority()
    {
        var state = new NarrativeState();
        state.SetFlag(NarrativeIds.FlagMetTownsfolkOne);
        state.SetFlag(NarrativeIds.FlagMetTownsfolkTwo);
        var service = CreateService(
        [
            CreateEntry("low", "Low", priority: 1, NarrativeIds.FlagMetTownsfolkOne),
            CreateEntry("high", "High", priority: 10, NarrativeIds.FlagMetTownsfolkOne, NarrativeIds.FlagMetTownsfolkTwo)
        ]);

        var entries = service.GetAvailableEntries(state);

        Assert.Equal(["high", "low"], entries.Select(entry => entry.Id));
    }

    [Fact]
    public void DiscoverAvailableEntries_RecordsOnlyNewEntries()
    {
        var state = new NarrativeState();
        state.SetFlag(NarrativeIds.FlagMetTownsfolkOne);
        var journalState = new JournalState();
        journalState.Discover("existing");
        var service = CreateService(
        [
            CreateEntry("existing", "Existing", priority: 1, NarrativeIds.FlagMetTownsfolkOne),
            CreateEntry("new", "New", priority: 2, NarrativeIds.FlagMetTownsfolkOne)
        ]);

        var discovered = service.DiscoverAvailableEntries(state, journalState);

        Assert.Equal(["new"], discovered.Select(entry => entry.Id));
        Assert.True(journalState.IsDiscovered("existing"));
        Assert.True(journalState.IsDiscovered("new"));
    }

    [Fact]
    public void GetDiscoveredEntries_ReturnsOnlyDiscoveredEntriesByPriority()
    {
        var journalState = new JournalState();
        journalState.Discover("low");
        journalState.Discover("high");
        var service = CreateService(
        [
            CreateEntry("low", "Low", priority: 1),
            CreateEntry("hidden", "Hidden", priority: 20),
            CreateEntry("high", "High", priority: 10)
        ]);

        var entries = service.GetDiscoveredEntries(journalState);

        Assert.Equal(["high", "low"], entries.Select(entry => entry.Id));
    }

    private static JournalService CreateService(JournalEntryTemplate[] entries)
    {
        return new JournalService(new JournalDataFile { Entries = entries.ToList() });
    }

    private static JournalEntryTemplate CreateEntry(
        string id,
        string title,
        int priority,
        params string[] requiredFlags)
    {
        return new JournalEntryTemplate
        {
            Id = id,
            RequiredFlags = requiredFlags.ToList(),
            Priority = priority,
            Title = title,
            Summary = $"{title} summary"
        };
    }
}
