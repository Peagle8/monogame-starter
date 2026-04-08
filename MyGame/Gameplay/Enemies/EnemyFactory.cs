using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Gameplay.World;
using MyGame.Infrastructure.Save;

namespace MyGame.Gameplay.Enemies;

public sealed class EnemyFactory : IEnemyFactory
{
    private readonly IEnemySettingsCatalog _enemySettingsCatalog;

    public EnemyFactory(IEnemySettingsCatalog enemySettingsCatalog)
    {
        _enemySettingsCatalog = enemySettingsCatalog;
    }

    public EnemyActor Create(EnemySpawnDefinition spawn)
    {
        return Create(spawn.Kind, spawn.Position, spawn.AxisPreference);
    }

    public EnemyActor CreateFromSaveData(EnemySaveData saveData)
    {
        var enemy = Create(
            saveData.Kind,
            new Vector2(saveData.PositionX, saveData.PositionY),
            saveData.AxisPreference);
        enemy.RestoreState(enemy.Position, saveData.CurrentHealth);
        return enemy;
    }

    public EnemyActor Create(EnemyKind kind, Vector2 position, EnemyAxisPreference axisPreference = EnemyAxisPreference.None)
    {
        var settings = _enemySettingsCatalog.Get(kind);
        var initialDashPauseSeconds = kind is EnemyKind.HornedRabbit or EnemyKind.Bat or EnemyKind.Grasshopper
            ? Random.Shared.NextSingle() * (settings.InitialDashPauseMaxSeconds - settings.InitialDashPauseMinSeconds)
                + settings.InitialDashPauseMinSeconds
            : 0f;

        return new EnemyActor(settings, position, initialDashPauseSeconds, axisPreference);
    }
}
