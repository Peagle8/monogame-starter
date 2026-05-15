namespace MyGame.Gameplay.Narrative;

public sealed record NpcDialogueState(
    bool IsPromptVisible,
    bool IsOpen,
    string? SpeakerId,
    string SpeakerName,
    string Text,
    string HintText)
{
    public static readonly NpcDialogueState Default = new(false, false, null, string.Empty, string.Empty, string.Empty);
}
