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
        return BuildOverworld(player);
    }

    public World BuildOverworld(PlayerActor player)
    {
        var shopDoorBounds = new Rectangle(376, 186, 48, 46);
        IWorldProp[] props =
        [
            new ShopExteriorProp(new Vector2(296f, 56f), new Point(208, 176), shopDoorBounds),
            new WallProp(new Vector2(312f, 92f), new Point(176, 36)),
            new WallProp(new Vector2(312f, 128f), new Point(32, 104)),
            new WallProp(new Vector2(456f, 128f), new Point(32, 104)),
            new WallProp(new Vector2(344f, 200f), new Point(32, 32)),
            new WallProp(new Vector2(424f, 200f), new Point(32, 32)),
            new TreeProp(new Vector2(120f, 120f), new Point(72, 104)),
            new TreeProp(new Vector2(560f, 160f), new Point(64, 96)),
            new TreeProp(new Vector2(620f, 320f), new Point(80, 112)),
            new GrassProp(new Vector2(188f, 180f), new Point(52, 36)),
            new GrassProp(new Vector2(308f, 132f), new Point(44, 28)),
            new GrassProp(new Vector2(500f, 360f), new Point(56, 34))
        ];

        var spawnMap = new EnemySpawnMap(
        [
            new EnemySpawnDefinition(EnemyKind.HornedRabbit, new Vector2(660f, 180f), EnemyAxisPreference.Horizontal),
            new EnemySpawnDefinition(EnemyKind.HornedRabbit, new Vector2(760f, 240f), EnemyAxisPreference.Vertical),
            new EnemySpawnDefinition(EnemyKind.HornedRabbit, new Vector2(660f, 340f), EnemyAxisPreference.Horizontal),
            new EnemySpawnDefinition(EnemyKind.HornedRabbit, new Vector2(820f, 340f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Bat, new Vector2(760f, 120f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Bat, new Vector2(860f, 180f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Grasshopper, new Vector2(700f, 280f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Grasshopper, new Vector2(860f, 260f), EnemyAxisPreference.None)
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
            _worldCombatSettings,
            [
                new WorldSceneTransition(
                    new Rectangle(384, 196, 32, 24),
                    Scenes.Gameplay.GameplaySceneNames.ShopInterior,
                    new Vector2(384f, 304f))
            ]);
    }

    // TODO: this type is also going to get crowded over time and really be an issue, eventually we should break these out into their own types
    public World BuildShopInterior(PlayerActor player)
    {
        IWorldProp[] props =
        [
            new WallProp(new Vector2(320f, 176f), new Point(24, 184)),
            new WallProp(new Vector2(456f, 176f), new Point(24, 184)),
            new WallProp(new Vector2(320f, 176f), new Point(160, 24)),
            new WallProp(new Vector2(320f, 336f), new Point(24, 24)),
            new WallProp(new Vector2(456f, 336f), new Point(24, 24)),
            new CounterProp(new Vector2(352f, 232f), new Point(96, 24)),
            new ShopkeeperProp(new Vector2(380f, 190f), new Point(40, 42))
        ];

        return new World(
            player,
            props,
            [],
            _defaultEnemySettings,
            _enemySettingsCatalog,
            _enemyFactory,
            _playerAttackHitResolver,
            _playerProjectileResolver,
            _worldObstacleResolver,
            _enemySeparationResolver,
            _enemyContactResolver,
            _worldCombatSettings,
            [
                new WorldSceneTransition(
                    new Rectangle(360, 342, 80, 18),
                    Scenes.Gameplay.GameplaySceneNames.Overworld,
                    new Vector2(384f, 240f))
            ]);
    }
}
