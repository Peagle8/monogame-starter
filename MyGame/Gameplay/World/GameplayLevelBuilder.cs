using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.Props;

namespace MyGame.Gameplay.World;

public sealed class GameplayLevelBuilder
{
    private readonly EnemySettings _defaultEnemySettings;
    private readonly IEnemyFactory _enemyFactory;
    private readonly IEnemySettingsCatalog _enemySettingsCatalog;
    private readonly WorldCombatSettings _worldCombatSettings;
    private readonly PlayerAttackHitResolver _playerAttackHitResolver;
    private readonly PlayerProjectileResolver _playerProjectileResolver;
    private readonly WorldObstacleResolver _worldObstacleResolver;
    private readonly EnemySeparationResolver _enemySeparationResolver;
    private readonly EnemyContactResolver _enemyContactResolver;

    public GameplayLevelBuilder(
        EnemySettings defaultEnemySettings,
        IEnemyFactory enemyFactory,
        IEnemySettingsCatalog enemySettingsCatalog,
        WorldCombatSettings worldCombatSettings,
        PlayerAttackHitResolver playerAttackHitResolver,
        PlayerProjectileResolver playerProjectileResolver,
        WorldObstacleResolver worldObstacleResolver,
        EnemySeparationResolver enemySeparationResolver,
        EnemyContactResolver enemyContactResolver)
    {
        _defaultEnemySettings = defaultEnemySettings;
        _enemyFactory = enemyFactory;
        _enemySettingsCatalog = enemySettingsCatalog;
        _worldCombatSettings = worldCombatSettings;
        _playerAttackHitResolver = playerAttackHitResolver;
        _playerProjectileResolver = playerProjectileResolver;
        _worldObstacleResolver = worldObstacleResolver;
        _enemySeparationResolver = enemySeparationResolver;
        _enemyContactResolver = enemyContactResolver;
    }

    public World BuildDefaultLevel(PlayerActor player)
    {
        IWorldProp[] props =
        [
            new TreeProp(new Vector2(120f, 120f), new Point(72, 104)),
            new TreeProp(new Vector2(560f, 160f), new Point(64, 96)),
            new TreeProp(new Vector2(620f, 320f), new Point(80, 112)),
            new GrassProp(new Vector2(188f, 180f), new Point(52, 36)),
            new GrassProp(new Vector2(308f, 132f), new Point(44, 28)),
            new GrassProp(new Vector2(500f, 360f), new Point(56, 34))
        ];

        var spawnMap = new EnemySpawnMap(
        [
            new EnemySpawnDefinition(EnemyKind.HornedRabbit, new Vector2(560f, 180f), EnemyAxisPreference.Horizontal),
            new EnemySpawnDefinition(EnemyKind.HornedRabbit, new Vector2(660f, 240f), EnemyAxisPreference.Vertical),
            new EnemySpawnDefinition(EnemyKind.HornedRabbit, new Vector2(560f, 320f), EnemyAxisPreference.Horizontal),
            new EnemySpawnDefinition(EnemyKind.HornedRabbit, new Vector2(720f, 320f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Bat, new Vector2(460f, 120f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Bat, new Vector2(760f, 160f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Grasshopper, new Vector2(500f, 260f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Grasshopper, new Vector2(700f, 220f), EnemyAxisPreference.None)
        ]);

        return new World(
            player,
            props,
            spawnMap.Spawns.Select(_enemyFactory.Create),
            _defaultEnemySettings,
            _enemySettingsCatalog,
            _enemyFactory,
            _playerAttackHitResolver,
            _playerProjectileResolver,
            _worldObstacleResolver,
            _enemySeparationResolver,
            _enemyContactResolver,
            _worldCombatSettings);
    }
}
