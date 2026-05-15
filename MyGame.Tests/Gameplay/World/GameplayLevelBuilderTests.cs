using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Core;
using MyGame.Core.Input;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.Props;
using MyGame.Gameplay.World;
using MyGame.Scenes.Gameplay;

namespace MyGame.Tests.Gameplay.World;

public sealed class GameplayLevelBuilderTests
{
    [Fact]
    public void BuildOverworld_CreatesTownHubWithExpectedDistrictProps()
    {
        var builder = CreateBuilder();
        var world = builder.BuildOverworld(CreatePlayer());

        Assert.Equal(new Rectangle(0, 0, 1920, 1920), world.WorldBounds);
        Assert.Equal(16, world.GetProps<HouseExteriorProp>().Count);
        Assert.Equal(3, world.GetProps<ShopExteriorProp>().Count);
        Assert.Single(world.GetProps<ArenaEntranceProp>());
        Assert.Single(world.GetProps<DungeonEntranceProp>());
    }

    [Fact]
    public void BuildOverworld_WhenPlayerTouchesNorthGate_QueuesWildernessNorthTransition()
    {
        var builder = CreateBuilder();
        var world = builder.BuildOverworld(CreatePlayer());
        world.Player.RestoreState(new Vector2(872f, 80f), world.Player.MaxHealth);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        var transition = world.ConsumePendingSceneTransition();

        Assert.NotNull(transition);
        Assert.Equal(GameplaySceneNames.WildernessNorth, transition!.TargetSceneName);
    }

    [Fact]
    public void BuildOverworld_WhenPlayerTouchesShopDoor_QueuesShopTransition()
    {
        var builder = CreateBuilder();
        var world = builder.BuildOverworld(CreatePlayer());
        world.Player.RestoreState(new Vector2(940f, 1084f), world.Player.MaxHealth);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        var transition = world.ConsumePendingSceneTransition();

        Assert.NotNull(transition);
        Assert.Equal(GameplaySceneNames.ShopInterior, transition!.TargetSceneName);
    }

    [Fact]
    public void BuildWildernessNorth_CreatesMountainBoundsAndTownReturn()
    {
        var builder = CreateBuilder();
        var world = builder.BuildWildernessNorth(CreatePlayer());
        world.Player.RestoreState(new Vector2(872f, 884f), world.Player.MaxHealth);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        var transition = world.ConsumePendingSceneTransition();

        Assert.Equal(new Rectangle(0, 0, 1920, 960), world.WorldBounds);
        Assert.NotEmpty(world.GetProps<MountainProp>());
        Assert.NotEmpty(world.Enemies);
        Assert.NotNull(transition);
        Assert.Equal(GameplaySceneNames.Overworld, transition!.TargetSceneName);
    }

    [Fact]
    public void BuildWildernessNorth_WhenPlayerTouchesWestEdge_QueuesWestWildernessTransition()
    {
        var builder = CreateBuilder();
        var world = builder.BuildWildernessNorth(CreatePlayer());
        world.Player.RestoreState(new Vector2(8f, 240f), world.Player.MaxHealth);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        var transition = world.ConsumePendingSceneTransition();

        Assert.NotNull(transition);
        Assert.Equal(GameplaySceneNames.WildernessWest, transition!.TargetSceneName);
        Assert.Equal(new Vector2(240f, 72f), transition.ResolveTargetPlayerPosition(world));
    }

    [Fact]
    public void BuildWildernessWest_WhenPlayerTouchesSouthEdge_QueuesSouthWildernessTransition()
    {
        var builder = CreateBuilder();
        var world = builder.BuildWildernessWest(CreatePlayer());
        world.Player.RestoreState(new Vector2(240f, 1900f), world.Player.MaxHealth);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        var transition = world.ConsumePendingSceneTransition();

        Assert.NotNull(transition);
        Assert.Equal(GameplaySceneNames.WildernessSouth, transition!.TargetSceneName);
        Assert.Equal(new Vector2(72f, 720f), transition.ResolveTargetPlayerPosition(world));
    }

