using System.Linq;
using JsonDrivenGameSample.Core;
using JsonDrivenGameSample.Infrastructure.Debugging;
using JsonDrivenGameSample.Infrastructure.History;

namespace JsonDrivenGameSample.Gameplay.Hints
{
    public sealed class HintService
    {
        private const string SystemName = "Hints";

        private readonly HintDataFile _data;
        private readonly DebugInspectionStore _debugStore;
        private readonly RecentSelectionHistory _history;

        public HintService(HintDataFile data, DebugInspectionStore debugStore, RecentSelectionHistory history)
        {
            _data = data;
            _debugStore = debugStore;
            _history = history;
        }

        public string GetHint(GameState gameState, string objectiveId)
        {
            var matches = _data.Entries
                .Where(x => x.ZoneId == gameState.CurrentZoneId)
                .Where(x => x.ObjectiveId == objectiveId)
                .Where(x => x.RequiredFlags.All(gameState.Flags.Contains))
                .Where(x => x.ExcludedFlags.All(flag => !gameState.Flags.Contains(flag)))
                .OrderByDescending(x => x.Priority)
                .ToList();

            if (matches.Count == 0)
            {
                _debugStore.Record(new MatchDebugInfo
                {
                    SystemName = SystemName,
                    FallbackReason = "No hint entries matched the current zone, objective, and flag state.",
                    ResultText = "Search the area carefully. You may already have what you need."
                });

                return "Search the area carefully. You may already have what you need.";
            }

            var freshMatches = matches
                .Where(x => !_history.WasRecentlyUsed(SystemName, x.Id))
                .ToList();

            var suppressedIds = matches
                .Where(x => _history.WasRecentlyUsed(SystemName, x.Id))
                .Select(x => x.Id)
                .ToList();

            var selected = (freshMatches.Count > 0 ? freshMatches : matches)
                .OrderByDescending(x => x.Priority)
                .First();

            _history.Record(SystemName, selected.Id);

            _debugStore.Record(new MatchDebugInfo
            {
                SystemName = SystemName,
                MatchedEntryIds = matches.Select(x => x.Id).ToList(),
                SuppressedEntryIds = suppressedIds,
                SelectedEntryId = selected.Id,
                ResultText = selected.Text,
                FallbackReason = freshMatches.Count == 0
                    ? "All matching hint entries were recently used, so the full pool was allowed."
                    : ""
            });

            return selected.Text;
        }
    }
}
