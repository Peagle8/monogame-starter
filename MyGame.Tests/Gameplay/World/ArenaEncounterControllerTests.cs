using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Core;
using MyGame.Core.Input;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.World;
using MyGame.Infrastructure.Save;

namespace MyGame.Tests.Gameplay.World;

public sealed class ArenaEncounterControllerTests
{
    [Fact]
    public void Initialize_ShowsWaveOneBanner()
    {
        var world = CreateArenaWorld([]);

        Assert.Equal("Wave 1", world.ActiveScreenBanner?.Text);
        Assert.Single(world.Enemies);
        Assert.All(world.Enemies, enemy => Assert.Equal(EnemyKind.HornedRabbitBoss, enemy.Kind));
    }

    [Fact]
    public void Update_WhenFirstWaveIsCleared_StartsWaveTwoIntroBeforeSpawningEnemies()
    {
        var world = CreateArenaWorld([]);
        DefeatLivingEnemies(world);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.Equal("Wave 2", world.ActiveScreenBanner?.Text);
        Assert.Equal(world.Player.MaxHealth, world.Player.CurrentHealth);
        Assert.Equal(world.Player.MaxAbilityPoints, world.Player.CurrentAbilityPoints);
        Assert.False(world.IsObjectiveComplete);
    }

    [Fact]
    public void Update_WhenInterWaveRecoveryIsDisabled_DoesNotFullyHealPlayer()
    {
        var world = CreateArenaWorld([], fullHealBetweenWaves: false);
        world.Player.TakeDamage(5);
        world.Player.TrySpendAbilityPoints(2.5f);
        foreach (var enemy in world.Enemies)
        {
            enemy.TakeDamage(enemy.MaxHealth);
        }

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.Equal(world.Player.MaxHealth - 5, world.Player.CurrentHealth);
        Assert.True(world.Player.CurrentAbilityPoints < world.Player.MaxAbilityPoints);
    }

