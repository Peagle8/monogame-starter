namespace MyGame.Infrastructure.Save;

public sealed class SaveGameData
{
    public required string SceneName { get; init; }

    public required float PlayerPositionX { get; init; }

    public required float PlayerPositionY { get; init; }

    public required int PlayerHealth { get; init; }

    public required float PlayerAbilityPoints { get; init; }

    public required int DefeatedEnemyCount { get; init; }

    public required EnemySaveData[] Enemies { get; init; }
}
