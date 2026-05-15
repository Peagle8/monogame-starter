namespace MyGame.Gameplay.Narrative;

public sealed class NpcDialogueDataFile
{
    public List<NpcDialogueEntry> Entries { get; set; } = [];
}

public sealed class NpcDialogueEntry
{
    public string Id { get; set; } = string.Empty;

    public string SpeakerId { get; set; } = string.Empty;

    public string SpeakerName { get; set; } = string.Empty;

    public string QuestId { get; set; } = string.Empty;

    public string ObjectiveId { get; set; } = string.Empty;

    public string LineStyle { get; set; } = "greeting";

    public int Weight { get; set; } = 1;

    public List<string> RequiredFlags { get; set; } = [];

    public List<string> ExcludedFlags { get; set; } = [];

    public List<string> SetFlags { get; set; } = [];

    public bool CanDeliverHint { get; set; }

    public string Text { get; set; } = string.Empty;
}

public sealed record NpcDialogueRequest(string SpeakerId, string LineStyle);

public sealed record NpcDialogueLine(
    string Id,
    string SpeakerId,
    string SpeakerName,
    string Text,
    bool CanDeliverHint = false);
