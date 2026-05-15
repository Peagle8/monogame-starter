namespace MyGame.Gameplay.Narrative;

public sealed class NarrativeState
{
    public const int MinimumReputation = -100;
    public const int MaximumReputation = 100;

    private readonly HashSet<string> _flags = new(StringComparer.OrdinalIgnoreCase);

    public string Locale { get; private set; } = NarrativeIds.LocaleEnglishUnitedStates;

    public string ActiveQuestId { get; private set; } = NarrativeIds.QuestTownIntroductions;

    public string ActiveObjectiveId { get; private set; } = NarrativeIds.ObjectiveMeetTownsfolk;

    public TownAlertLevel TownAlertLevel { get; private set; } = TownAlertLevel.Calm;

    public int PlayerReputation { get; private set; }

    public IReadOnlyCollection<string> Flags => _flags;

    public void SetLocale(string locale)
    {
        if (!string.IsNullOrWhiteSpace(locale))
        {
            Locale = locale;
        }
    }

    public void SetProgress(string questId, string objectiveId)
    {
        if (!string.IsNullOrWhiteSpace(questId))
        {
            ActiveQuestId = questId;
        }

        if (!string.IsNullOrWhiteSpace(objectiveId))
        {
            ActiveObjectiveId = objectiveId;
        }
    }

    public void SetTownState(TownAlertLevel alertLevel, int playerReputation)
    {
        TownAlertLevel = alertLevel;
        PlayerReputation = Math.Clamp(playerReputation, MinimumReputation, MaximumReputation);
    }

    public void AdjustPlayerReputation(int delta)
    {
        PlayerReputation = Math.Clamp(
            PlayerReputation + delta,
            MinimumReputation,
            MaximumReputation);
    }

    public bool HasFlag(string flagId)
    {
        return _flags.Contains(flagId);
    }

    public void SetFlag(string flagId)
    {
        if (!string.IsNullOrWhiteSpace(flagId))
        {
            _flags.Add(flagId);
        }
    }

    public void ReplaceFlags(IEnumerable<string> flagIds)
    {
        _flags.Clear();

        foreach (var flagId in flagIds)
        {
            SetFlag(flagId);
        }
    }
}
