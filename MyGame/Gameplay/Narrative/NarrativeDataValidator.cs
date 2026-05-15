namespace MyGame.Gameplay.Narrative;

public sealed class NarrativeDataValidator
{
    private readonly HashSet<string> _questIds = new(StringComparer.OrdinalIgnoreCase)
    {
        NarrativeIds.QuestTownIntroductions
    };

    private readonly HashSet<string> _objectiveIds = new(StringComparer.OrdinalIgnoreCase)
    {
        NarrativeIds.ObjectiveMeetTownsfolk
    };

    private readonly HashSet<string> _zoneIds = new(StringComparer.OrdinalIgnoreCase)
    {
        NarrativeIds.ZoneOverworld,
        NarrativeIds.ZoneShopInterior,
        NarrativeIds.ZoneArena,
        NarrativeIds.ZoneWildernessNorth,
        NarrativeIds.ZoneWildernessSouth,
        NarrativeIds.ZoneWildernessEast,
        NarrativeIds.ZoneWildernessWest
    };

    private readonly HashSet<string> _flagIds = new(StringComparer.OrdinalIgnoreCase)
    {
        NarrativeIds.FlagMetShopkeeper,
        NarrativeIds.FlagMetTownsfolkOne,
        NarrativeIds.FlagMetTownsfolkTwo
    };

    public void Validate(NpcDialogueDataFile file)
    {
        ValidateUniqueIds(file.Entries.Select(entry => entry.Id), "dialogue");

        foreach (var entry in file.Entries)
        {
            ValidateRequiredFields(entry);
            ValidateKnownReference(_questIds, entry.QuestId, entry.Id, "quest");
            ValidateKnownReference(_objectiveIds, entry.ObjectiveId, entry.Id, "objective");
            ValidateKnownFlags(entry.RequiredFlags, entry.Id, "required");
            ValidateKnownFlags(entry.ExcludedFlags, entry.Id, "excluded");
            ValidateKnownFlags(entry.SetFlags, entry.Id, "set");

            if (entry.Weight <= 0)
            {
                throw new InvalidOperationException($"Dialogue entry '{entry.Id}' must have a weight greater than zero.");
            }
        }
    }

    public void Validate(HintDataFile file)
    {
        ValidateUniqueIds(file.Entries.Select(entry => entry.Id), "hints");

        foreach (var entry in file.Entries)
        {
            ValidateRequiredFields(entry);
            ValidateKnownReference(_zoneIds, entry.ZoneId, entry.Id, "zone", "Hint");
            ValidateKnownReference(_objectiveIds, entry.ObjectiveId, entry.Id, "objective", "Hint");
            ValidateKnownFlags(entry.RequiredFlags, entry.Id, "required", "Hint");
            ValidateKnownFlags(entry.ExcludedFlags, entry.Id, "excluded", "Hint");
        }
    }

    public void Validate(JournalDataFile file)
    {
        ValidateUniqueIds(file.Entries.Select(entry => entry.Id), "journal");

        foreach (var entry in file.Entries)
        {
            ValidateRequiredFields(entry);
            ValidateKnownFlags(entry.RequiredFlags, entry.Id, "required", "Journal");
        }
    }

    private static void ValidateRequiredFields(NpcDialogueEntry entry)
    {
        if (string.IsNullOrWhiteSpace(entry.Id)
            || string.IsNullOrWhiteSpace(entry.SpeakerId)
            || string.IsNullOrWhiteSpace(entry.SpeakerName)
            || string.IsNullOrWhiteSpace(entry.QuestId)
            || string.IsNullOrWhiteSpace(entry.ObjectiveId)
            || string.IsNullOrWhiteSpace(entry.LineStyle)
            || string.IsNullOrWhiteSpace(entry.Text))
        {
            throw new InvalidOperationException($"Dialogue entry '{entry.Id}' has one or more required empty fields.");
        }
    }

    private static void ValidateRequiredFields(HintEntry entry)
    {
        if (string.IsNullOrWhiteSpace(entry.Id)
            || string.IsNullOrWhiteSpace(entry.ZoneId)
            || string.IsNullOrWhiteSpace(entry.ObjectiveId)
            || string.IsNullOrWhiteSpace(entry.Text))
        {
            throw new InvalidOperationException($"Hint entry '{entry.Id}' has one or more required empty fields.");
        }
    }

    private static void ValidateRequiredFields(JournalEntryTemplate entry)
    {
        if (string.IsNullOrWhiteSpace(entry.Id)
            || string.IsNullOrWhiteSpace(entry.Title)
            || string.IsNullOrWhiteSpace(entry.Summary))
        {
            throw new InvalidOperationException($"Journal entry '{entry.Id}' has one or more required empty fields.");
        }
    }

    private void ValidateKnownFlags(
        IEnumerable<string> flagIds,
        string entryId,
        string area,
        string entryKind = "Dialogue")
    {
        foreach (var flagId in flagIds)
        {
            ValidateKnownReference(_flagIds, flagId, entryId, $"{area} flag", entryKind);
        }
    }

    private static void ValidateKnownReference(
        HashSet<string> knownIds,
        string id,
        string entryId,
        string referenceName,
        string entryKind = "Dialogue")
    {
        if (!knownIds.Contains(id))
        {
            throw new InvalidOperationException($"{entryKind} entry '{entryId}' references unknown {referenceName} id '{id}'.");
        }
    }

    private static void ValidateUniqueIds(IEnumerable<string> ids, string areaName)
    {
        var duplicates = ids
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .GroupBy(id => id, StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToArray();

        if (duplicates.Length > 0)
        {
            throw new InvalidOperationException($"Duplicate ids found in {areaName}: {string.Join(", ", duplicates)}");
        }
    }
}
