using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Core;
using MyGame.Core.Input;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.Props;

namespace MyGame.Tests.Gameplay.World;

public sealed class WorldTests
{
    [Fact]
    public void Update_UpdatesPlayerState()
    {
        var inputService = new StubInputService(new InputSnapshot(new HashSet<GameAction> { GameAction.MoveRight }));
        var player = new PlayerActor(
            inputService,
            new PlayerMovementController(new PlayerMovementSettings { MoveSpeed = 180f }),
            new PlayerAttackController(new PlayerAttackSettings()));
        var enemy = new EnemyActor(new EnemySettings(), new Vector2(900f, 240f));
        var world = new global::MyGame.Gameplay.World.World(player, [], [enemy]);
        var frameTime = new FrameTime(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

        world.Update(frameTime);

        Assert.Equal(new Vector2(580f, 240f), world.Player.Position);
        Assert.Equal(Direction.Right, world.Player.Facing);
        Assert.True(world.Player.IsMoving);
    }

    [Fact]
    public void Update_WhenEnemyIntersectsPlayer_AppliesContactDamage()
    {
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty),
            new PlayerMovementController(new PlayerMovementSettings { MoveSpeed = 180f }),
            new PlayerAttackController(new PlayerAttackSettings()));
        var enemy = new EnemyActor(
            new EnemySettings { MoveSpeed = 0f, ChaseRange = 100f, RecoverySeconds = 0.65f },
            new Vector2(400f, 240f));
        var world = new global::MyGame.Gameplay.World.World(player, [], [enemy]);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.Equal(4, world.Player.CurrentHealth);
        Assert.Equal(EnemyState.Recovering, enemy.State);
    }

    [Fact]
    public void Update_WhenWithinContactCooldown_DoesNotApplyRepeatedDamage()
    {
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty),
            new PlayerMovementController(new PlayerMovementSettings { MoveSpeed = 180f }),
            new PlayerAttackController(new PlayerAttackSettings()));
        var enemy = new EnemyActor(
            new EnemySettings { MoveSpeed = 0f, ChaseRange = 100f, RecoverySeconds = 0.65f },
            new Vector2(400f, 240f));
        var world = new global::MyGame.Gameplay.World.World(player, [], [enemy]);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.2)));

        Assert.Equal(4, world.Player.CurrentHealth);
    }

    [Fact]
    public void Update_AfterCooldownExpires_AllowsContactDamageAgain()
    {
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty),
            new PlayerMovementController(new PlayerMovementSettings { MoveSpeed = 180f }),
            new PlayerAttackController(new PlayerAttackSettings()));
        var enemy = new EnemyActor(
            new EnemySettings { MoveSpeed = 0f, ChaseRange = 100f, RecoverySeconds = 0.65f },
            new Vector2(400f, 240f));
        var world = new global::MyGame.Gameplay.World.World(player, [], [enemy]);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(0.6), TimeSpan.FromSeconds(0.7)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.8)));

        Assert.Equal(3, world.Player.CurrentHealth);
    }

    [Fact]
    public void Update_WhenEnemyIsRecovering_DoesNotApplyDamageEvenIfStillIntersecting()
    {
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty),
            new PlayerMovementController(new PlayerMovementSettings { MoveSpeed = 180f }),
            new PlayerAttackController(new PlayerAttackSettings()));
        var enemy = new EnemyActor(
            new EnemySettings { MoveSpeed = 0f, ChaseRange = 100f, RecoverySeconds = 1.0f },
            new Vector2(400f, 240f));
        var world = new global::MyGame.Gameplay.World.World(player, [], [enemy]);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(0.6), TimeSpan.FromSeconds(0.7)));

        Assert.Equal(4, world.Player.CurrentHealth);
        Assert.Equal(EnemyState.Recovering, enemy.State);
    }

    [Fact]
    public void Constructor_StoresTreeProps()
    {
        var enemySettings = new EnemySettings();
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty),
            new PlayerMovementController(new PlayerMovementSettings { MoveSpeed = 180f }),
            new PlayerAttackController(new PlayerAttackSettings()));
        TreeProp[] treeProps =
        [
            new(new Vector2(10f, 20f), new Point(30, 40)),
            new(new Vector2(50f, 60f), new Point(70, 80))
        ];

        var enemies = new[] { new EnemyActor(enemySettings, new Vector2(100f, 100f)) };
        var world = new global::MyGame.Gameplay.World.World(player, treeProps, enemies, enemySettings);

        Assert.Equal(treeProps, world.TreeProps);
        Assert.Equal(enemies, world.Enemies);
    }

    [Fact]
    public void GetDebugState_ReturnsPlayerTreeAndEnemyDetails()
    {
        var inputService = new StubInputService(new InputSnapshot(new HashSet<GameAction> { GameAction.MoveUp }));
        var player = new PlayerActor(
            inputService,
            new PlayerMovementController(new PlayerMovementSettings { MoveSpeed = 60f }),
            new PlayerAttackController(new PlayerAttackSettings()));
        var enemy = new EnemyActor(new EnemySettings(), new Vector2(450f, 240f));
        var world = new global::MyGame.Gameplay.World.World(
            player,
            [new TreeProp(new Vector2(5f, 10f), new Point(16, 24))],
            [enemy]);
        var frameTime = new FrameTime(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

        world.Update(frameTime);
        var debugState = world.GetDebugState();

        Assert.Equal("400.00, 180.00", debugState["PlayerPosition"]);
        Assert.Equal("Up", debugState["PlayerFacing"]);
        Assert.Equal("1", debugState["TreePropCount"]);
        Assert.Equal("1", debugState["EnemyCount"]);
        Assert.Equal("0", debugState["DefeatedEnemyCount"]);
        Assert.Equal("Chasing", debugState["FirstEnemyState"]);
        Assert.Equal("False", debugState["PlayerAttackActive"]);
        Assert.Equal("5/5", debugState["PlayerHealth"]);
    }

    [Fact]
    public void Update_WhenPlayerAttackHitsEnemy_DealsDamageOncePerAttack()
    {
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.Attack),
            new PlayerMovementController(new PlayerMovementSettings { MoveSpeed = 180f }),
            new PlayerAttackController(new PlayerAttackSettings()));
        var enemy = new EnemyActor(
            new EnemySettings { MoveSpeed = 0f, ChaseRange = 20f },
            new Vector2(400f, 272f));
        var world = new global::MyGame.Gameplay.World.World(player, [], [enemy]);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(0.05), TimeSpan.FromSeconds(0.15)));

        Assert.Equal(enemy.MaxHealth - 1, enemy.CurrentHealth);
    }

    [Fact]
    public void Update_WhenPlayerAttackKillsEnemy_EnemyBecomesDead()
    {
        var enemySettings = new EnemySettings { MaxHealth = 2, MoveSpeed = 0f, ChaseRange = 20f };
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.Attack),
            new PlayerMovementController(new PlayerMovementSettings { MoveSpeed = 180f }),
            new PlayerAttackController(new PlayerAttackSettings { Damage = 3 }));
        var enemy = new EnemyActor(
            enemySettings,
            new Vector2(400f, 272f));
        var world = new global::MyGame.Gameplay.World.World(player, [], [enemy], enemySettings);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.Equal(EnemyState.Dead, enemy.State);
        Assert.Equal(0, enemy.CurrentHealth);
        Assert.Equal(1, world.DefeatedEnemyCount);
    }

    [Fact]
    public void CreateSaveData_IncludesEnemySnapshotsAndWorldState()
    {
        var enemySettings = new EnemySettings { MaxHealth = 2, MoveSpeed = 0f, ChaseRange = 20f };
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.Attack),
            new PlayerMovementController(new PlayerMovementSettings { MoveSpeed = 180f }),
            new PlayerAttackController(new PlayerAttackSettings { Damage = 3 }));
        var enemy = new EnemyActor(enemySettings, new Vector2(400f, 272f));
        var world = new global::MyGame.Gameplay.World.World(player, [], [enemy], enemySettings);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        var saveData = world.CreateSaveData("Gameplay");

        Assert.Equal("Gameplay", saveData.SceneName);
        Assert.Equal(1, saveData.DefeatedEnemyCount);
        Assert.Single(saveData.Enemies);
        Assert.Equal(400f, saveData.Enemies[0].PositionX);
        Assert.Equal(272f, saveData.Enemies[0].PositionY);
        Assert.Equal(0, saveData.Enemies[0].CurrentHealth);
    }

    [Fact]
    public void ApplySaveData_RestoresEnemyStateAndDefeatedCount()
    {
        var enemySettings = new EnemySettings { MaxHealth = 3, MoveSpeed = 120f, ChaseRange = 160f };
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty),
            new PlayerMovementController(new PlayerMovementSettings { MoveSpeed = 180f }),
            new PlayerAttackController(new PlayerAttackSettings()));
        var world = new global::MyGame.Gameplay.World.World(player, [], [], enemySettings);
        var saveData = new MyGame.Infrastructure.Save.SaveGameData
        {
            SceneName = "Gameplay",
            DefeatedEnemyCount = 1,
            Enemies =
            [
                new MyGame.Infrastructure.Save.EnemySaveData
                {
                    PositionX = 150f,
                    PositionY = 160f,
                    CurrentHealth = 2
                },
                new MyGame.Infrastructure.Save.EnemySaveData
                {
                    PositionX = 240f,
                    PositionY = 260f,
                    CurrentHealth = 0
                }
            ],
            PlayerHealth = 4,
            PlayerPositionX = 128f,
            PlayerPositionY = 196f
        };

        world.ApplySaveData(saveData);

        Assert.Equal(new Vector2(128f, 196f), world.Player.Position);
        Assert.Equal(4, world.Player.CurrentHealth);
        Assert.Equal(2, world.Enemies.Count);
        Assert.Equal(new Vector2(150f, 160f), world.Enemies[0].Position);
        Assert.Equal(2, world.Enemies[0].CurrentHealth);
        Assert.Equal(EnemyState.Idle, world.Enemies[0].State);
        Assert.Equal(new Vector2(240f, 260f), world.Enemies[1].Position);
        Assert.Equal(0, world.Enemies[1].CurrentHealth);
        Assert.Equal(EnemyState.Dead, world.Enemies[1].State);
        Assert.Equal(1, world.DefeatedEnemyCount);
    }

    private sealed class StubInputService : IInputService
    {
        private readonly HashSet<GameAction> _justPressedActions;

        public StubInputService(InputSnapshot current)
        {
            Current = current;
            _justPressedActions = [];
        }

        public StubInputService(InputSnapshot current, params GameAction[] justPressedActions)
        {
            Current = current;
            _justPressedActions = justPressedActions.ToHashSet();
        }

        public InputSnapshot Current { get; }

        public InputSnapshot Previous => InputSnapshot.Empty;

        public void Update()
        {
        }

        public bool IsPressed(GameAction action)
        {
            return Current.IsPressed(action);
        }

        public bool IsJustPressed(GameAction action)
        {
            return _justPressedActions.Contains(action);
        }

        public bool IsJustReleased(GameAction action)
        {
            return false;
        }
    }
}
