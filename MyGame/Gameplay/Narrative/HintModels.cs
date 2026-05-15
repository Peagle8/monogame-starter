namespace MyGame.Gameplay.Narrative;

public sealed class HintDataFile
{
    public List<HintEntry> Entries { get; set; } = [];
}

public sealed class HintEntry
{
    public string Id { get; set; } = string.Empty;

    public string ZoneId { get; set; } = string.Empty;

    public string ObjectiveId { get; set; } = string.Empty;

    public List<string> RequiredFlags { get; set; } = [];

    public List<string> ExcludedFlags { get; set; } = [];

    public int Priority { get; set; }

    public string Text { get; set; } = string.Empty;
}

public sealed record HintLine(string Id, string Text);
