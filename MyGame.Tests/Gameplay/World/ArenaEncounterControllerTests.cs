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
        Assert.Equal(6, world.Enemies.Count(enemy => enemy.State != EnemyState.Dead));
        Assert.All(world.Enemies, enemy => Assert.Equal(EnemyKind.Skeleton, enemy.Kind));
    }

    [Fact]
    public void Update_WhenFirstWaveIsCleared_StartsWaveTwoIntroBeforeSpawningEnemies()
    {
        var world = CreateArenaWorld([]);

        ShowNextWaveIntro(world);

        Assert.Equal("Wave 2", world.ActiveScreenBanner?.Text);
        Assert.Equal(world.Player.MaxHealth, world.Player.CurrentHealth);
        Assert.Equal(world.Player.MaxAbilityPoints, world.Player.CurrentAbilityPoints);
        Assert.False(world.IsObjectiveComplete);
    }

    [Fact]
    public void Update_WhenFirstWaveIsCleared_PreservesActiveFireShieldAndEquippedDefenseAbility()
    {
        var inputService = new StubInputService(GameAction.DefenseAbility);
        var player = new PlayerActor(
            inputService,
            new PlayerCombatSettings { MaxAbilityPoints = 3f, AbilityPointRegenPerSecond = 0f },
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash, PlayerAbility.Fireball]),
            new PlayerAttackController(new PlayerAttackSettings()),
            new PlayerDefenseAbilityController(new PlayerDefenseAbilitySettings()),
            new PlayerRangedAttackController(new PlayerRangedAttackSettings()));
        player.RestoreState(new Vector2(461f, 470.4f), player.MaxHealth, player.MaxAbilityPoints);
        player.EquipDefenseAbility(PlayerDefenseAbilityKind.FireShield);
        player.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        var world = CreateArenaWorld(player, []);

        ShowNextWaveIntro(world);

        Assert.Equal(PlayerDefenseAbilityKind.FireShield, world.Player.EquippedDefenseAbility);
        Assert.True(world.Player.IsFireShieldActive);
        Assert.Equal(3, world.Player.ShieldCharges);
        Assert.Equal(world.Player.MaxHealth, world.Player.CurrentHealth);
        Assert.Equal(world.Player.MaxAbilityPoints, world.Player.CurrentAbilityPoints);
    }

    [Fact]
    public void Update_WhenInterWaveRecoveryIsDisabled_DoesNotFullyHealPlayer()
    {
        var world = CreateArenaWorld([], fullHealBetweenWaves: false);
        world.Player.TakeDamage(5);
        world.Player.TrySpendAbilityPoints(2.5f);

        ShowNextWaveIntro(world);

        Assert.Equal(world.Player.MaxHealth - 5, world.Player.CurrentHealth);
    }

    [Fact]
    public void Update_AfterWaveTwoBannerExpires_SpawnsMixedSkeletonWave()
    {
        var world = CreateArenaWorld([]);

        AdvanceToSpawnedWave(world, 2);

        Assert.Equal(5, world.Enemies.Count(enemy => enemy.State != EnemyState.Dead));
        Assert.Equal(3, world.Enemies.Count(enemy => enemy.Kind == EnemyKind.Skeleton && enemy.State != EnemyState.Dead));
        Assert.Equal(2, world.Enemies.Count(enemy => enemy.Kind == EnemyKind.SkeletonElite && enemy.State != EnemyState.Dead));
    }

    [Fact]
    public void Update_WhenSecondWaveIsCleared_StartsWaveThreeIntro()
    {
        var world = CreateArenaWorld([]);

        AdvanceToSpawnedWave(world, 2);
        ShowNextWaveIntro(world);

        Assert.Equal("Wave 3", world.ActiveScreenBanner?.Text);
        Assert.False(world.IsObjectiveComplete);
    }

    [Fact]
    public void Update_AfterWaveThreeBannerExpires_SpawnsBossWave()
    {
        var world = CreateArenaWorld([]);

        AdvanceToSpawnedWave(world, 3);

        Assert.Single(world.Enemies.Where(enemy => enemy.State != EnemyState.Dead));
        Assert.All(
            world.Enemies.Where(enemy => enemy.State != EnemyState.Dead),
            enemy => Assert.Equal(EnemyKind.HornedRabbitBoss, enemy.Kind));
    }

    [Fact]
    public void Update_AfterWaveFourBannerExpires_SpawnsBatWave()
    {
        var world = CreateArenaWorld([]);

        AdvanceToSpawnedWave(world, 4);

        Assert.Equal(4, world.Enemies.Count(enemy => enemy.State != EnemyState.Dead));
        Assert.Contains(world.Enemies, enemy => enemy.Kind == EnemyKind.BatMiniBoss && enemy.State != EnemyState.Dead);
        Assert.Equal(3, world.Enemies.Count(enemy => enemy.Kind == EnemyKind.Bat && enemy.State != EnemyState.Dead));
    }

    [Fact]
    public void Update_AfterWaveFiveBannerExpires_SpawnsEliteRabbitWave()
    {
        var world = CreateArenaWorld([]);

        AdvanceToSpawnedWave(world, 5);

        Assert.Equal(3, world.Enemies.Count(enemy => enemy.State != EnemyState.Dead));
        Assert.All(
            world.Enemies.Where(enemy => enemy.State != EnemyState.Dead),
            enemy => Assert.Equal(EnemyKind.HornedRabbitElite, enemy.Kind));
    }

    [Fact]
    public void Update_AfterWaveSixBannerExpires_SpawnsHornedRabbitWave()
    {
        var world = CreateArenaWorld([]);

        AdvanceToSpawnedWave(world, 6);

        Assert.Equal(10, world.Enemies.Count(enemy => enemy.Kind == EnemyKind.HornedRabbit && enemy.State != EnemyState.Dead));
    }

    [Fact]
    public void Update_AfterWaveSevenBannerExpires_SpawnsMixedFinalWave()
    {
        var world = CreateArenaWorld([]);

        AdvanceToSpawnedWave(world, 7);

        Assert.Equal(19, world.Enemies.Count(enemy => enemy.State != EnemyState.Dead));
        Assert.Equal(4, world.Enemies.Count(enemy => enemy.Kind == EnemyKind.Grasshopper && enemy.State != EnemyState.Dead));
        Assert.Equal(3, world.Enemies.Count(enemy => enemy.Kind == EnemyKind.Skeleton && enemy.State != EnemyState.Dead));
        Assert.Equal(1, world.Enemies.Count(enemy => enemy.Kind == EnemyKind.SkeletonElite && enemy.State != EnemyState.Dead));
        Assert.Equal(4, world.Enemies.Count(enemy => enemy.Kind == EnemyKind.HornedRabbitElite && enemy.State != EnemyState.Dead));
        Assert.Equal(4, world.Enemies.Count(enemy => enemy.Kind == EnemyKind.HornedRabbit && enemy.State != EnemyState.Dead));
        Assert.Equal(3, world.Enemies.Count(enemy => enemy.Kind == EnemyKind.Bat && enemy.State != EnemyState.Dead));
    }

    [Fact]
    public void Update_WhenSeventhWaveIsCleared_CompletesEncounter()
    {
        var world = CreateArenaWorld([]);

        AdvanceToSpawnedWave(world, 7);
        DefeatLivingEnemies(world);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

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

        return CreateArenaWorld(player, initialEnemies, fullHealBetweenWaves);
    }

    private static global::MyGame.Gameplay.World.World CreateArenaWorld(PlayerActor player, IEnumerable<EnemyActor> initialEnemies, bool fullHealBetweenWaves = true)
    {
        var controller = new ArenaEncounterController(
            new StubEnemyFactory(),
            fullHealBetweenWaves,
            new[]
            {
                new EnemySpawnDefinition(EnemyKind.Skeleton, new Vector2(120f, 96f), EnemyAxisPreference.None),
                new EnemySpawnDefinition(EnemyKind.Skeleton, new Vector2(220f, 96f), EnemyAxisPreference.None),
                new EnemySpawnDefinition(EnemyKind.Skeleton, new Vector2(320f, 96f), EnemyAxisPreference.None),
                new EnemySpawnDefinition(EnemyKind.Skeleton, new Vector2(120f, 196f), EnemyAxisPreference.None),
                new EnemySpawnDefinition(EnemyKind.Skeleton, new Vector2(220f, 196f), EnemyAxisPreference.None),
                new EnemySpawnDefinition(EnemyKind.Skeleton, new Vector2(320f, 196f), EnemyAxisPreference.None)
            },
            new[]
            {
                new EnemySpawnDefinition(EnemyKind.Skeleton, new Vector2(100f, 116f), EnemyAxisPreference.None),
                new EnemySpawnDefinition(EnemyKind.SkeletonElite, new Vector2(160f, 148f), EnemyAxisPreference.None),
                new EnemySpawnDefinition(EnemyKind.Skeleton, new Vector2(220f, 92f), EnemyAxisPreference.None),
                new EnemySpawnDefinition(EnemyKind.SkeletonElite, new Vector2(280f, 148f), EnemyAxisPreference.None),
                new EnemySpawnDefinition(EnemyKind.Skeleton, new Vector2(340f, 116f), EnemyAxisPreference.None)
            },
            new[]
            {
                new EnemySpawnDefinition(EnemyKind.HornedRabbitBoss, new Vector2(200f, 120f), EnemyAxisPreference.None)
            },
            new[]
            {
                new EnemySpawnDefinition(EnemyKind.BatMiniBoss, new Vector2(200f, 200f), EnemyAxisPreference.None),
                new EnemySpawnDefinition(EnemyKind.Bat, new Vector2(260f, 180f), EnemyAxisPreference.None),
                new EnemySpawnDefinition(EnemyKind.Bat, new Vector2(320f, 180f), EnemyAxisPreference.None),
                new EnemySpawnDefinition(EnemyKind.Bat, new Vector2(380f, 180f), EnemyAxisPreference.None)
            },
            Enumerable.Range(0, 3).Select(index =>
                new EnemySpawnDefinition(
                    EnemyKind.HornedRabbitElite,
                    new Vector2(120f + (index * 60f), 96f),
                    EnemyAxisPreference.None)),
            Enumerable.Range(0, 10).Select(index =>
                new EnemySpawnDefinition(
                    EnemyKind.HornedRabbit,
                    new Vector2(80f + (index * 16f), 96f),
                    EnemyAxisPreference.None)),
            new EnemySpawnDefinition[]
            {
                new(EnemyKind.Grasshopper, new Vector2(120f, 96f), EnemyAxisPreference.None),
                new(EnemyKind.Grasshopper, new Vector2(220f, 96f), EnemyAxisPreference.None),
                new(EnemyKind.Grasshopper, new Vector2(320f, 96f), EnemyAxisPreference.None),
                new(EnemyKind.Grasshopper, new Vector2(420f, 96f), EnemyAxisPreference.None),
                new(EnemyKind.Skeleton, new Vector2(140f, 136f), EnemyAxisPreference.None),
                new(EnemyKind.Skeleton, new Vector2(220f, 136f), EnemyAxisPreference.None),
                new(EnemyKind.SkeletonElite, new Vector2(300f, 136f), EnemyAxisPreference.None),
                new(EnemyKind.Skeleton, new Vector2(380f, 136f), EnemyAxisPreference.None),
                new(EnemyKind.HornedRabbitElite, new Vector2(120f, 172f), EnemyAxisPreference.None),
                new(EnemyKind.HornedRabbitElite, new Vector2(220f, 172f), EnemyAxisPreference.None),
                new(EnemyKind.HornedRabbitElite, new Vector2(320f, 172f), EnemyAxisPreference.None),
                new(EnemyKind.HornedRabbitElite, new Vector2(420f, 172f), EnemyAxisPreference.None),
                new(EnemyKind.HornedRabbit, new Vector2(120f, 256f), EnemyAxisPreference.None),
                new(EnemyKind.HornedRabbit, new Vector2(220f, 256f), EnemyAxisPreference.None),
                new(EnemyKind.HornedRabbit, new Vector2(320f, 256f), EnemyAxisPreference.None),
                new(EnemyKind.HornedRabbit, new Vector2(420f, 256f), EnemyAxisPreference.None),
                new(EnemyKind.Bat, new Vector2(170f, 330f), EnemyAxisPreference.None),
                new(EnemyKind.Bat, new Vector2(270f, 330f), EnemyAxisPreference.None),
                new(EnemyKind.Bat, new Vector2(370f, 330f), EnemyAxisPreference.None)
            });

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
        private HashSet<GameAction> _justPressedActions;

        public StubInputService(params GameAction[] justPressedActions)
        {
            _justPressedActions = justPressedActions.ToHashSet();
        }

        public InputSnapshot Current => InputSnapshot.Empty;

        public InputSnapshot Previous => InputSnapshot.Empty;

        public bool IsPressed(GameAction action)
        {
            return false;
        }

        public bool IsJustPressed(GameAction action)
        {
            return _justPressedActions.Remove(action);
        }

        public bool IsJustReleased(GameAction action)
        {
            return false;
        }

        public void Update()
        {
        }
    }

    private static void AdvanceToSpawnedWave(global::MyGame.Gameplay.World.World world, int targetWave)
    {
        for (var waveNumber = 1; waveNumber < targetWave; waveNumber++)
        {
            ShowNextWaveIntro(world);
            if (world.IsObjectiveComplete)
            {
                return;
            }

            world.Update(new FrameTime(TimeSpan.FromSeconds(3.1), TimeSpan.FromSeconds(3.1)));
        }
    }

    private static void ShowNextWaveIntro(global::MyGame.Gameplay.World.World world)
    {
        DefeatLivingEnemies(world);
        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
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