    [Fact]
    public void Update_AfterWaveTwoBannerExpires_SpawnsEliteWave()
    {
        var world = CreateArenaWorld([]);
        DefeatLivingEnemies(world);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(3.1), TimeSpan.FromSeconds(3.2)));

        Assert.Equal(3, world.Enemies.Count(enemy => enemy.State != EnemyState.Dead));
        Assert.All(world.Enemies.Where(enemy => enemy.State != EnemyState.Dead), enemy => Assert.Equal(EnemyKind.HornedRabbitElite, enemy.Kind));
    }

    [Fact]
    public void Update_WhenSecondWaveIsCleared_StartsWaveThreeIntro()
    {
        var world = CreateArenaWorld([]);
        DefeatLivingEnemies(world);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(3.1), TimeSpan.FromSeconds(3.2)));
        DefeatLivingEnemies(world);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(3.3)));

        Assert.Equal("Wave 3", world.ActiveScreenBanner?.Text);
        Assert.False(world.IsObjectiveComplete);
    }

    [Fact]
    public void Update_AfterWaveThreeBannerExpires_SpawnsBatWave()
    {
        var world = CreateArenaWorld([]);
        DefeatLivingEnemies(world);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(3.1), TimeSpan.FromSeconds(3.2)));
        DefeatLivingEnemies(world);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(3.3)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(3.1), TimeSpan.FromSeconds(6.4)));

        Assert.Equal(4, world.Enemies.Count(enemy => enemy.State != EnemyState.Dead));
        Assert.Contains(world.Enemies, enemy => enemy.Kind == EnemyKind.BatMiniBoss && enemy.State != EnemyState.Dead);
        Assert.Equal(3, world.Enemies.Count(enemy => enemy.Kind == EnemyKind.Bat && enemy.State != EnemyState.Dead));
    }

    [Fact]
    public void Update_WhenThirdWaveIsCleared_StartsWaveFourIntro()
    {
        var world = CreateArenaWorld([]);
        DefeatLivingEnemies(world);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(3.1), TimeSpan.FromSeconds(3.2)));
        DefeatLivingEnemies(world);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(3.3)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(3.1), TimeSpan.FromSeconds(6.4)));
        DefeatLivingEnemies(world);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(6.5)));

        Assert.Equal("Wave 4", world.ActiveScreenBanner?.Text);
        Assert.False(world.IsObjectiveComplete);
    }

    [Fact]
    public void Update_WhenFourthWaveIsCleared_CompletesEncounter()
    {
        var world = CreateArenaWorld([]);
        DefeatLivingEnemies(world);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(3.1), TimeSpan.FromSeconds(3.2)));
        DefeatLivingEnemies(world);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(3.3)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(3.1), TimeSpan.FromSeconds(6.4)));
        DefeatLivingEnemies(world);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(6.5)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(3.1), TimeSpan.FromSeconds(9.6)));
        DefeatLivingEnemies(world);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(9.7)));

        Assert.True(world.IsObjectiveComplete);
    }

    private static global::MyGame.Gameplay.World.World CreateArenaWorld(IEnumerable<EnemyActor> initialEnemies, bool fullHealBetweenWaves = true)
    {
        var player = new PlayerActor(
            new StubInputService(),
            new PlayerCombatSettings(),
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));
        player.RestoreState(new Vector2(461f, 470.4f), player.MaxHealth);

        var controller = new ArenaEncounterController(
            new StubEnemyFactory(),
            fullHealBetweenWaves,
            new[]
            {
                new EnemySpawnDefinition(EnemyKind.HornedRabbitBoss, new Vector2(200f, 120f), EnemyAxisPreference.None)
            },
            Enumerable.Range(0, 3).Select(index =>
                new EnemySpawnDefinition(
                    EnemyKind.HornedRabbitElite,
                    new Vector2(120f + (index * 60f), 96f),
                    EnemyAxisPreference.None)),
            new[]
            {
                new EnemySpawnDefinition(EnemyKind.BatMiniBoss, new Vector2(200f, 200f), EnemyAxisPreference.None),
                new EnemySpawnDefinition(EnemyKind.Bat, new Vector2(260f, 180f), EnemyAxisPreference.None),
                new EnemySpawnDefinition(EnemyKind.Bat, new Vector2(320f, 180f), EnemyAxisPreference.None),
                new EnemySpawnDefinition(EnemyKind.Bat, new Vector2(380f, 180f), EnemyAxisPreference.None)
            },
            Enumerable.Range(0, 10).Select(index =>
                new EnemySpawnDefinition(
                    EnemyKind.HornedRabbit,
                    new Vector2(80f + (index * 16f), 96f),
                    EnemyAxisPreference.None)));

        return new global::MyGame.Gameplay.World.World(
            player,
            [],
            initialEnemies,
            sceneTransitions: [],
            eventController: controller);
    }

    private sealed class StubEnemyFactory : IEnemyFactory
    {
        public EnemyActor Create(EnemySpawnDefinition spawn)
        {
            return Create(spawn.Kind, spawn.Position, spawn.AxisPreference);
        }

        public EnemyActor CreateFromSaveData(EnemySaveData saveData)
        {
            return Create(saveData.Kind, new Vector2(saveData.PositionX, saveData.PositionY), saveData.AxisPreference);
        }

        public EnemyActor Create(EnemyKind kind, Vector2 position, EnemyAxisPreference axisPreference = EnemyAxisPreference.None)
        {
            return new EnemyActor(EnemySettingsCatalog.CreateDefault(kind), position, axisPreference: axisPreference);
        }
    }

    private sealed class StubInputService : IInputService
    {
        public InputSnapshot Current => InputSnapshot.Empty;

        public InputSnapshot Previous => InputSnapshot.Empty;

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

        public void Update()
        {
        }
    }

    private static void DefeatLivingEnemies(global::MyGame.Gameplay.World.World world)
    {
        foreach (var enemy in world.Enemies.Where(static enemy => enemy.State != EnemyState.Dead))
        {
            DefeatEnemy(enemy);
        }
    }

    private static void DefeatEnemy(EnemyActor enemy)
    {
        while (enemy.State != EnemyState.Dead)
        {
            enemy.TakeDamage(enemy.MaxHealth);
        }
    }
}
