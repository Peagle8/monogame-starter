using MyGame.Gameplay.Player;

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
}
