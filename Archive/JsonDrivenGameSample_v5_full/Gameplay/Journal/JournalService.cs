using System.Linq;
using JsonDrivenGameSample.Core;
using JsonDrivenGameSample.Infrastructure.Debugging;
using JsonDrivenGameSample.Infrastructure.History;

namespace JsonDrivenGameSample.Gameplay.Journal
{
    public sealed class JournalService
    {
        private const string SystemName = "Journal";

        private readonly JournalDataFile _data;
        private readonly DebugInspectionStore _debugStore;
        private readonly RecentSelectionHistory _history;

        public JournalService(JournalDataFile data, DebugInspectionStore debugStore, RecentSelectionHistory history)
        {
            _data = data;
            _debugStore = debugStore;
            _history = history;
        }

        public JournalTemplateEntry GetEntry(GameState gameState)
        {
            var matches = _data.Entries
                .Where(x => x.RequiredFlags.All(gameState.Flags.Contains))
                .OrderByDescending(x => x.Priority)
                .ToList();

            if (matches.Count == 0)
            {
                JournalTemplateEntry fallback = new()
                {
                    Id = "fallback",
                    Title = "Journey Continues",
                    Summary = "You continue gathering clues about the trouble spreading through the region."
                };

                _debugStore.Record(new MatchDebugInfo
                {
                    SystemName = SystemName,
                    FallbackReason = "No journal templates matched the current flag state.",
                    SelectedEntryId = fallback.Id,
                    ResultText = fallback.Summary
                });

                return fallback;
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
                ResultText = selected.Summary,
                FallbackReason = freshMatches.Count == 0
                    ? "All matching journal entries were recently used, so the full pool was allowed."
                    : ""
            });

            return selected;
        }
    }
}
