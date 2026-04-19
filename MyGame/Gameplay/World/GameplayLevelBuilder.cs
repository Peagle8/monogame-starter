using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.Props;
using MyGame.Scenes.Gameplay;

namespace MyGame.Gameplay.World;

public sealed class GameplayLevelBuilder
{
    private const float ArenaScale = 1.2f;

    private readonly EnemySettings _defaultEnemySettings;
    private readonly IEnemyFactory _enemyFactory;
    private readonly IEnemySettingsCatalog _enemySettingsCatalog;
    private readonly WorldCombatSettings _worldCombatSettings;
    private readonly PlayerAttackHitResolver _playerAttackHitResolver;
    private readonly PlayerBombResolver _playerBombResolver;
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
        : this(
            defaultEnemySettings,
            enemyFactory,
            enemySettingsCatalog,
            worldCombatSettings,
            playerAttackHitResolver,
            new PlayerBombResolver(),
            playerProjectileResolver,
            worldObstacleResolver,
            enemySeparationResolver,
            enemyContactResolver)
    {
    }

    public GameplayLevelBuilder(
        EnemySettings defaultEnemySettings,
        IEnemyFactory enemyFactory,
        IEnemySettingsCatalog enemySettingsCatalog,
        WorldCombatSettings worldCombatSettings,
        PlayerAttackHitResolver playerAttackHitResolver,
        PlayerBombResolver playerBombResolver,
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
        _playerBombResolver = playerBombResolver;
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
        var props = new List<IWorldProp>();
        var shopDoorBounds = new Rectangle(940, 1084, 32, 48);
        var arenaDoorBounds = new Rectangle(676, 1084, 40, 48);

        AddTownWalls(props);
        AddTownHouses(props);
        AddTownCentralDistrict(props, shopDoorBounds, arenaDoorBounds);
        AddTownDecor(props);

        return CreateWorld(
            player,
            props,
            [],
            BuildTownTransitions(shopDoorBounds, arenaDoorBounds),
            OverworldLayoutMetrics.TownBounds);
    }

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

