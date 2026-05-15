namespace MyGame.Gameplay.Narrative;

public sealed class JournalState
{
    private readonly HashSet<string> _discoveredEntryIds = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _readEntryIds = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyCollection<string> DiscoveredEntryIds => _discoveredEntryIds;

    public IReadOnlyCollection<string> ReadEntryIds => _readEntryIds;

    public bool IsDiscovered(string entryId)
    {
        return _discoveredEntryIds.Contains(entryId);
    }

    public bool IsRead(string entryId)
    {
        return _readEntryIds.Contains(entryId);
    }

    public void Discover(string entryId)
    {
        if (!string.IsNullOrWhiteSpace(entryId))
        {
            _discoveredEntryIds.Add(entryId);
        }
    }

    public void MarkRead(string entryId)
    {
        if (IsDiscovered(entryId))
        {
            _readEntryIds.Add(entryId);
        }
    }

    public void Replace(IEnumerable<string> discoveredEntryIds, IEnumerable<string> readEntryIds)
    {
        _discoveredEntryIds.Clear();
        _readEntryIds.Clear();

        foreach (var entryId in discoveredEntryIds)
        {
            Discover(entryId);
        }

        foreach (var entryId in readEntryIds)
        {
            MarkRead(entryId);
        }
    }
}
