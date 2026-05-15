namespace MyGame.Gameplay.Narrative;

public sealed class HintService
{
    public const string HistorySystemName = "Hints";

    private static readonly HintLine FallbackHint = new(
        "hint_fallback",
        "Search the area carefully. You may already have what you need.");

    private readonly HintDataFile _data;
    private readonly RecentSelectionHistory _history;

    public HintService(HintDataFile data, RecentSelectionHistory history)
    {
        _data = data;
        _history = history;
    }

    public HintLine SelectHint(string zoneId, NarrativeState state)
    {
        return SelectHint(zoneId, state, _history);
    }

    public HintLine SelectHint(string zoneId, NarrativeState state, RecentSelectionHistory history)
    {
        var matches = _data.Entries
            .Where(entry => MatchesState(entry, zoneId, state))
            .OrderByDescending(entry => entry.Priority)
            .ToArray();

        if (matches.Length == 0)
        {
            return FallbackHint;
        }

        var freshMatches = matches
            .Where(entry => !history.WasRecentlyUsed(HistorySystemName, entry.Id))
            .ToArray();
        var candidatePool = freshMatches.Length > 0 ? freshMatches : matches;
        var selected = candidatePool
            .OrderByDescending(entry => entry.Priority)
            .First();

        history.Record(HistorySystemName, selected.Id);
        return new HintLine(selected.Id, selected.Text);
    }

    private static bool MatchesState(HintEntry entry, string zoneId, NarrativeState state)
    {
        return entry.ZoneId.Equals(zoneId, StringComparison.OrdinalIgnoreCase)
            && entry.ObjectiveId.Equals(state.ActiveObjectiveId, StringComparison.OrdinalIgnoreCase)
            && entry.RequiredFlags.All(state.HasFlag)
            && !entry.ExcludedFlags.Any(state.HasFlag);
    }
}
