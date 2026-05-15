namespace MyGame.Gameplay.Narrative;

public sealed record NpcDialogueDebugInfo(
    string SpeakerId,
    string LineStyle,
    IReadOnlyList<string> MatchedEntryIds,
    IReadOnlyList<string> SuppressedEntryIds,
    string SelectedEntryId,
    string FallbackReason)
{
    public static readonly NpcDialogueDebugInfo Empty = new(
        string.Empty,
        string.Empty,
        [],
        [],
        string.Empty,
        string.Empty);
}