    [Fact]
    public void BuildWildernessSouth_WhenPlayerTouchesEastEdge_QueuesEastWildernessTransition()
    {
        var builder = CreateBuilder();
        var world = builder.BuildWildernessSouth(CreatePlayer());
        world.Player.RestoreState(new Vector2(1888f, 240f), world.Player.MaxHealth);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        var transition = world.ConsumePendingSceneTransition();

        Assert.NotNull(transition);
        Assert.Equal(GameplaySceneNames.WildernessEast, transition!.TargetSceneName);
        Assert.Equal(new Vector2(240f, 1848f), transition.ResolveTargetPlayerPosition(world));
    }

    [Fact]
    public void BuildWildernessEast_WhenPlayerTouchesNorthEdge_QueuesNorthWildernessTransition()
    {
        var builder = CreateBuilder();
        var world = builder.BuildWildernessEast(CreatePlayer());
        world.Player.RestoreState(new Vector2(720f, 8f), world.Player.MaxHealth);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        var transition = world.ConsumePendingSceneTransition();

        Assert.NotNull(transition);
        Assert.Equal(GameplaySceneNames.WildernessNorth, transition!.TargetSceneName);
        Assert.Equal(new Vector2(1848f, 240f), transition.ResolveTargetPlayerPosition(world));
    }

    [Fact]
    public void BuildWildernessWest_WhenPlayerTouchesNorthEdge_QueuesNorthWildernessTransition()
    {
        var builder = CreateBuilder();
        var world = builder.BuildWildernessWest(CreatePlayer());
        world.Player.RestoreState(new Vector2(720f, 8f), world.Player.MaxHealth);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        var transition = world.ConsumePendingSceneTransition();

        Assert.NotNull(transition);
        Assert.Equal(GameplaySceneNames.WildernessNorth, transition!.TargetSceneName);
        Assert.Equal(new Vector2(72f, 720f), transition.ResolveTargetPlayerPosition(world));
    }

    [Fact]
    public void BuildWildernessEast_WhenPlayerTouchesSouthEdge_QueuesSouthWildernessTransition()
    {
        var builder = CreateBuilder();
        var world = builder.BuildWildernessEast(CreatePlayer());
        world.Player.RestoreState(new Vector2(240f, 1900f), world.Player.MaxHealth);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        var transition = world.ConsumePendingSceneTransition();

        Assert.NotNull(transition);
        Assert.Equal(GameplaySceneNames.WildernessSouth, transition!.TargetSceneName);
        Assert.Equal(new Vector2(1848f, 240f), transition.ResolveTargetPlayerPosition(world));
    }

    [Fact]
    public void BuildArena_CreatesArenaLayoutBoundsAndHiddenCollisionProps()
    {
        var builder = CreateBuilder();
        var world = builder.BuildArena(CreatePlayer());
        var boundaries = world.GetProps<ArenaBoundaryProp>();

        Assert.Equal(ArenaLayout.WorldBounds, world.WorldBounds);
        Assert.Equal(ArenaLayout.CollisionBounds.Count, boundaries.Count);
        Assert.All(boundaries, boundary => Assert.False(boundary.IsVisible));
    }

    [Fact]
    public void BuildArena_InitialWaveUsesSixSkeletons()
    {
        var builder = CreateBuilder();
        var world = builder.BuildArena(CreatePlayer());

        Assert.Equal(6, world.Enemies.Count(enemy => enemy.State != EnemyState.Dead));
        Assert.All(world.Enemies.Where(enemy => enemy.State != EnemyState.Dead), enemy => Assert.Equal(EnemyKind.Skeleton, enemy.Kind));
    }

