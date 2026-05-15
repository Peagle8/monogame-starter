namespace MyGame.Gameplay.Narrative;

public sealed class JournalService
{
    private readonly JournalDataFile _data;

    public JournalService(JournalDataFile data)
    {
        _data = data;
    }

    public IReadOnlyList<JournalEntry> GetAvailableEntries(NarrativeState narrativeState)
    {
        return _data.Entries
            .Where(entry => entry.RequiredFlags.All(narrativeState.HasFlag))
            .OrderByDescending(entry => entry.Priority)
            .Select(CreateEntry)
            .ToArray();
    }

    public IReadOnlyList<JournalEntry> DiscoverAvailableEntries(
        NarrativeState narrativeState,
        JournalState journalState)
    {
        var newlyDiscovered = new List<JournalEntry>();
        foreach (var entry in GetAvailableEntries(narrativeState))
        {
            if (journalState.IsDiscovered(entry.Id))
            {
                continue;
            }

            journalState.Discover(entry.Id);
            newlyDiscovered.Add(entry);
        }

        return newlyDiscovered;
    }

    public IReadOnlyList<JournalEntry> GetDiscoveredEntries(JournalState journalState)
    {
        return _data.Entries
            .Where(entry => journalState.IsDiscovered(entry.Id))
            .OrderByDescending(entry => entry.Priority)
            .Select(CreateEntry)
            .ToArray();
    }

    private static JournalEntry CreateEntry(JournalEntryTemplate template)
    {
        return new JournalEntry(template.Id, template.Title, template.Summary, template.Priority);
    }
}
