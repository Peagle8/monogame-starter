namespace MyGame.Gameplay.Narrative;

public sealed class NpcDialogueService
{
    public const string HistorySystemName = "NpcDialogue";

    private static readonly NpcDialogueLine FallbackLine = new(
        "dialogue_fallback",
        "unknown",
        "Someone",
        "I have nothing to say yet.");

    private readonly NpcDialogueDataFile _data;
    private readonly WeightedRandomSelector _selector;
    private readonly RecentSelectionHistory _history;
    private NpcDialogueDebugInfo _lastDebugInfo = NpcDialogueDebugInfo.Empty;

    public NpcDialogueService(
        NpcDialogueDataFile data,
        WeightedRandomSelector selector,
        RecentSelectionHistory history)
    {
        _data = data;
        _selector = selector;
        _history = history;
    }

    public NpcDialogueDebugInfo LastDebugInfo => _lastDebugInfo;

    public NpcDialogueLine SelectLine(NpcDialogueRequest request, NarrativeState state)
    {
        return SelectLine(request, state, _history);
    }

    public NpcDialogueLine SelectLine(
        NpcDialogueRequest request,
        NarrativeState state,
        RecentSelectionHistory history)
    {
        var matches = _data.Entries
            .Where(entry => MatchesRequest(entry, request))
            .Where(entry => MatchesState(entry, state))
            .ToArray();

        if (matches.Length == 0)
        {
            var fallback = FallbackLine with { SpeakerId = request.SpeakerId };
            _lastDebugInfo = new NpcDialogueDebugInfo(
                request.SpeakerId,
                request.LineStyle,
                [],
                [],
                fallback.Id,
                "No dialogue entries matched the requested speaker, line style, quest, objective, and flags.");
            return fallback;
        }

        var freshMatches = matches
            .Where(entry => !history.WasRecentlyUsed(HistorySystemName, entry.Id))
            .ToArray();
        var suppressedIds = matches
            .Where(entry => history.WasRecentlyUsed(HistorySystemName, entry.Id))
            .Select(entry => entry.Id)
            .ToArray();
        var candidatePool = freshMatches.Length > 0 ? freshMatches : matches;
        var selected = _selector.Select(candidatePool, entry => entry.Weight);

        history.Record(HistorySystemName, selected.Id);
        foreach (var flagId in selected.SetFlags)
        {
            state.SetFlag(flagId);
        }

        _lastDebugInfo = new NpcDialogueDebugInfo(
            request.SpeakerId,
            request.LineStyle,
            matches.Select(entry => entry.Id).ToArray(),
            suppressedIds,
            selected.Id,
            freshMatches.Length == 0
                ? "All matching dialogue entries were recently used, so the full pool was allowed."
                : string.Empty);

        return new NpcDialogueLine(
            selected.Id,
            selected.SpeakerId,
            selected.SpeakerName,
            selected.Text,
            selected.CanDeliverHint);
    }

    private static bool MatchesRequest(NpcDialogueEntry entry, NpcDialogueRequest request)
    {
        return entry.SpeakerId.Equals(request.SpeakerId, StringComparison.OrdinalIgnoreCase)
            && entry.LineStyle.Equals(request.LineStyle, StringComparison.OrdinalIgnoreCase);
    }

    private static bool MatchesState(NpcDialogueEntry entry, NarrativeState state)
    {
        return entry.QuestId.Equals(state.ActiveQuestId, StringComparison.OrdinalIgnoreCase)
            && entry.ObjectiveId.Equals(state.ActiveObjectiveId, StringComparison.OrdinalIgnoreCase)
            && entry.RequiredFlags.All(state.HasFlag)
            && !entry.ExcludedFlags.Any(state.HasFlag);
    }
}
