using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.Props;
using MyGame.Scenes.Gameplay;

namespace MyGame.Gameplay.World;

public sealed class GameplayLevelBuilder
{
    private const int TownSize = 1920;
    private const int TownWallThickness = 80;
    private const int TownGateWidth = 176;
    private const int WildernessShortSize = 960;
    private const int WildernessLongSize = 1920;
    private const int MountainThickness = 156;

    private static readonly Rectangle TownBounds = new(0, 0, TownSize, TownSize);
    private static readonly Rectangle TownCentralDistrictBounds = new(576, 576, 768, 768);
    private static readonly Rectangle TownNorthGateTrigger = new((TownSize - TownGateWidth) / 2, TownWallThickness, TownGateWidth, 44);
    private static readonly Rectangle TownSouthGateTrigger = new((TownSize - TownGateWidth) / 2, TownSize - TownWallThickness - 44, TownGateWidth, 44);
    private static readonly Rectangle TownWestGateTrigger = new(TownWallThickness, (TownSize - TownGateWidth) / 2, 44, TownGateWidth);
    private static readonly Rectangle TownEastGateTrigger = new(TownSize - TownWallThickness - 44, (TownSize - TownGateWidth) / 2, 44, TownGateWidth);

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
            TownBounds);
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
        var waveOneSpawns = BuildArenaWaveOneSpawns();
        var waveTwoSpawns = BuildArenaWaveTwoSpawns();
        var waveThreeSpawns = BuildArenaWaveThreeSpawns();
        var waveFourSpawns = BuildArenaWaveFourSpawns();
        IWorldProp[] props =
        [
            new ArenaBoundaryProp(new Vector2(0f, 0f), new Point(800, 72)),
            new ArenaBoundaryProp(new Vector2(0f, 0f), new Point(72, 480)),
            new ArenaBoundaryProp(new Vector2(728f, 0f), new Point(72, 480)),
            new ArenaBoundaryProp(new Vector2(0f, 408f), new Point(304, 72)),
            new ArenaBoundaryProp(new Vector2(496f, 408f), new Point(304, 72)),
            new ArenaBoundaryProp(new Vector2(304f, 432f), new Point(32, 48)),
            new ArenaBoundaryProp(new Vector2(464f, 432f), new Point(32, 48))
        ];

        return CreateWorld(
            player,
            props,
            [],
            [
                new WorldSceneTransition(
                    new Rectangle(320, 392, 160, 64),
                    GameplaySceneNames.Overworld,
                    new Vector2(676f, 1144f),
                    world => world.IsObjectiveComplete)
            ],
            new Rectangle(0, 0, 800, 480),
            new ArenaEncounterController(_enemyFactory, true, waveOneSpawns, waveTwoSpawns, waveThreeSpawns, waveFourSpawns));
    }

    public World BuildWildernessNorth(PlayerActor player)
    {
        return BuildVerticalWilderness(
            player,
            new Rectangle(0, 0, WildernessLongSize, WildernessShortSize),
            new Rectangle((WildernessLongSize - TownGateWidth) / 2, WildernessShortSize - 76, TownGateWidth, 44),
            new Vector2(TownNorthGateTrigger.X, 144f),
            [
                new EnemySpawnDefinition(EnemyKind.HornedRabbit, new Vector2(540f, 240f), EnemyAxisPreference.Horizontal),
                new EnemySpawnDefinition(EnemyKind.HornedRabbit, new Vector2(1320f, 260f), EnemyAxisPreference.Vertical),
                new EnemySpawnDefinition(EnemyKind.Bat, new Vector2(820f, 180f), EnemyAxisPreference.None),
                new EnemySpawnDefinition(EnemyKind.Grasshopper, new Vector2(1040f, 420f), EnemyAxisPreference.None)
            ]);
    }

    public World BuildWildernessSouth(PlayerActor player)
    {
        return BuildVerticalWilderness(
            player,
            new Rectangle(0, 0, WildernessLongSize, WildernessShortSize),
            new Rectangle((WildernessLongSize - TownGateWidth) / 2, 32, TownGateWidth, 44),
            new Vector2(TownSouthGateTrigger.X, TownSize - 188f),
            [
                new EnemySpawnDefinition(EnemyKind.HornedRabbit, new Vector2(460f, 520f), EnemyAxisPreference.Horizontal),
                new EnemySpawnDefinition(EnemyKind.HornedRabbit, new Vector2(1410f, 500f), EnemyAxisPreference.Vertical),
                new EnemySpawnDefinition(EnemyKind.Bat, new Vector2(920f, 330f), EnemyAxisPreference.None),
                new EnemySpawnDefinition(EnemyKind.Grasshopper, new Vector2(760f, 620f), EnemyAxisPreference.None)
            ]);
    }

    public World BuildWildernessWest(PlayerActor player)
    {
        return BuildHorizontalWilderness(
            player,
            new Rectangle(0, 0, WildernessShortSize, WildernessLongSize),
            new Rectangle(WildernessShortSize - 76, (WildernessLongSize - TownGateWidth) / 2, 44, TownGateWidth),
            new Vector2(144f, TownWestGateTrigger.Y),
            [
                new EnemySpawnDefinition(EnemyKind.HornedRabbit, new Vector2(260f, 540f), EnemyAxisPreference.Horizontal),
                new EnemySpawnDefinition(EnemyKind.HornedRabbit, new Vector2(240f, 1360f), EnemyAxisPreference.Vertical),
                new EnemySpawnDefinition(EnemyKind.Bat, new Vector2(420f, 860f), EnemyAxisPreference.None),
                new EnemySpawnDefinition(EnemyKind.Grasshopper, new Vector2(560f, 1120f), EnemyAxisPreference.None)
            ]);
    }

    public World BuildWildernessEast(PlayerActor player)
    {
        return BuildHorizontalWilderness(
            player,
            new Rectangle(0, 0, WildernessShortSize, WildernessLongSize),
            new Rectangle(32, (WildernessLongSize - TownGateWidth) / 2, 44, TownGateWidth),
            new Vector2(TownSize - 188f, TownEastGateTrigger.Y),
            [
                new EnemySpawnDefinition(EnemyKind.HornedRabbit, new Vector2(620f, 620f), EnemyAxisPreference.Horizontal),
                new EnemySpawnDefinition(EnemyKind.HornedRabbit, new Vector2(600f, 1320f), EnemyAxisPreference.Vertical),
                new EnemySpawnDefinition(EnemyKind.Bat, new Vector2(440f, 980f), EnemyAxisPreference.None),
                new EnemySpawnDefinition(EnemyKind.Grasshopper, new Vector2(250f, 760f), EnemyAxisPreference.None)
            ]);
    }

    private IEnumerable<WorldSceneTransition> BuildTownTransitions(Rectangle shopDoorBounds, Rectangle arenaDoorBounds)
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
                new Vector2(384f, 392f)),
            new WorldSceneTransition(
                TownNorthGateTrigger,
                GameplaySceneNames.WildernessNorth,
                new Vector2(TownNorthGateTrigger.X, WildernessShortSize - 160f)),
            new WorldSceneTransition(
                TownSouthGateTrigger,
                GameplaySceneNames.WildernessSouth,
                new Vector2(TownSouthGateTrigger.X, 144f)),
            new WorldSceneTransition(
                TownWestGateTrigger,
                GameplaySceneNames.WildernessWest,
                new Vector2(WildernessShortSize - 160f, TownWestGateTrigger.Y)),
            new WorldSceneTransition(
                TownEastGateTrigger,
                GameplaySceneNames.WildernessEast,
                new Vector2(144f, TownEastGateTrigger.Y))
        ];
    }

    private World BuildVerticalWilderness(
        PlayerActor player,
        Rectangle bounds,
        Rectangle gateTrigger,
        Vector2 townReturnPosition,
        IEnumerable<EnemySpawnDefinition> spawns)
    {
        var props = CreateVerticalWildernessProps(bounds, gateTrigger);
        return CreateWorld(
            player,
            props,
            spawns.Select(_enemyFactory.Create),
            [
                new WorldSceneTransition(
                    gateTrigger,
                    GameplaySceneNames.Overworld,
                    townReturnPosition)
            ],
            bounds);
    }

    private World BuildHorizontalWilderness(
        PlayerActor player,
        Rectangle bounds,
        Rectangle gateTrigger,
        Vector2 townReturnPosition,
        IEnumerable<EnemySpawnDefinition> spawns)
    {
        var props = CreateHorizontalWildernessProps(bounds, gateTrigger);
        return CreateWorld(
            player,
            props,
            spawns.Select(_enemyFactory.Create),
            [
                new WorldSceneTransition(
                    gateTrigger,
                    GameplaySceneNames.Overworld,
                    townReturnPosition)
            ],
            bounds);
    }

    private IEnumerable<IWorldProp> CreateVerticalWildernessProps(Rectangle bounds, Rectangle gateTrigger)
    {
        var props = new List<IWorldProp>
        {
            new MountainProp(new Vector2(bounds.Left, bounds.Top), new Point(MountainThickness, bounds.Height)),
            new MountainProp(new Vector2(bounds.Right - MountainThickness, bounds.Top), new Point(MountainThickness, bounds.Height))
        };

        if (gateTrigger.Y < bounds.Center.Y)
        {
            props.Add(new MountainProp(new Vector2(bounds.Left, bounds.Bottom - MountainThickness), new Point(bounds.Width, MountainThickness)));
            AddTopMountainWithGate(props, bounds, gateTrigger);
        }
        else
        {
            props.Add(new MountainProp(new Vector2(bounds.Left, bounds.Top), new Point(bounds.Width, MountainThickness)));
            AddBottomMountainWithGate(props, bounds, gateTrigger);
        }

        AddWildernessDecor(props, bounds);
        return props;
    }

    private IEnumerable<IWorldProp> CreateHorizontalWildernessProps(Rectangle bounds, Rectangle gateTrigger)
    {
        var props = new List<IWorldProp>
        {
            new MountainProp(new Vector2(bounds.Left, bounds.Top), new Point(bounds.Width, MountainThickness)),
            new MountainProp(new Vector2(bounds.Left, bounds.Bottom - MountainThickness), new Point(bounds.Width, MountainThickness))
        };

        if (gateTrigger.X < bounds.Center.X)
        {
            props.Add(new MountainProp(new Vector2(bounds.Right - MountainThickness, bounds.Top), new Point(MountainThickness, bounds.Height)));
            AddLeftMountainWithGate(props, bounds, gateTrigger);
        }
        else
        {
            props.Add(new MountainProp(new Vector2(bounds.Left, bounds.Top), new Point(MountainThickness, bounds.Height)));
            AddRightMountainWithGate(props, bounds, gateTrigger);
        }

        AddWildernessDecor(props, bounds);
        return props;
    }

    private void AddTownWalls(List<IWorldProp> props)
    {
        props.AddRange(
        [
            new ArenaBoundaryProp(new Vector2(0f, 0f), new Point((TownBounds.Width - TownGateWidth) / 2, TownWallThickness)),
            new ArenaBoundaryProp(new Vector2(((TownBounds.Width + TownGateWidth) / 2), 0f), new Point((TownBounds.Width - TownGateWidth) / 2, TownWallThickness)),
            new ArenaBoundaryProp(new Vector2(0f, TownBounds.Bottom - TownWallThickness), new Point((TownBounds.Width - TownGateWidth) / 2, TownWallThickness)),
            new ArenaBoundaryProp(new Vector2(((TownBounds.Width + TownGateWidth) / 2), TownBounds.Bottom - TownWallThickness), new Point((TownBounds.Width - TownGateWidth) / 2, TownWallThickness)),
            new ArenaBoundaryProp(new Vector2(0f, 0f), new Point(TownWallThickness, (TownBounds.Height - TownGateWidth) / 2)),
            new ArenaBoundaryProp(new Vector2(0f, ((TownBounds.Height + TownGateWidth) / 2)), new Point(TownWallThickness, (TownBounds.Height - TownGateWidth) / 2)),
            new ArenaBoundaryProp(new Vector2(TownBounds.Right - TownWallThickness, 0f), new Point(TownWallThickness, (TownBounds.Height - TownGateWidth) / 2)),
            new ArenaBoundaryProp(new Vector2(TownBounds.Right - TownWallThickness, ((TownBounds.Height + TownGateWidth) / 2)), new Point(TownWallThickness, (TownBounds.Height - TownGateWidth) / 2))
        ]);
    }

    private void AddTopMountainWithGate(List<IWorldProp> props, Rectangle bounds, Rectangle gateTrigger)
    {
        props.Add(new MountainProp(new Vector2(bounds.Left, bounds.Top), new Point(gateTrigger.X, MountainThickness)));
        props.Add(new MountainProp(
            new Vector2(gateTrigger.Right, bounds.Top),
            new Point(bounds.Width - gateTrigger.Right, MountainThickness)));
    }

    private void AddTownHouses(List<IWorldProp> props)
    {
        AddHouseStreet(props, 188f);
        AddHouseStreet(props, 428f);
        AddHouseStreet(props, 1296f);
        AddHouseStreet(props, 1536f);
    }

    private void AddTownCentralDistrict(List<IWorldProp> props, Rectangle shopDoorBounds, Rectangle arenaDoorBounds)
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

    private void AddTownDecor(List<IWorldProp> props)
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

    private void AddHouseStreet(List<IWorldProp> props, float y)
    {
        props.AddRange(
        [
            new HouseExteriorProp(new Vector2(168f, y), new Point(176, 144)),
            new HouseExteriorProp(new Vector2(392f, y), new Point(176, 144)),
            new HouseExteriorProp(new Vector2(1360f, y), new Point(176, 144)),
            new HouseExteriorProp(new Vector2(1584f, y), new Point(176, 144))
        ]);
    }

    private void AddBottomMountainWithGate(List<IWorldProp> props, Rectangle bounds, Rectangle gateTrigger)
    {
        props.Add(new MountainProp(new Vector2(bounds.Left, bounds.Bottom - MountainThickness), new Point(gateTrigger.X, MountainThickness)));
        props.Add(new MountainProp(
            new Vector2(gateTrigger.Right, bounds.Bottom - MountainThickness),
            new Point(bounds.Width - gateTrigger.Right, MountainThickness)));
    }

    private void AddLeftMountainWithGate(List<IWorldProp> props, Rectangle bounds, Rectangle gateTrigger)
    {
        props.Add(new MountainProp(new Vector2(bounds.Left, bounds.Top), new Point(MountainThickness, gateTrigger.Y)));
        props.Add(new MountainProp(
            new Vector2(bounds.Left, gateTrigger.Bottom),
            new Point(MountainThickness, bounds.Height - gateTrigger.Bottom)));
    }

    private void AddRightMountainWithGate(List<IWorldProp> props, Rectangle bounds, Rectangle gateTrigger)
    {
        props.Add(new MountainProp(new Vector2(bounds.Right - MountainThickness, bounds.Top), new Point(MountainThickness, gateTrigger.Y)));
        props.Add(new MountainProp(
            new Vector2(bounds.Right - MountainThickness, gateTrigger.Bottom),
            new Point(MountainThickness, bounds.Height - gateTrigger.Bottom)));
    }

    private void AddWildernessDecor(List<IWorldProp> props, Rectangle bounds)
    {
        props.AddRange(
        [
            new TreeProp(new Vector2(bounds.Left + 220f, bounds.Top + 180f), new Point(76, 110)),
            new TreeProp(new Vector2(bounds.Right - 340f, bounds.Top + 220f), new Point(76, 110)),
            new TreeProp(new Vector2(bounds.Left + 300f, bounds.Bottom - 340f), new Point(76, 110)),
            new GrassProp(new Vector2(bounds.Center.X - 180f, bounds.Center.Y - 40f), new Point(58, 36)),
            new GrassProp(new Vector2(bounds.Center.X + 120f, bounds.Center.Y + 60f), new Point(58, 36))
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
            new EnemySpawnDefinition(EnemyKind.HornedRabbitBoss, new Vector2(376f, 180f), EnemyAxisPreference.None)
        ];
    }

    private static EnemySpawnDefinition[] BuildArenaWaveTwoSpawns()
    {
        return
        [
            new EnemySpawnDefinition(EnemyKind.BatMiniBoss, new Vector2(372f, 180f)),
            new EnemySpawnDefinition(EnemyKind.Bat, new Vector2(386f, 132f)),
            new EnemySpawnDefinition(EnemyKind.Bat, new Vector2(300f, 196f)),
            new EnemySpawnDefinition(EnemyKind.Bat, new Vector2(472f, 196f))
        ];
    }

    private static EnemySpawnDefinition[] BuildArenaWaveThreeSpawns()
    {
        return
        [
            new EnemySpawnDefinition(EnemyKind.HornedRabbitElite, new Vector2(164f, 180f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.HornedRabbitElite, new Vector2(604f, 180f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.HornedRabbitElite, new Vector2(384f, 110f), EnemyAxisPreference.None)
        ];
    }

    private static EnemySpawnDefinition[] BuildArenaWaveFourSpawns()
    {
        return
        [
            CreateArenaHornedRabbitSpawn(new Vector2(170f, 96f)),
            CreateArenaHornedRabbitSpawn(new Vector2(376f, 96f)),
            CreateArenaHornedRabbitSpawn(new Vector2(582f, 96f)),
            CreateArenaHornedRabbitSpawn(new Vector2(170f, 328f)),
            CreateArenaHornedRabbitSpawn(new Vector2(376f, 328f)),
            CreateArenaHornedRabbitSpawn(new Vector2(582f, 328f)),
            CreateArenaHornedRabbitSpawn(new Vector2(106f, 164f)),
            CreateArenaHornedRabbitSpawn(new Vector2(106f, 260f)),
            CreateArenaHornedRabbitSpawn(new Vector2(646f, 164f)),
            CreateArenaHornedRabbitSpawn(new Vector2(646f, 260f))
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
}
