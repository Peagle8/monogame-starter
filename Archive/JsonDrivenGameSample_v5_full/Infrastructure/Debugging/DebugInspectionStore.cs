using System.Collections.Generic;

namespace JsonDrivenGameSample.Infrastructure.Debugging
{
    public sealed class DebugInspectionStore
    {
        private readonly List<MatchDebugInfo> _entries = new();

        public IReadOnlyList<MatchDebugInfo> Entries => _entries;

        public void Record(MatchDebugInfo info)
        {
            _entries.Add(info);
        }

        public void Clear()
        {
            _entries.Clear();
        }
    }
}
