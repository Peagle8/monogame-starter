namespace MyGame.Gameplay.Narrative;

public sealed class JournalDataFile
{
    public List<JournalEntryTemplate> Entries { get; set; } = [];
}

public sealed class JournalEntryTemplate
{
    public string Id { get; set; } = string.Empty;

    public List<string> RequiredFlags { get; set; } = [];

    public int Priority { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;
}

public sealed record JournalEntry(string Id, string Title, string Summary, int Priority);
