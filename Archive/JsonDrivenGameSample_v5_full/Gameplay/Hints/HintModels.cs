using System.Collections.Generic;

namespace JsonDrivenGameSample.Gameplay.Hints
{
    public sealed class HintDataFile
    {
        public List<HintEntry> Entries { get; set; } = new();
    }

    public sealed class HintEntry
    {
        public string Id { get; set; } = "";
        public string ZoneId { get; set; } = "";
        public string ObjectiveId { get; set; } = "";
        public List<string> RequiredFlags { get; set; } = new();
        public List<string> ExcludedFlags { get; set; } = new();
        public int Priority { get; set; }
        public string Text { get; set; } = "";
    }
}
