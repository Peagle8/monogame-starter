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
