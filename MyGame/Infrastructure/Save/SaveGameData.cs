using MyGame.Gameplay.Player;
using MyGame.Gameplay.Narrative;

namespace MyGame.Infrastructure.Save;

public sealed class SaveGameData
{
    public required string SceneName { get; init; }

    public required float PlayerPositionX { get; init; }

    public required float PlayerPositionY { get; init; }

    public required int PlayerHealth { get; init; }

    public required float PlayerAbilityPoints { get; init; }

    public PlayerAbility[] UnlockedAbilities { get; init; } = [];

    public PlayerDashAbilityKind EquippedDashAbility { get; init; } = PlayerDashAbilityKind.BaseDash;

    public PlayerDefenseAbilityKind EquippedDefenseAbility { get; init; } = PlayerDefenseAbilityKind.Shield;

    public PlayerRangedAttackKind EquippedRangedAbility { get; init; } = PlayerRangedAttackKind.Fireball;

    public PlayerMeleeAbilityKind EquippedMeleeAbility { get; init; } = PlayerMeleeAbilityKind.BaseAttack;

    public required int DefeatedEnemyCount { get; init; }

    public required EnemySaveData[] Enemies { get; init; }

    public string NarrativeLocale { get; init; } = "en-US";

    public string ActiveQuestId { get; init; } = "town_introductions";

    public string ActiveObjectiveId { get; init; } = "meet_townsfolk";

    public TownAlertLevel TownAlertLevel { get; init; } = TownAlertLevel.Calm;

    public int PlayerReputation { get; init; }

    public string[] NarrativeFlags { get; init; } = [];

    public string[] RecentDialogueIds { get; init; } = [];

    public string[] RecentHintIds { get; init; } = [];

    public string[] DiscoveredJournalEntryIds { get; init; } = [];

    public string[] ReadJournalEntryIds { get; init; } = [];
}