        return CreateWorld(
            player,
            props,
            [],
            [
                new WorldSceneTransition(
                    new Rectangle(360, 342, 80, 18),
                    GameplaySceneNames.Overworld,
                    new Vector2(940f, 1144f))
            ]);
    }

    public World BuildArena(PlayerActor player)
    {
        IWorldProp[] props =
        [
            new ArenaBoundaryProp(new Vector2(0f, 0f), ScaleArenaPoint(800, 72)),
            new ArenaBoundaryProp(new Vector2(0f, 0f), ScaleArenaPoint(72, 480)),
            new ArenaBoundaryProp(ScaleArenaVector(728f, 0f), ScaleArenaPoint(72, 480)),
            new ArenaBoundaryProp(new Vector2(0f, ScaleArena(408)), ScaleArenaPoint(304, 72)),
            new ArenaBoundaryProp(ScaleArenaVector(496f, 408f), ScaleArenaPoint(304, 72)),
            new ArenaBoundaryProp(ScaleArenaVector(304f, 432f), ScaleArenaPoint(32, 48)),
            new ArenaBoundaryProp(ScaleArenaVector(464f, 432f), ScaleArenaPoint(32, 48))
        ];

        return CreateWorld(
            player,
            props,
            [],
            [
                new WorldSceneTransition(
                    ScaleArenaRectangle(320, 392, 160, 64),
                    GameplaySceneNames.Overworld,
                    new Vector2(676f, 1144f),
                    world => world.IsObjectiveComplete)
            ],
            new Rectangle(0, 0, ScaleArena(800), ScaleArena(480)),
            new ArenaEncounterController(
                _enemyFactory,
                true,
                BuildArenaWaveOneSpawns(),
                BuildArenaWaveTwoSpawns(),
                BuildArenaWaveThreeSpawns(),
                BuildArenaWaveFourSpawns()));
    }

    public World BuildWildernessNorth(PlayerActor player)
    {
        return BuildWilderness(player, WildernessSceneDefinitionFactory.CreateNorth());
    }

    public World BuildWildernessSouth(PlayerActor player)
    {
        return BuildWilderness(player, WildernessSceneDefinitionFactory.CreateSouth());
    }

    public World BuildWildernessWest(PlayerActor player)
    {
        return BuildWilderness(player, WildernessSceneDefinitionFactory.CreateWest());
    }

    public World BuildWildernessEast(PlayerActor player)
    {
        return BuildWilderness(player, WildernessSceneDefinitionFactory.CreateEast());
    }

    private World BuildWilderness(PlayerActor player, WildernessSceneDefinition definition)
    {
        return CreateWorld(
            player,
            definition.Props,
            definition.Spawns.Select(_enemyFactory.Create),
            definition.SceneTransitions,
            definition.Bounds);
    }

    private static IEnumerable<WorldSceneTransition> BuildTownTransitions(Rectangle shopDoorBounds, Rectangle arenaDoorBounds)
    {
        return
        [
            new WorldSceneTransition(
                shopDoorBounds,
                GameplaySceneNames.ShopInterior,
                new Vector2(384f, 304f)),
            new WorldSceneTransition(
                arenaDoorBounds,
                GameplaySceneNames.Arena,
                ScaleArenaVector(384f, 392f)),
            new WorldSceneTransition(
                OverworldLayoutMetrics.TownNorthGateTrigger,
                GameplaySceneNames.WildernessNorth,
                new Vector2(OverworldLayoutMetrics.TownNorthGateTrigger.X, OverworldLayoutMetrics.WildernessShortSize - 160f)),
            new WorldSceneTransition(
                OverworldLayoutMetrics.TownSouthGateTrigger,
                GameplaySceneNames.WildernessSouth,
                new Vector2(OverworldLayoutMetrics.TownSouthGateTrigger.X, 144f)),
            new WorldSceneTransition(
                OverworldLayoutMetrics.TownWestGateTrigger,
                GameplaySceneNames.WildernessWest,
                new Vector2(OverworldLayoutMetrics.WildernessShortSize - 160f, OverworldLayoutMetrics.TownWestGateTrigger.Y)),
            new WorldSceneTransition(
                OverworldLayoutMetrics.TownEastGateTrigger,
                GameplaySceneNames.WildernessEast,
                new Vector2(144f, OverworldLayoutMetrics.TownEastGateTrigger.Y))
        ];
    }

    private static void AddTownWalls(List<IWorldProp> props)
    {
        props.AddRange(
        [
            new ArenaBoundaryProp(new Vector2(0f, 0f), new Point((OverworldLayoutMetrics.TownBounds.Width - OverworldLayoutMetrics.TownGateWidth) / 2, OverworldLayoutMetrics.TownWallThickness)),
            new ArenaBoundaryProp(new Vector2(((OverworldLayoutMetrics.TownBounds.Width + OverworldLayoutMetrics.TownGateWidth) / 2), 0f), new Point((OverworldLayoutMetrics.TownBounds.Width - OverworldLayoutMetrics.TownGateWidth) / 2, OverworldLayoutMetrics.TownWallThickness)),
            new ArenaBoundaryProp(new Vector2(0f, OverworldLayoutMetrics.TownBounds.Bottom - OverworldLayoutMetrics.TownWallThickness), new Point((OverworldLayoutMetrics.TownBounds.Width - OverworldLayoutMetrics.TownGateWidth) / 2, OverworldLayoutMetrics.TownWallThickness)),
            new ArenaBoundaryProp(new Vector2(((OverworldLayoutMetrics.TownBounds.Width + OverworldLayoutMetrics.TownGateWidth) / 2), OverworldLayoutMetrics.TownBounds.Bottom - OverworldLayoutMetrics.TownWallThickness), new Point((OverworldLayoutMetrics.TownBounds.Width - OverworldLayoutMetrics.TownGateWidth) / 2, OverworldLayoutMetrics.TownWallThickness)),
            new ArenaBoundaryProp(new Vector2(0f, 0f), new Point(OverworldLayoutMetrics.TownWallThickness, (OverworldLayoutMetrics.TownBounds.Height - OverworldLayoutMetrics.TownGateWidth) / 2)),
            new ArenaBoundaryProp(new Vector2(0f, ((OverworldLayoutMetrics.TownBounds.Height + OverworldLayoutMetrics.TownGateWidth) / 2)), new Point(OverworldLayoutMetrics.TownWallThickness, (OverworldLayoutMetrics.TownBounds.Height - OverworldLayoutMetrics.TownGateWidth) / 2)),
            new ArenaBoundaryProp(new Vector2(OverworldLayoutMetrics.TownBounds.Right - OverworldLayoutMetrics.TownWallThickness, 0f), new Point(OverworldLayoutMetrics.TownWallThickness, (OverworldLayoutMetrics.TownBounds.Height - OverworldLayoutMetrics.TownGateWidth) / 2)),
            new ArenaBoundaryProp(new Vector2(OverworldLayoutMetrics.TownBounds.Right - OverworldLayoutMetrics.TownWallThickness, ((OverworldLayoutMetrics.TownBounds.Height + OverworldLayoutMetrics.TownGateWidth) / 2)), new Point(OverworldLayoutMetrics.TownWallThickness, (OverworldLayoutMetrics.TownBounds.Height - OverworldLayoutMetrics.TownGateWidth) / 2))
        ]);
    }

    private static void AddTownHouses(List<IWorldProp> props)
    {
        AddHouseStreet(props, 188f);
        AddHouseStreet(props, 428f);
        AddHouseStreet(props, 1296f);
        AddHouseStreet(props, 1536f);
    }

    private static void AddTownCentralDistrict(List<IWorldProp> props, Rectangle shopDoorBounds, Rectangle arenaDoorBounds)
    {
        props.AddRange(
        [
            new DungeonEntranceProp(new Vector2(840f, 676f), new Point(240, 184)),
            new ArenaEntranceProp(new Vector2(616f, 960f), new Point(160, 172), arenaDoorBounds, "Arena"),
            new ShopExteriorProp(new Vector2(876f, 968f), new Point(160, 164), shopDoorBounds, "Shop 1"),
            new ShopExteriorProp(new Vector2(1136f, 968f), new Point(160, 164), new Rectangle(1196, 1084, 32, 48), "Shop 2"),
            new ShopExteriorProp(new Vector2(1008f, 1212f), new Point(160, 164), new Rectangle(1068, 1328, 32, 48), "Shop 3")
        ]);
    }

    private static void AddTownDecor(List<IWorldProp> props)
    {
        props.AddRange(
        [
            new GrassProp(new Vector2(252f, 300f), new Point(54, 34)),
            new GrassProp(new Vector2(1580f, 328f), new Point(54, 34)),
            new GrassProp(new Vector2(264f, 1480f), new Point(58, 36)),
            new GrassProp(new Vector2(1576f, 1460f), new Point(58, 36)),
            new TreeProp(new Vector2(148f, 120f), new Point(76, 110)),
            new TreeProp(new Vector2(1692f, 122f), new Point(76, 110)),
            new TreeProp(new Vector2(156f, 1684f), new Point(76, 110)),
            new TreeProp(new Vector2(1688f, 1680f), new Point(76, 110))
        ]);
    }

    private static void AddHouseStreet(List<IWorldProp> props, float y)
    {
        props.AddRange(
        [
            new HouseExteriorProp(new Vector2(168f, y), new Point(176, 144)),
            new HouseExteriorProp(new Vector2(392f, y), new Point(176, 144)),
            new HouseExteriorProp(new Vector2(1360f, y), new Point(176, 144)),
            new HouseExteriorProp(new Vector2(1584f, y), new Point(176, 144))
        ]);
    }

    private World CreateWorld(
        PlayerActor player,
        IEnumerable<IWorldProp> props,
        IEnumerable<EnemyActor> enemies,
        IEnumerable<WorldSceneTransition> sceneTransitions,
        Rectangle? worldBounds = null,
        IWorldEventController? eventController = null)
    {
        return new World(
            player,
            props,
            enemies,
            _defaultEnemySettings,
            _enemySettingsCatalog,
            _enemyFactory,
            _playerAttackHitResolver,
            _playerBombResolver,
            _playerProjectileResolver,
            _worldObstacleResolver,
            _enemySeparationResolver,
            _enemyContactResolver,
            _worldCombatSettings,
            sceneTransitions,
            worldBounds,
            eventController);
    }

    private static EnemySpawnDefinition[] BuildArenaWaveOneSpawns()
    {
        return
        [
            new EnemySpawnDefinition(EnemyKind.HornedRabbitBoss, ScaleArenaVector(376f, 180f), EnemyAxisPreference.None)
        ];
    }

    private static EnemySpawnDefinition[] BuildArenaWaveTwoSpawns()
    {
        return
        [
            new EnemySpawnDefinition(EnemyKind.BatMiniBoss, ScaleArenaVector(372f, 180f)),
            new EnemySpawnDefinition(EnemyKind.Bat, ScaleArenaVector(386f, 132f)),
            new EnemySpawnDefinition(EnemyKind.Bat, ScaleArenaVector(300f, 196f)),
            new EnemySpawnDefinition(EnemyKind.Bat, ScaleArenaVector(472f, 196f))
        ];
    }

    private static EnemySpawnDefinition[] BuildArenaWaveThreeSpawns()
    {
        return
        [
            new EnemySpawnDefinition(EnemyKind.HornedRabbitElite, ScaleArenaVector(164f, 180f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.HornedRabbitElite, ScaleArenaVector(604f, 180f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.HornedRabbitElite, ScaleArenaVector(384f, 110f), EnemyAxisPreference.None)
        ];
    }

    private static EnemySpawnDefinition[] BuildArenaWaveFourSpawns()
    {
        return
        [
            CreateArenaHornedRabbitSpawn(ScaleArenaVector(170f, 96f)),
            CreateArenaHornedRabbitSpawn(ScaleArenaVector(376f, 96f)),
            CreateArenaHornedRabbitSpawn(ScaleArenaVector(582f, 96f)),
            CreateArenaHornedRabbitSpawn(ScaleArenaVector(170f, 328f)),
            CreateArenaHornedRabbitSpawn(ScaleArenaVector(376f, 328f)),
            CreateArenaHornedRabbitSpawn(ScaleArenaVector(582f, 328f)),
            CreateArenaHornedRabbitSpawn(ScaleArenaVector(106f, 164f)),
            CreateArenaHornedRabbitSpawn(ScaleArenaVector(106f, 260f)),
            CreateArenaHornedRabbitSpawn(ScaleArenaVector(646f, 164f)),
            CreateArenaHornedRabbitSpawn(ScaleArenaVector(646f, 260f))
        ];
    }

    private static EnemySpawnDefinition CreateArenaHornedRabbitSpawn(Vector2 position)
    {
        var movementTypes = new[]
        {
            EnemyAxisPreference.None,
            EnemyAxisPreference.Horizontal,
            EnemyAxisPreference.Vertical
        };
        var movementType = movementTypes[Random.Shared.Next(movementTypes.Length)];
        return new EnemySpawnDefinition(EnemyKind.HornedRabbit, position, movementType);
    }

    private static int ScaleArena(int value)
    {
        return (int)MathF.Round(value * ArenaScale);
    }

    private static Vector2 ScaleArenaVector(float x, float y)
    {
        return new Vector2(x * ArenaScale, y * ArenaScale);
    }

    private static Vector2 ScaleArenaVector(Vector2 value)
    {
        return ScaleArenaVector(value.X, value.Y);
    }

    private static Point ScaleArenaPoint(int x, int y)
    {
        return new Point(ScaleArena(x), ScaleArena(y));
    }

    private static Rectangle ScaleArenaRectangle(int x, int y, int width, int height)
    {
        return new Rectangle(ScaleArena(x), ScaleArena(y), ScaleArena(width), ScaleArena(height));
    }
}
