namespace MyGame.Gameplay.World;

public sealed class EnemySpawnMap
{
    public EnemySpawnMap(IEnumerable<EnemySpawnDefinition> spawns)
    {
        Spawns = spawns.ToArray();
    }

    public IReadOnlyList<EnemySpawnDefinition> Spawns { get; }
}
