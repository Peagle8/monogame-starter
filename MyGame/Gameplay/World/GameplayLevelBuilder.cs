using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Narrative;
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
    private readonly PlayerFireShieldResolver _playerFireShieldResolver;
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
            new PlayerFireShieldResolver(new PlayerDefenseAbilitySettings()),
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
        PlayerFireShieldResolver playerFireShieldResolver,
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
            playerFireShieldResolver,
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
        : this(
            defaultEnemySettings,
            enemyFactory,
            enemySettingsCatalog,
            worldCombatSettings,
            playerAttackHitResolver,
            playerBombResolver,
            new PlayerFireShieldResolver(new PlayerDefenseAbilitySettings()),
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
        PlayerFireShieldResolver playerFireShieldResolver,
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
        _playerFireShieldResolver = playerFireShieldResolver;
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
        var arenaEnemySettingsCatalog = CreateArenaEnemySettingsCatalog();
        var arenaEnemyFactory = new EnemyFactory(arenaEnemySettingsCatalog);
        IWorldProp[] props = ArenaLayout.CreateBoundaryProps(isVisible: false);

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
            ArenaLayout.WorldBounds,
            new ArenaEncounterController(
                arenaEnemyFactory,
                true,
                BuildArenaWaveOneSpawns(),
                BuildArenaWaveTwoSpawns(),
                BuildArenaWaveThreeSpawns(),
                BuildArenaWaveFourSpawns(),
                BuildArenaWaveFiveSpawns(),
                BuildArenaWaveSixSpawns(),
                BuildArenaWaveSevenSpawns()),
            arenaEnemySettingsCatalog.Get(EnemyKind.Crab),
            arenaEnemySettingsCatalog,
            arenaEnemyFactory);
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
            new ShopExteriorProp(new Vector2(1008f, 1212f), new Point(160, 164), new Rectangle(1068, 1328, 32, 48), "Shop 3"),
            new TownsfolkProp(new Vector2(800f, 1196f), new Point(34, 44), NarrativeIds.SpeakerTownsfolkOne, "Townsfolk"),
            new TownsfolkProp(new Vector2(1236f, 1196f), new Point(34, 44), NarrativeIds.SpeakerTownsfolkTwo, "Townsfolk")
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
        IWorldEventController? eventController = null,
        EnemySettings? enemySettings = null,
        IEnemySettingsCatalog? enemySettingsCatalog = null,
        IEnemyFactory? enemyFactory = null)
    {
        var resolvedEnemySettings = enemySettings ?? _defaultEnemySettings;
        var resolvedEnemySettingsCatalog = enemySettingsCatalog ?? _enemySettingsCatalog;
        var resolvedEnemyFactory = enemyFactory ?? _enemyFactory;

        return new World(
            player,
            props,
            enemies,
            resolvedEnemySettings,
            resolvedEnemySettingsCatalog,
            resolvedEnemyFactory,
            _playerAttackHitResolver,
            _playerBombResolver,
            _playerFireShieldResolver,
            _playerProjectileResolver,
            _worldObstacleResolver,
            _enemySeparationResolver,
            _enemyContactResolver,
            _worldCombatSettings,
            sceneTransitions,
            worldBounds,
            eventController);
    }

    private EnemySettingsCatalog CreateArenaEnemySettingsCatalog()
    {
        var arenaChaseRange = ScaleArenaVector(800f, 480f).Length();

        return new EnemySettingsCatalog(
            CreateArenaEnemySettings(_enemySettingsCatalog.Get(EnemyKind.Crab), arenaChaseRange),
            CreateArenaEnemySettings(_enemySettingsCatalog.Get(EnemyKind.HornedRabbit), arenaChaseRange),
            CreateArenaEnemySettings(_enemySettingsCatalog.Get(EnemyKind.HornedRabbitElite), arenaChaseRange),
            CreateArenaEnemySettings(_enemySettingsCatalog.Get(EnemyKind.Bat), arenaChaseRange),
            CreateArenaEnemySettings(_enemySettingsCatalog.Get(EnemyKind.Grasshopper), arenaChaseRange),
            CreateArenaEnemySettings(_enemySettingsCatalog.Get(EnemyKind.BatMiniBoss), arenaChaseRange),
            CreateArenaEnemySettings(_enemySettingsCatalog.Get(EnemyKind.HornedRabbitBoss), arenaChaseRange),
            CreateArenaEnemySettings(_enemySettingsCatalog.Get(EnemyKind.Skeleton), arenaChaseRange),
            CreateArenaEnemySettings(_enemySettingsCatalog.Get(EnemyKind.SkeletonElite), arenaChaseRange));
    }

    private static EnemySettings CreateArenaEnemySettings(EnemySettings source, float arenaChaseRange)
    {
        return new EnemySettings
        {
            Kind = source.Kind,
            MaxHealth = source.MaxHealth,
            MoveSpeed = source.MoveSpeed,
            ChaseRange = Math.Max(source.ChaseRange, arenaChaseRange),
            RecoverySeconds = source.RecoverySeconds,
            DefeatedVisibleSeconds = source.DefeatedVisibleSeconds,
            PlayerHitKnockbackDistance = source.PlayerHitKnockbackDistance,
            PlayerHitKnockbackSeconds = source.PlayerHitKnockbackSeconds,
            PlayerHitPauseSeconds = source.PlayerHitPauseSeconds,
            DashSpeed = source.DashSpeed,
            DashSeconds = source.DashSeconds,
            DashPauseSeconds = source.DashPauseSeconds,
            InitialDashPauseMinSeconds = source.InitialDashPauseMinSeconds,
            InitialDashPauseMaxSeconds = source.InitialDashPauseMaxSeconds,
            AttackHitboxPadding = source.AttackHitboxPadding,
            BoundsWidth = source.BoundsWidth,
            BoundsHeight = source.BoundsHeight,
            MaxAbilityPoints = source.MaxAbilityPoints,
            AbilityPointRegenPerSecond = source.AbilityPointRegenPerSecond,
            ShieldActivationCost = source.ShieldActivationCost,
            ShieldMaxCharges = source.ShieldMaxCharges,
            ProjectileDamage = source.ProjectileDamage,
            ProjectileSpeed = source.ProjectileSpeed,
            ProjectileLifetimeSeconds = source.ProjectileLifetimeSeconds,
            ProjectileSize = source.ProjectileSize,
            ProjectileAttackRange = source.ProjectileAttackRange,
            PreferredRange = source.PreferredRange,
            SpecialAttackDamage = source.SpecialAttackDamage,
            SpecialAttackRange = source.SpecialAttackRange,
            SpecialAttackPauseSeconds = source.SpecialAttackPauseSeconds,
            SpecialAttackStunSeconds = source.SpecialAttackStunSeconds,
            SpecialAttackConeHalfAngleDegrees = source.SpecialAttackConeHalfAngleDegrees
        };
    }

    private static EnemySpawnDefinition[] BuildArenaWaveOneSpawns()
    {
        return
        [
            new EnemySpawnDefinition(EnemyKind.Skeleton, ScaleArenaVector(176f, 140f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Skeleton, ScaleArenaVector(384f, 140f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Skeleton, ScaleArenaVector(592f, 140f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Skeleton, ScaleArenaVector(176f, 284f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Skeleton, ScaleArenaVector(384f, 284f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Skeleton, ScaleArenaVector(592f, 284f), EnemyAxisPreference.None)
        ];
    }

    private static EnemySpawnDefinition[] BuildArenaWaveTwoSpawns()
    {
        return
        [
            new EnemySpawnDefinition(EnemyKind.Skeleton, ScaleArenaVector(176f, 132f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.SkeletonElite, ScaleArenaVector(256f, 176f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Skeleton, ScaleArenaVector(384f, 116f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.SkeletonElite, ScaleArenaVector(512f, 176f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Skeleton, ScaleArenaVector(592f, 132f), EnemyAxisPreference.None)
        ];
    }

    private static EnemySpawnDefinition[] BuildArenaWaveThreeSpawns()
    {
        return
        [
            new EnemySpawnDefinition(EnemyKind.HornedRabbitBoss, ScaleArenaVector(376f, 180f), EnemyAxisPreference.None)
        ];
    }

    private static EnemySpawnDefinition[] BuildArenaWaveFourSpawns()
    {
        return
        [
            new EnemySpawnDefinition(EnemyKind.BatMiniBoss, ScaleArenaVector(372f, 180f)),
            new EnemySpawnDefinition(EnemyKind.Bat, ScaleArenaVector(386f, 132f)),
            new EnemySpawnDefinition(EnemyKind.Bat, ScaleArenaVector(300f, 196f)),
            new EnemySpawnDefinition(EnemyKind.Bat, ScaleArenaVector(472f, 196f))
        ];
    }

    private static EnemySpawnDefinition[] BuildArenaWaveFiveSpawns()
    {
        return
        [
            new EnemySpawnDefinition(EnemyKind.HornedRabbitElite, ScaleArenaVector(164f, 180f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.HornedRabbitElite, ScaleArenaVector(604f, 180f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.HornedRabbitElite, ScaleArenaVector(384f, 110f), EnemyAxisPreference.None)
        ];
    }

    private static EnemySpawnDefinition[] BuildArenaWaveSixSpawns()
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

    private static EnemySpawnDefinition[] BuildArenaWaveSevenSpawns()
    {
        return
        [
            new EnemySpawnDefinition(EnemyKind.Grasshopper, ScaleArenaVector(144f, 116f)),
            new EnemySpawnDefinition(EnemyKind.Grasshopper, ScaleArenaVector(288f, 116f)),
            new EnemySpawnDefinition(EnemyKind.Grasshopper, ScaleArenaVector(480f, 116f)),
            new EnemySpawnDefinition(EnemyKind.Grasshopper, ScaleArenaVector(624f, 116f)),
            new EnemySpawnDefinition(EnemyKind.Skeleton, ScaleArenaVector(160f, 160f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Skeleton, ScaleArenaVector(304f, 160f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.SkeletonElite, ScaleArenaVector(448f, 160f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Skeleton, ScaleArenaVector(592f, 160f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.HornedRabbitElite, ScaleArenaVector(176f, 208f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.HornedRabbitElite, ScaleArenaVector(304f, 208f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.HornedRabbitElite, ScaleArenaVector(496f, 208f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.HornedRabbitElite, ScaleArenaVector(624f, 208f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Bat, ScaleArenaVector(224f, 288f)),
            new EnemySpawnDefinition(EnemyKind.Bat, ScaleArenaVector(400f, 272f)),
            new EnemySpawnDefinition(EnemyKind.Bat, ScaleArenaVector(576f, 288f)),
            new EnemySpawnDefinition(EnemyKind.HornedRabbit, ScaleArenaVector(160f, 344f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.HornedRabbit, ScaleArenaVector(304f, 344f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.HornedRabbit, ScaleArenaVector(496f, 344f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.HornedRabbit, ScaleArenaVector(640f, 344f), EnemyAxisPreference.None)
        ];
    }

    private static EnemySpawnDefinition CreateArenaHornedRabbitSpawn(Vector2 position)
    {
        return new EnemySpawnDefinition(EnemyKind.HornedRabbit, position, EnemyAxisPreference.None);
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
