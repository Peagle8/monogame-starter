using System;
using System.Linq;
using JsonDrivenGameSample.Infrastructure.Debugging;
using JsonDrivenGameSample.Infrastructure.History;

namespace JsonDrivenGameSample.Gameplay.Dialogue
{
    public sealed class DialogueService
    {
        private const string SystemName = "Dialogue";

        private readonly DialogueDataFile _data;
        private readonly WeightedRandomSelector _selector;
        private readonly DebugInspectionStore _debugStore;
        private readonly RecentSelectionHistory _history;

        public DialogueService(
            DialogueDataFile data,
            WeightedRandomSelector selector,
            DebugInspectionStore debugStore,
            RecentSelectionHistory history)
        {
            _data = data;
            _selector = selector;
            _debugStore = debugStore;
            _history = history;
        }

        public string GetLine(string role, string mood, string questId, string style)
        {
            var matches = _data.Entries
                .Where(x => x.SpeakerRole.Equals(role, StringComparison.OrdinalIgnoreCase))
                .Where(x => x.Mood.Equals(mood, StringComparison.OrdinalIgnoreCase))
                .Where(x => x.QuestId.Equals(questId, StringComparison.OrdinalIgnoreCase))
                .Where(x => x.LineStyle.Equals(style, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (matches.Count == 0)
            {
                _debugStore.Record(new MatchDebugInfo
                {
                    SystemName = SystemName,
                    FallbackReason = "No dialogue entries matched the requested role, mood, quest, and line style.",
                    ResultText = "I have nothing to say."
                });

                return "I have nothing to say.";
            }

            var freshMatches = matches
                .Where(x => !_history.WasRecentlyUsed(SystemName, x.Id))
                .ToList();

            var suppressedIds = matches
                .Where(x => _history.WasRecentlyUsed(SystemName, x.Id))
                .Select(x => x.Id)
                .ToList();

            var candidatePool = freshMatches.Count > 0 ? freshMatches : matches;
            DialogueEntry selected = _selector.Select(candidatePool, x => x.Weight);

            _history.Record(SystemName, selected.Id);

            _debugStore.Record(new MatchDebugInfo
            {
                SystemName = SystemName,
                MatchedEntryIds = matches.Select(x => x.Id).ToList(),
                SuppressedEntryIds = suppressedIds,
                SelectedEntryId = selected.Id,
                ResultText = selected.Text,
                FallbackReason = freshMatches.Count == 0
                    ? "All matching dialogue entries were recently used, so the full pool was allowed."
                    : ""
            });

            return selected.Text;
        }
    }
}
