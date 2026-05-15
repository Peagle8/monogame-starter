namespace MyGame.Gameplay.Narrative;

public sealed class RecentSelectionHistory
{
    private readonly Dictionary<string, Queue<string>> _history = new(StringComparer.OrdinalIgnoreCase);
    private readonly int _maxItemsPerSystem;

    public RecentSelectionHistory(int maxItemsPerSystem = 3)
    {
        _maxItemsPerSystem = Math.Max(1, maxItemsPerSystem);
    }

    public IReadOnlyCollection<string> GetRecentIds(string systemName)
    {
        return _history.TryGetValue(systemName, out var queue)
            ? queue.ToArray()
            : [];
    }

    public bool WasRecentlyUsed(string systemName, string id)
    {
        return _history.TryGetValue(systemName, out var queue) && queue.Contains(id);
    }

    public void Record(string systemName, string id)
    {
        if (string.IsNullOrWhiteSpace(systemName) || string.IsNullOrWhiteSpace(id))
        {
            return;
        }

        if (!_history.TryGetValue(systemName, out var queue))
        {
            queue = new Queue<string>();
            _history[systemName] = queue;
        }

        queue.Enqueue(id);

        while (queue.Count > _maxItemsPerSystem)
        {
            queue.Dequeue();
        }
    }

    public void Replace(string systemName, IEnumerable<string> ids)
    {
        var queue = new Queue<string>();
        foreach (var id in ids.Where(id => !string.IsNullOrWhiteSpace(id)).TakeLast(_maxItemsPerSystem))
        {
            queue.Enqueue(id);
        }

        _history[systemName] = queue;
    }
}