    [Fact]
    public void BuildArena_WhenWaveTwoSpawns_UsesMixedSkeletonPack()
    {
        var builder = CreateBuilder();
        var world = builder.BuildArena(CreatePlayer());

        AdvanceArenaToWaveTwo(world);

        Assert.Equal(5, world.Enemies.Count(enemy => enemy.State != EnemyState.Dead));
        Assert.Equal(3, world.Enemies.Count(enemy => enemy.Kind == EnemyKind.Skeleton && enemy.State != EnemyState.Dead));
        Assert.Equal(2, world.Enemies.Count(enemy => enemy.Kind == EnemyKind.SkeletonElite && enemy.State != EnemyState.Dead));
    }

    [Fact]
    public void BuildArena_WhenWaveFourBatsSpawn_TheyTrackPlayerFromAcrossArena()
    {
        var builder = CreateBuilder();
        var world = builder.BuildArena(CreatePlayer());
        world.Player.RestoreState(new Vector2(760f, 440f), world.Player.MaxHealth);

        AdvanceArenaToWaveFour(world);
        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(9.7)));

        var farBat = world.Enemies
            .Where(enemy => enemy.Kind == EnemyKind.Bat && enemy.State != EnemyState.Dead)
            .OrderByDescending(enemy => Vector2.Distance(enemy.Position, world.Player.Position))
            .First();

        Assert.True(Vector2.Distance(farBat.Position, world.Player.Position) > EnemySettingsCatalog.CreateDefault(EnemyKind.Bat).ChaseRange);
        Assert.NotEqual(EnemyState.Idle, farBat.State);
    }

    [Fact]
    public void BuildArena_WhenWaveSixHornedRabbitsSpawn_TheyTrackPlayerFromAcrossArena()
    {
        var builder = CreateBuilder();
        var world = builder.BuildArena(CreatePlayer());
        world.Player.RestoreState(new Vector2(760f, 440f), world.Player.MaxHealth);

        AdvanceArenaToWaveSix(world);
        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(16.1)));

        var farRabbit = world.Enemies
            .Where(enemy => enemy.Kind == EnemyKind.HornedRabbit && enemy.State != EnemyState.Dead)
            .OrderByDescending(enemy => Vector2.Distance(enemy.Position, world.Player.Position))
            .First();
        var liveRabbits = world.Enemies
            .Where(enemy => enemy.Kind == EnemyKind.HornedRabbit && enemy.State != EnemyState.Dead)
            .ToArray();

        Assert.True(Vector2.Distance(farRabbit.Position, world.Player.Position) > EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbit).ChaseRange);
        Assert.NotEqual(EnemyState.Idle, farRabbit.State);
        Assert.All(liveRabbits, enemy => Assert.Equal(EnemyAxisPreference.None, enemy.AxisPreference));
    }

    [Fact]
    public void BuildArena_WhenWaveSevenSpawns_UsesRequestedEnemyMix()
    {
        var builder = CreateBuilder();
        var world = builder.BuildArena(CreatePlayer());

        AdvanceArenaToWaveSeven(world);
        var liveRabbits = world.Enemies
            .Where(enemy => enemy.Kind == EnemyKind.HornedRabbit && enemy.State != EnemyState.Dead)
            .ToArray();

        Assert.Equal(19, world.Enemies.Count(enemy => enemy.State != EnemyState.Dead));
        Assert.Equal(4, world.Enemies.Count(enemy => enemy.Kind == EnemyKind.Grasshopper && enemy.State != EnemyState.Dead));
        Assert.Equal(3, world.Enemies.Count(enemy => enemy.Kind == EnemyKind.Skeleton && enemy.State != EnemyState.Dead));
        Assert.Equal(1, world.Enemies.Count(enemy => enemy.Kind == EnemyKind.SkeletonElite && enemy.State != EnemyState.Dead));
        Assert.Equal(4, world.Enemies.Count(enemy => enemy.Kind == EnemyKind.HornedRabbitElite && enemy.State != EnemyState.Dead));
        Assert.Equal(4, liveRabbits.Length);
        Assert.Equal(3, world.Enemies.Count(enemy => enemy.Kind == EnemyKind.Bat && enemy.State != EnemyState.Dead));
        Assert.All(liveRabbits, enemy => Assert.Equal(EnemyAxisPreference.None, enemy.AxisPreference));
    }

    private static GameplayLevelBuilder CreateBuilder()
    {
        var catalog = new EnemySettingsCatalog(
            EnemySettingsCatalog.CreateDefault(EnemyKind.Crab),
            EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbit),
            EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbitElite),
            EnemySettingsCatalog.CreateDefault(EnemyKind.Bat),
            EnemySettingsCatalog.CreateDefault(EnemyKind.Grasshopper),
            EnemySettingsCatalog.CreateDefault(EnemyKind.BatMiniBoss));

        return new GameplayLevelBuilder(
            catalog.Get(EnemyKind.Crab),
            new EnemyFactory(catalog),
            catalog,
            new WorldCombatSettings(),
            new PlayerAttackHitResolver(),
            new PlayerProjectileResolver(),
            new WorldObstacleResolver(new WorldCombatSettings()),
            new EnemySeparationResolver(new WorldCombatSettings()),
            new EnemyContactResolver(new WorldCombatSettings()));
    }

    private static PlayerActor CreatePlayer()
    {
        return new PlayerActor(
            new StubInputService(),
            new PlayerCombatSettings(),
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));
    }

    private static void AdvanceArenaToWaveTwo(global::MyGame.Gameplay.World.World world)
    {
        DefeatLivingEnemies(world);
        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(3.1), TimeSpan.FromSeconds(3.2)));
    }

    private static void AdvanceArenaToWaveThree(global::MyGame.Gameplay.World.World world)
    {
        AdvanceArenaToWaveTwo(world);
        DefeatLivingEnemies(world);
        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(3.3)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(3.1), TimeSpan.FromSeconds(6.4)));
    }

    private static void AdvanceArenaToWaveFour(global::MyGame.Gameplay.World.World world)
    {
        AdvanceArenaToWaveThree(world);
        DefeatLivingEnemies(world);
        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(6.5)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(3.1), TimeSpan.FromSeconds(9.6)));
    }

    private static void AdvanceArenaToWaveFive(global::MyGame.Gameplay.World.World world)
    {
        AdvanceArenaToWaveFour(world);
        DefeatLivingEnemies(world);
        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(9.7)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(3.1), TimeSpan.FromSeconds(12.8)));
    }

    private static void AdvanceArenaToWaveSix(global::MyGame.Gameplay.World.World world)
    {
        AdvanceArenaToWaveFive(world);
        DefeatLivingEnemies(world);
        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(12.9)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(3.1), TimeSpan.FromSeconds(16.0)));
    }

    private static void AdvanceArenaToWaveSeven(global::MyGame.Gameplay.World.World world)
    {
        AdvanceArenaToWaveSix(world);
        DefeatLivingEnemies(world);
        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(16.1)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(3.1), TimeSpan.FromSeconds(19.2)));
    }

    private static void DefeatLivingEnemies(global::MyGame.Gameplay.World.World world)
    {
        foreach (var enemy in world.Enemies.Where(enemy => enemy.State != EnemyState.Dead))
        {
            while (enemy.State != EnemyState.Dead)
            {
                enemy.TakeDamage(enemy.MaxHealth);
            }
        }
    }

    private sealed class StubInputService : IInputService
    {
        public InputSnapshot Current => InputSnapshot.Empty;

        public InputSnapshot Previous => InputSnapshot.Empty;

        public void Update()
        {
        }

        public bool IsPressed(GameAction action)
        {
            return false;
        }

        public bool IsJustPressed(GameAction action)
        {
            return false;
        }

        public bool IsJustReleased(GameAction action)
        {
            return false;
        }
    }
}
