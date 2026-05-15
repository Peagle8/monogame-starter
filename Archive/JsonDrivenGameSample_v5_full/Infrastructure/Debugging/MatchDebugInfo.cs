using System.Collections.Generic;

namespace JsonDrivenGameSample.Infrastructure.Debugging
{
    public sealed class MatchDebugInfo
    {
        public string SystemName { get; init; } = "";
        public List<string> MatchedEntryIds { get; init; } = new();
        public List<string> SuppressedEntryIds { get; init; } = new();
        public string SelectedEntryId { get; init; } = "";
        public string ResultText { get; init; } = "";
        public string FallbackReason { get; init; } = "";
    }
}
