using System;
using System.Collections.Generic;
using System.Linq;

namespace JsonDrivenGameSample.Infrastructure.History
{
    public sealed class RecentSelectionHistory
    {
        private readonly Dictionary<string, Queue<string>> _history = new(StringComparer.OrdinalIgnoreCase);
        private readonly int _maxItemsPerSystem;

        public RecentSelectionHistory(int maxItemsPerSystem)
        {
            _maxItemsPerSystem = maxItemsPerSystem;
        }

        public IReadOnlyCollection<string> GetRecentIds(string systemName)
        {
            if (_history.TryGetValue(systemName, out Queue<string>? queue))
            {
                return queue.ToList();
            }

            return Array.Empty<string>();
        }

        public bool WasRecentlyUsed(string systemName, string id)
        {
            if (_history.TryGetValue(systemName, out Queue<string>? queue))
            {
                return queue.Contains(id);
            }

            return false;
        }

        public void Record(string systemName, string id)
        {
            if (!_history.TryGetValue(systemName, out Queue<string>? queue))
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
    }
}
