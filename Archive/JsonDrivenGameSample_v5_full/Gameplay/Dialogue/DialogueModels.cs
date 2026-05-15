using System.Collections.Generic;

namespace JsonDrivenGameSample.Gameplay.Dialogue
{
    public sealed class DialogueDataFile
    {
        public List<DialogueEntry> Entries { get; set; } = new();
    }

    public sealed class DialogueEntry
    {
        public string Id { get; set; } = "";
        public string SpeakerRole { get; set; } = "";
        public string Mood { get; set; } = "";
        public string QuestId { get; set; } = "";
        public string LineStyle { get; set; } = "";
        public int Weight { get; set; }
        public string Text { get; set; } = "";
    }
}
