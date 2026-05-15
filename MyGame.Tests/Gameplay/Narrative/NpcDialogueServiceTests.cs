using MyGame.Gameplay.Narrative;

namespace MyGame.Tests.Gameplay.Narrative;

public sealed class NpcDialogueServiceTests
{
    [Fact]
    public void SelectLine_WhenEntryMatchesRequestAndState_ReturnsTextAndSetsFlags()
    {
        var state = new NarrativeState();
        var history = new RecentSelectionHistory();
        var service = CreateService(
        [
            CreateEntry("line_1", "Hello there.", setFlags: [NarrativeIds.FlagMetTownsfolkOne])
        ]);

        var line = service.SelectLine(
            new NpcDialogueRequest(NarrativeIds.SpeakerTownsfolkOne, "greeting"),
            state,
            history);

        Assert.Equal("line_1", line.Id);
        Assert.Equal("Hello there.", line.Text);
        Assert.True(state.HasFlag(NarrativeIds.FlagMetTownsfolkOne));
        Assert.Contains("line_1", history.GetRecentIds(NpcDialogueService.HistorySystemName));
    }

    [Fact]
    public void SelectLine_WhenFreshMatchExists_SuppressesRecentEntry()
    {
        var state = new NarrativeState();
        var history = new RecentSelectionHistory();
        history.Record(NpcDialogueService.HistorySystemName, "recent_line");
        var service = CreateService(
        [
            CreateEntry("recent_line", "Already said.", weight: 100),
            CreateEntry("fresh_line", "Fresh line.", weight: 1)
        ]);

        var line = service.SelectLine(
            new NpcDialogueRequest(NarrativeIds.SpeakerTownsfolkOne, "greeting"),
            state,
            history);

        Assert.Equal("fresh_line", line.Id);
        Assert.Equal(["recent_line", "fresh_line"], service.LastDebugInfo.MatchedEntryIds);
        Assert.Equal(["recent_line"], service.LastDebugInfo.SuppressedEntryIds);
        Assert.Equal("fresh_line", service.LastDebugInfo.SelectedEntryId);
        Assert.Equal(string.Empty, service.LastDebugInfo.FallbackReason);
    }

    [Fact]
    public void SelectLine_WhenNoEntryMatches_ReturnsFallback()
    {
        var service = CreateService([]);

        var line = service.SelectLine(
            new NpcDialogueRequest(NarrativeIds.SpeakerTownsfolkOne, "greeting"),
            new NarrativeState(),
            new RecentSelectionHistory());

        Assert.Equal("dialogue_fallback", line.Id);
        Assert.Equal(NarrativeIds.SpeakerTownsfolkOne, line.SpeakerId);
        Assert.Empty(service.LastDebugInfo.MatchedEntryIds);
        Assert.Empty(service.LastDebugInfo.SuppressedEntryIds);
        Assert.Equal("dialogue_fallback", service.LastDebugInfo.SelectedEntryId);
        Assert.Contains("No dialogue entries matched", service.LastDebugInfo.FallbackReason);
    }

    [Fact]
    public void SelectLine_WhenAllMatchesWereRecent_RecordsFallbackReasonForFullPool()
    {
        var state = new NarrativeState();
        var history = new RecentSelectionHistory();
        history.Record(NpcDialogueService.HistorySystemName, "recent_line");
        var service = CreateService(
        [
            CreateEntry("recent_line", "Already said.")
        ]);

        var line = service.SelectLine(
            new NpcDialogueRequest(NarrativeIds.SpeakerTownsfolkOne, "greeting"),
            state,
            history);

        Assert.Equal("recent_line", line.Id);
        Assert.Equal(["recent_line"], service.LastDebugInfo.MatchedEntryIds);
        Assert.Equal(["recent_line"], service.LastDebugInfo.SuppressedEntryIds);
        Assert.Equal("recent_line", service.LastDebugInfo.SelectedEntryId);
        Assert.Contains("full pool", service.LastDebugInfo.FallbackReason);
    }

    private static NpcDialogueService CreateService(NpcDialogueEntry[] entries)
    {
        return new NpcDialogueService(
            new NpcDialogueDataFile { Entries = entries.ToList() },
            new WeightedRandomSelector(new Random(1)),
            new RecentSelectionHistory());
    }

    private static NpcDialogueEntry CreateEntry(
        string id,
        string text,
        int weight = 1,
        string[]? setFlags = null)
    {
        return new NpcDialogueEntry
        {
            Id = id,
            SpeakerId = NarrativeIds.SpeakerTownsfolkOne,
            SpeakerName = "Townsfolk",
            QuestId = NarrativeIds.QuestTownIntroductions,
            ObjectiveId = NarrativeIds.ObjectiveMeetTownsfolk,
            LineStyle = "greeting",
            Weight = weight,
            SetFlags = setFlags?.ToList() ?? [],
            Text = text
        };
    }
}
