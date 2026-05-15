using MyGame.Gameplay.Narrative;

namespace MyGame.Tests.Gameplay.Narrative;

public sealed class NarrativeDataValidatorTests
{
    private readonly NarrativeDataValidator _validator = new();

    [Fact]
    public void Validate_WhenIdsAreDuplicated_Throws()
    {
        var file = new NpcDialogueDataFile
        {
            Entries =
            [
                CreateEntry("duplicate_id"),
                CreateEntry("DUPLICATE_ID")
            ]
        };

        var exception = Assert.Throws<InvalidOperationException>(() => _validator.Validate(file));
        Assert.Contains("Duplicate ids", exception.Message);
    }

    [Fact]
    public void Validate_WhenQuestIdIsUnknown_Throws()
    {
        var entry = CreateEntry("line_1");
        entry.QuestId = "missing_quest";
        var file = new NpcDialogueDataFile { Entries = [entry] };

        var exception = Assert.Throws<InvalidOperationException>(() => _validator.Validate(file));
        Assert.Contains("unknown quest", exception.Message);
    }

    [Fact]
    public void Validate_WhenFlagIdIsUnknown_Throws()
    {
        var entry = CreateEntry("line_1");
        entry.RequiredFlags = ["missing_flag"];
        var file = new NpcDialogueDataFile { Entries = [entry] };

        var exception = Assert.Throws<InvalidOperationException>(() => _validator.Validate(file));
        Assert.Contains("unknown required flag", exception.Message);
    }

    [Fact]
    public void Validate_WhenEntryIsValid_DoesNotThrow()
    {
        var file = new NpcDialogueDataFile { Entries = [CreateEntry("line_1")] };

        _validator.Validate(file);
    }

    [Fact]
    public void Validate_WhenHintZoneIdIsUnknown_Throws()
    {
        var entry = CreateHintEntry("hint_1");
        entry.ZoneId = "missing_zone";
        var file = new HintDataFile { Entries = [entry] };

        var exception = Assert.Throws<InvalidOperationException>(() => _validator.Validate(file));
        Assert.Contains("unknown zone", exception.Message);
    }

    [Fact]
    public void Validate_WhenHintEntryIsValid_DoesNotThrow()
    {
        var file = new HintDataFile { Entries = [CreateHintEntry("hint_1")] };

        _validator.Validate(file);
    }

    [Fact]
    public void Validate_WhenJournalFlagIdIsUnknown_Throws()
    {
        var entry = CreateJournalEntry("journal_1");
        entry.RequiredFlags = ["missing_flag"];
        var file = new JournalDataFile { Entries = [entry] };

        var exception = Assert.Throws<InvalidOperationException>(() => _validator.Validate(file));
        Assert.Contains("unknown required flag", exception.Message);
    }

    [Fact]
    public void Validate_WhenJournalEntryIsValid_DoesNotThrow()
    {
        var file = new JournalDataFile { Entries = [CreateJournalEntry("journal_1")] };

        _validator.Validate(file);
    }

    private static NpcDialogueEntry CreateEntry(string id)
    {
        return new NpcDialogueEntry
        {
            Id = id,
            SpeakerId = NarrativeIds.SpeakerTownsfolkOne,
            SpeakerName = "Townsfolk",
            QuestId = NarrativeIds.QuestTownIntroductions,
            ObjectiveId = NarrativeIds.ObjectiveMeetTownsfolk,
            LineStyle = "greeting",
            Weight = 1,
            Text = "Hello."
        };
    }

    private static HintEntry CreateHintEntry(string id)
    {
        return new HintEntry
        {
            Id = id,
            ZoneId = NarrativeIds.ZoneOverworld,
            ObjectiveId = NarrativeIds.ObjectiveMeetTownsfolk,
            Priority = 1,
            Text = "Try talking to someone."
        };
    }

    private static JournalEntryTemplate CreateJournalEntry(string id)
    {
        return new JournalEntryTemplate
        {
            Id = id,
            RequiredFlags = [NarrativeIds.FlagMetTownsfolkOne],
            Priority = 1,
            Title = "Town Notes",
            Summary = "A backend journal entry."
        };
    }
}
