using System.Collections.Generic;

namespace JsonDrivenGameSample.Gameplay.Journal
{
    public sealed class JournalDataFile
    {
        public List<JournalTemplateEntry> Entries { get; set; } = new();
    }

    public sealed class JournalTemplateEntry
    {
        public string Id { get; set; } = "";
        public List<string> RequiredFlags { get; set; } = new();
        public int Priority { get; set; }
        public string Title { get; set; } = "";
        public string Summary { get; set; } = "";
    }
}
