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
        var movementSettings = new PlayerMovementSettings { MoveSpeed = 180f };
        var player = new PlayerActor(
            inputService,
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash]),
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
        var movementSettings = new PlayerMovementSettings { MoveSpeed = 180f, ContactKnockbackDistance = 20f, ContactKnockbackSeconds = 0.2f };
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty),
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));
        var enemy = new EnemyActor(
            new EnemySettings { MoveSpeed = 0f, ChaseRange = 100f, RecoverySeconds = 0.65f },
            new Vector2(400f, 240f));
        var world = new global::MyGame.Gameplay.World.World(player, [], [enemy]);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.Equal(19, world.Player.CurrentHealth);
        Assert.Equal(new Vector2(400f, 230f), world.Player.Position);
        Assert.Equal(EnemyState.Recovering, enemy.State);
    }

    [Fact]
    public void Update_WhenShieldIsActive_EnemyContactConsumesShieldChargeInsteadOfDamagingPlayer()
    {
        var movementSettings = new PlayerMovementSettings { MoveSpeed = 180f, ContactKnockbackDistance = 20f, ContactKnockbackSeconds = 0.2f };
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.DefenseAbility),
            new PlayerCombatSettings { MaxAbilityPoints = 3f, AbilityPointRegenPerSecond = 0f },
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash, PlayerAbility.Fireball]),
            new PlayerAttackController(new PlayerAttackSettings()),
            new PlayerDefenseAbilityController(new PlayerDefenseAbilitySettings()),
            new PlayerRangedAttackController(new PlayerRangedAttackSettings()));
        var enemy = new EnemyActor(
            new EnemySettings { MoveSpeed = 0f, ChaseRange = 100f, RecoverySeconds = 0.65f },
            new Vector2(400f, 240f));
        var world = new global::MyGame.Gameplay.World.World(player, [], [enemy]);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.2)));

        Assert.Equal(20, world.Player.CurrentHealth);
        Assert.Equal(new Vector2(400f, 225f), world.Player.Position);
        Assert.True(world.Player.IsShieldActive);
        Assert.Equal(2, world.Player.ShieldCharges);
        Assert.Equal(EnemyState.Recovering, enemy.State);
    }

    [Fact]
    public void Update_WhenShieldIsActive_EnemyContactContinuesPlayerKnockbackMotion()
    {
        var movementSettings = new PlayerMovementSettings { MoveSpeed = 180f, ContactKnockbackDistance = 20f, ContactKnockbackSeconds = 0.2f };
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.DefenseAbility),
            new PlayerCombatSettings { MaxAbilityPoints = 3f, AbilityPointRegenPerSecond = 0f },
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash, PlayerAbility.Fireball]),
            new PlayerAttackController(new PlayerAttackSettings()),
            new PlayerDefenseAbilityController(new PlayerDefenseAbilitySettings()),
            new PlayerRangedAttackController(new PlayerRangedAttackSettings()));
        var enemy = new EnemyActor(
            new EnemySettings { MoveSpeed = 0f, ChaseRange = 100f, RecoverySeconds = 0.05f },
            new Vector2(400f, 240f));
        var world = new global::MyGame.Gameplay.World.World(player, [], [enemy]);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.2)));

        Assert.Equal(20, world.Player.CurrentHealth);
        Assert.Equal(new Vector2(400f, 225f), world.Player.Position);
    }

    [Fact]
    public void Update_WhenBatSwoopHitboxReachesPlayer_AppliesContactDamage()
    {
        var movementSettings = new PlayerMovementSettings
        {
            MoveSpeed = 180f,
            ContactKnockbackDistance = 20f,
            ContactKnockbackSeconds = 0.2f
        };
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty),
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));
        player.RestoreState(new Vector2(400f, 240f), player.MaxHealth);
        var bat = new EnemyActor(
            EnemySettingsCatalog.CreateDefault(EnemyKind.Bat),
            new Vector2(429f, 240f));
        var world = new global::MyGame.Gameplay.World.World(player, [], [bat]);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.Equal(19, world.Player.CurrentHealth);
        Assert.Equal(EnemyState.Recovering, bat.State);
    }

    [Fact]
    public void Update_WhenGrasshopperLeapHitboxReachesPlayer_AppliesContactDamage()
    {
        var movementSettings = new PlayerMovementSettings
        {
            MoveSpeed = 180f,
            ContactKnockbackDistance = 20f,
            ContactKnockbackSeconds = 0.2f
        };
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty),
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));
        player.RestoreState(new Vector2(400f, 240f), player.MaxHealth);
        var grasshopper = new EnemyActor(
            EnemySettingsCatalog.CreateDefault(EnemyKind.Grasshopper),
            new Vector2(420f, 240f));
        var world = new global::MyGame.Gameplay.World.World(player, [], [grasshopper]);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.05), TimeSpan.FromSeconds(0.05)));

        Assert.Equal(19, world.Player.CurrentHealth);
        Assert.Equal(EnemyState.Recovering, grasshopper.State);
    }

    [Fact]
    public void Update_WhenEnemiesOverlap_PushesThemApart()
    {
        var player = CreatePlayer();
        var worldCombatSettings = new global::MyGame.Gameplay.World.WorldCombatSettings
        {
            EnemySeparationDistance = 28f,
            EnemySeparationIterations = 2
        };
        var firstEnemy = new EnemyActor(
            new EnemySettings { MoveSpeed = 0f, ChaseRange = 10f },
            new Vector2(900f, 240f));
        var secondEnemy = new EnemyActor(
            new EnemySettings { MoveSpeed = 0f, ChaseRange = 10f },
            new Vector2(900f, 240f));
        var world = new global::MyGame.Gameplay.World.World(
            player,
            [],
            [firstEnemy, secondEnemy],
            worldCombatSettings: worldCombatSettings);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.Equal(new Vector2(886f, 240f), firstEnemy.Position);
        Assert.Equal(new Vector2(914f, 240f), secondEnemy.Position);
    }

    [Fact]
    public void Update_WhenEnemyIsDead_DoesNotSeparateItFromLivingEnemies()
    {
        var player = CreatePlayer();
        var aliveEnemy = new EnemyActor(
            new EnemySettings { MoveSpeed = 0f, ChaseRange = 10f },
            new Vector2(900f, 240f));
        var deadEnemy = new EnemyActor(
            new EnemySettings { MaxHealth = 1, MoveSpeed = 0f, ChaseRange = 10f },
            new Vector2(900f, 240f));
        deadEnemy.TakeDamage(1);
        var world = new global::MyGame.Gameplay.World.World(player, [], [aliveEnemy, deadEnemy]);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.Equal(new Vector2(900f, 240f), aliveEnemy.Position);
        Assert.Equal(new Vector2(900f, 240f), deadEnemy.Position);
    }

    [Fact]
    public void Update_WhenWithinContactCooldown_DoesNotApplyRepeatedDamage()
    {
        var movementSettings = new PlayerMovementSettings { MoveSpeed = 180f, ContactKnockbackDistance = 20f, ContactKnockbackSeconds = 0.2f };
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty),
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));
        var enemy = new EnemyActor(
            new EnemySettings { MoveSpeed = 0f, ChaseRange = 100f, RecoverySeconds = 0.65f },
            new Vector2(400f, 240f));
        var world = new global::MyGame.Gameplay.World.World(player, [], [enemy]);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.2)));

        Assert.Equal(19, world.Player.CurrentHealth);
    }

    [Fact]
    public void Update_AfterCooldownExpires_AllowsContactDamageAgain()
    {
        var movementSettings = new PlayerMovementSettings { MoveSpeed = 180f, ContactKnockbackDistance = 20f, ContactKnockbackSeconds = 0.2f };
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty),
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));
        var enemy = new EnemyActor(
            new EnemySettings { MoveSpeed = 0f, ChaseRange = 100f, RecoverySeconds = 0.65f },
            new Vector2(400f, 240f));
        var world = new global::MyGame.Gameplay.World.World(player, [], [enemy]);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(0.6), TimeSpan.FromSeconds(0.7)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.8)));

        Assert.Equal(18, world.Player.CurrentHealth);
    }

    [Fact]
    public void Update_WhenEnemyIsRecovering_DoesNotApplyDamageEvenIfStillIntersecting()
    {
        var movementSettings = new PlayerMovementSettings { MoveSpeed = 180f, ContactKnockbackDistance = 20f, ContactKnockbackSeconds = 0.2f };
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty),
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));
        var enemy = new EnemyActor(
            new EnemySettings { MoveSpeed = 0f, ChaseRange = 100f, RecoverySeconds = 1.0f },
            new Vector2(400f, 240f));
        var world = new global::MyGame.Gameplay.World.World(player, [], [enemy]);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(0.6), TimeSpan.FromSeconds(0.7)));

        Assert.Equal(19, world.Player.CurrentHealth);
        Assert.Equal(EnemyState.Recovering, enemy.State);
    }

    [Fact]
    public void Constructor_StoresTreeProps()
    {
        var enemySettings = new EnemySettings();
        var movementSettings = new PlayerMovementSettings { MoveSpeed = 180f };
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty),
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash]),
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
    public void Constructor_StoresGrassProps()
    {
        var player = CreatePlayer();
        IWorldProp[] props =
        [
            new TreeProp(new Vector2(10f, 20f), new Point(30, 40)),
            new GrassProp(new Vector2(50f, 60f), new Point(24, 18))
        ];
        var world = new global::MyGame.Gameplay.World.World(player, props, []);

        Assert.Single(world.GrassProps);
        Assert.Equal(new Rectangle(50, 60, 24, 18), world.GrassProps[0].Bounds);
    }

    [Fact]
    public void GetDebugState_ReturnsPlayerTreeAndEnemyDetails()
    {
        var inputService = new StubInputService(new InputSnapshot(new HashSet<GameAction> { GameAction.MoveUp }));
        var movementSettings = new PlayerMovementSettings { MoveSpeed = 60f };
        var player = new PlayerActor(
            inputService,
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));
        var enemy = new EnemyActor(new EnemySettings { MoveSpeed = 0f }, new Vector2(450f, 240f));
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
        Assert.Equal("0", debugState["GrassPropCount"]);
        Assert.Equal("1", debugState["PropCount"]);
        Assert.Equal("1", debugState["EnemyCount"]);
        Assert.Equal("0", debugState["DefeatedEnemyCount"]);
        Assert.Equal("Chasing", debugState["FirstEnemyState"]);
        Assert.Equal("False", debugState["PlayerAttackActive"]);
        Assert.Equal("3.00/3.00", debugState["PlayerAbilityPoints"]);
        Assert.Equal("20/20", debugState["PlayerHealth"]);
    }

    [Fact]
    public void Update_WhenEnemyMovesIntoBlockingTree_StopsAtTreeEdge()
    {
        var player = CreatePlayer();
        player.RestoreState(new Vector2(520f, 280f), player.MaxHealth);
        var enemy = new EnemyActor(
            new EnemySettings { MoveSpeed = 60f, ChaseRange = 300f },
            new Vector2(380f, 280f));
        IWorldProp[] props =
        [
            new TreeProp(new Vector2(420f, 230f), new Point(48, 64))
        ];
        var world = new global::MyGame.Gameplay.World.World(player, props, [enemy]);

        world.Update(new FrameTime(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)));

        Assert.Equal(new Vector2(408f, 280f), enemy.Position);
    }

    [Fact]
    public void Update_WhenEnemyMovesThroughGrass_GrassDoesNotBlockMovement()
    {
        var player = CreatePlayer();
        var enemy = new EnemyActor(
            new EnemySettings { MoveSpeed = 60f, ChaseRange = 300f },
            new Vector2(380f, 240f));
        IWorldProp[] props =
        [
            new GrassProp(new Vector2(420f, 230f), new Point(48, 64))
        ];
        var world = new global::MyGame.Gameplay.World.World(player, props, [enemy]);

        world.Update(new FrameTime(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)));

        Assert.Equal(new Vector2(440f, 240f), enemy.Position);
    }

    [Fact]
    public void Update_WhenPlayerMovesIntoBlockingTree_StopsAtTreeEdge()
    {
        var player = CreatePlayer(GameAction.MoveRight);
        player.RestoreState(new Vector2(400f, 280f), player.MaxHealth);
        IWorldProp[] props =
        [
            new TreeProp(new Vector2(460f, 230f), new Point(48, 64))
        ];
        var world = new global::MyGame.Gameplay.World.World(player, props, []);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5)));

        Assert.Equal(new Vector2(444f, 280f), world.Player.Position);
    }

    [Fact]
    public void Update_WhenPlayerMovesThroughGrass_GrassDoesNotBlockMovement()
    {
        var player = CreatePlayer(GameAction.MoveRight);
        IWorldProp[] props =
        [
            new GrassProp(new Vector2(420f, 230f), new Point(48, 64))
        ];
        var world = new global::MyGame.Gameplay.World.World(player, props, []);

        world.Update(new FrameTime(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)));

        Assert.Equal(new Vector2(580f, 240f), world.Player.Position);
    }

    [Fact]
    public void Update_WhenRangedAttackHitsEnemy_DealsDamage()
    {
        var player = CreateRangedPlayer(GameAction.RangedAttack);
        var enemy = new EnemyActor(
            new EnemySettings { MoveSpeed = 0f, ChaseRange = 20f },
            new Vector2(402f, 292f));
        var world = new global::MyGame.Gameplay.World.World(player, [], [enemy]);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.Equal(enemy.MaxHealth - 1, enemy.CurrentHealth);
        Assert.Empty(world.PlayerProjectiles);
    }

    [Fact]
    public void Update_WhenRangedAttackHitsTree_RemovesProjectile()
    {
        var player = CreateRangedPlayer(GameAction.RangedAttack);
        IWorldProp[] props =
        [
            new TreeProp(new Vector2(396f, 254f), new Point(48, 64))
        ];
        var world = new global::MyGame.Gameplay.World.World(player, props, []);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.Empty(world.PlayerProjectiles);
    }

    [Fact]
    public void Update_WhenRangedAttackPassesThroughGrass_ProjectileStaysActive()
    {
        var player = CreateRangedPlayer(GameAction.RangedAttack);
        IWorldProp[] props =
        [
            new GrassProp(new Vector2(396f, 284f), new Point(48, 64))
        ];
        var world = new global::MyGame.Gameplay.World.World(player, props, []);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.Single(world.PlayerProjectiles);
    }

    [Fact]
    public void Update_WhenPlayerAttackHitsEnemy_DealsDamageOncePerAttack()
    {
        var movementSettings = new PlayerMovementSettings { MoveSpeed = 180f };
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.Attack),
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash]),
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
    public void Update_AfterEnemyHitsPlayer_ContinuesPlayerKnockbackMotion()
    {
        var movementSettings = new PlayerMovementSettings { MoveSpeed = 180f, ContactKnockbackDistance = 20f, ContactKnockbackSeconds = 0.2f };
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty),
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));
        var enemy = new EnemyActor(
            new EnemySettings { MoveSpeed = 0f, ChaseRange = 100f, RecoverySeconds = 0.65f },
            new Vector2(400f, 240f));
        var world = new global::MyGame.Gameplay.World.World(player, [], [enemy]);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.2)));

        Assert.Equal(new Vector2(400f, 225f), world.Player.Position);
    }

    [Fact]
    public void Update_WhenPlayerAttackHitsEnemy_AppliesKnockback()
    {
        var enemySettings = new EnemySettings
        {
            MoveSpeed = 0f,
            ChaseRange = 20f,
            PlayerHitKnockbackDistance = 24f,
            PlayerHitKnockbackSeconds = 0.12f
        };
        var movementSettings = new PlayerMovementSettings { MoveSpeed = 180f };
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.Attack),
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));
        var enemy = new EnemyActor(
            enemySettings,
            new Vector2(400f, 272f));
        var world = new global::MyGame.Gameplay.World.World(player, [], [enemy], enemySettings);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.Equal(new Vector2(400f, 284f), enemy.Position);
        Assert.Equal(EnemyState.Recovering, enemy.State);
    }

    [Fact]
    public void Update_AfterPlayerAttackHit_ContinuesEnemyKnockbackMotion()
    {
        var enemySettings = new EnemySettings
        {
            MoveSpeed = 0f,
            ChaseRange = 20f,
            PlayerHitKnockbackDistance = 24f,
            PlayerHitKnockbackSeconds = 0.12f,
            PlayerHitPauseSeconds = 0.045f
        };
        var movementSettings = new PlayerMovementSettings { MoveSpeed = 180f };
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.Attack),
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));
        var enemy = new EnemyActor(
            enemySettings,
            new Vector2(400f, 272f));
        var world = new global::MyGame.Gameplay.World.World(player, [], [enemy], enemySettings);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(0.06), TimeSpan.FromSeconds(0.16)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(0.06), TimeSpan.FromSeconds(0.22)));

        Assert.Equal(new Vector2(400f, 290f), enemy.Position);
    }

    [Fact]
    public void Update_AfterPlayerAttackHit_AppliesShortHitPause()
    {
        var enemySettings = new EnemySettings
        {
            MoveSpeed = 0f,
            ChaseRange = 200f,
            PlayerHitKnockbackDistance = 24f,
            PlayerHitKnockbackSeconds = 0.12f,
            PlayerHitPauseSeconds = 0.045f
        };
        var movementSettings = new PlayerMovementSettings { MoveSpeed = 180f };
        var player = new PlayerActor(
            new StubInputService(new InputSnapshot(new HashSet<GameAction> { GameAction.MoveRight }), GameAction.Attack),
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));
        var enemy = new EnemyActor(
            enemySettings,
            new Vector2(418f, 272f));
        var world = new global::MyGame.Gameplay.World.World(player, [], [enemy], enemySettings);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        var enemyPositionAfterHit = enemy.Position;

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.02), TimeSpan.FromSeconds(0.12)));

        Assert.Equal(enemyPositionAfterHit, enemy.Position);
    }

    [Fact]
    public void Update_WhenPlayerAttackKillsEnemy_EnemyBecomesDead()
    {
        var enemySettings = new EnemySettings { MaxHealth = 2, MoveSpeed = 0f, ChaseRange = 20f };
        var movementSettings = new PlayerMovementSettings { MoveSpeed = 180f };
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.Attack),
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash]),
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
    public void Update_WhenPlayerDefeatsEnemy_AwardsOneAbilityPoint()
    {
        var enemySettings = new EnemySettings { MaxHealth = 1, MoveSpeed = 0f, ChaseRange = 20f };
        var movementSettings = new PlayerMovementSettings { MoveSpeed = 180f };
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.Attack),
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings { Damage = 1 }));
        Assert.True(player.TrySpendAbilityPoints(1f));
        var enemy = new EnemyActor(
            enemySettings,
            new Vector2(400f, 272f));
        var world = new global::MyGame.Gameplay.World.World(player, [], [enemy], enemySettings);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.Equal(3f, world.Player.CurrentAbilityPoints);
        Assert.Equal(1, world.DefeatedEnemyCount);
    }

    [Fact]
    public void Update_WhenPlayerDefeatsEnemy_AddsAbilityPointToastAbovePlayer()
    {
        var enemySettings = new EnemySettings { MaxHealth = 1, MoveSpeed = 0f, ChaseRange = 20f };
        var movementSettings = new PlayerMovementSettings { MoveSpeed = 180f };
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.Attack),
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings { Damage = 1 }));
        Assert.True(player.TrySpendAbilityPoints(1f));
        var enemy = new EnemyActor(
            enemySettings,
            new Vector2(400f, 272f));
        var world = new global::MyGame.Gameplay.World.World(player, [], [enemy], enemySettings);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        var toast = Assert.Single(world.Toasts);
        Assert.Equal("+1 AP", toast.Text);
        Assert.Equal(new Vector2(world.Player.Bounds.Center.X, world.Player.Bounds.Top - 4f), toast.Position);
    }

    [Fact]
    public void Update_WhenPlayerDefeatsEnemyAtMaxAbilityPoints_DoesNotAddAbilityPointToast()
    {
        var enemySettings = new EnemySettings { MaxHealth = 1, MoveSpeed = 0f, ChaseRange = 20f };
        var movementSettings = new PlayerMovementSettings { MoveSpeed = 180f };
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.Attack),
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings { Damage = 1 }));
        var enemy = new EnemyActor(
            enemySettings,
            new Vector2(400f, 272f));
        var world = new global::MyGame.Gameplay.World.World(player, [], [enemy], enemySettings);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.Empty(world.Toasts);
        Assert.Equal(player.MaxAbilityPoints, world.Player.CurrentAbilityPoints);
    }

    [Fact]
    public void Update_AfterAbilityPointToastLifetimeExpires_RemovesToast()
    {
        var enemySettings = new EnemySettings { MaxHealth = 1, MoveSpeed = 0f, ChaseRange = 20f };
        var movementSettings = new PlayerMovementSettings { MoveSpeed = 180f };
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.Attack),
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings { Damage = 1 }));
        Assert.True(player.TrySpendAbilityPoints(1f));
        var enemy = new EnemyActor(
            enemySettings,
            new Vector2(400f, 272f));
        var world = new global::MyGame.Gameplay.World.World(player, [], [enemy], enemySettings);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        Assert.Single(world.Toasts);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.8), TimeSpan.FromSeconds(0.9)));
        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(1.0)));

        Assert.Empty(world.Toasts);
    }

    [Fact]
    public void CreateSaveData_IncludesEnemySnapshotsAndWorldState()
    {
        var enemySettings = new EnemySettings { MaxHealth = 2, MoveSpeed = 0f, ChaseRange = 20f };
        var movementSettings = new PlayerMovementSettings { MoveSpeed = 180f };
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.Attack),
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings { Damage = 3 }));
        var enemy = new EnemyActor(enemySettings, new Vector2(400f, 272f));
        var world = new global::MyGame.Gameplay.World.World(player, [], [enemy], enemySettings);

        world.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        var saveData = world.CreateSaveData("Gameplay");

        Assert.Equal("Gameplay", saveData.SceneName);
        Assert.Equal(1, saveData.DefeatedEnemyCount);
        Assert.Single(saveData.Enemies);
        Assert.Equal(3f, saveData.PlayerAbilityPoints);
        Assert.Equal(EnemyKind.Crab, saveData.Enemies[0].Kind);
        Assert.Equal(EnemyAxisPreference.None, saveData.Enemies[0].AxisPreference);
        Assert.Equal(400f, saveData.Enemies[0].PositionX);
        Assert.Equal(272f, saveData.Enemies[0].PositionY);
        Assert.Equal(0, saveData.Enemies[0].CurrentHealth);
    }

    [Fact]
    public void ApplySaveData_RestoresEnemyStateAndDefeatedCount()
    {
        var enemySettings = new EnemySettings { MaxHealth = 3, MoveSpeed = 120f, ChaseRange = 160f };
        var movementSettings = new PlayerMovementSettings { MoveSpeed = 180f };
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty),
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash]),
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
                    Kind = EnemyKind.Crab,
                    AxisPreference = EnemyAxisPreference.None,
                    PositionX = 150f,
                    PositionY = 160f,
                    CurrentHealth = 2
                },
                new MyGame.Infrastructure.Save.EnemySaveData
                {
                    Kind = EnemyKind.Crab,
                    AxisPreference = EnemyAxisPreference.None,
                    PositionX = 240f,
                    PositionY = 260f,
                    CurrentHealth = 0
                }
            ],
            PlayerAbilityPoints = 1.5f,
            PlayerHealth = 4,
            PlayerPositionX = 128f,
            PlayerPositionY = 196f
        };

        world.ApplySaveData(saveData);

        Assert.Equal(new Vector2(128f, 196f), world.Player.Position);
        Assert.Equal(4, world.Player.CurrentHealth);
        Assert.Equal(1.5f, world.Player.CurrentAbilityPoints);
        Assert.Equal(2, world.Enemies.Count);
        Assert.Equal(new Vector2(150f, 160f), world.Enemies[0].Position);
        Assert.Equal(2, world.Enemies[0].CurrentHealth);
        Assert.Equal(EnemyState.Idle, world.Enemies[0].State);
        Assert.Equal(new Vector2(240f, 260f), world.Enemies[1].Position);
        Assert.Equal(0, world.Enemies[1].CurrentHealth);
        Assert.Equal(EnemyState.Dead, world.Enemies[1].State);
        Assert.Equal(1, world.DefeatedEnemyCount);
    }

    [Fact]
    public void ApplySaveData_DoesNotAwardAbilityPointsForAlreadyDefeatedEnemies()
    {
        var enemySettings = new EnemySettings { MaxHealth = 3, MoveSpeed = 120f, ChaseRange = 160f };
        var movementSettings = new PlayerMovementSettings { MoveSpeed = 180f };
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty),
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash]),
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
                    Kind = EnemyKind.Crab,
                    AxisPreference = EnemyAxisPreference.None,
                    PositionX = 240f,
                    PositionY = 260f,
                    CurrentHealth = 0
                }
            ],
            PlayerAbilityPoints = 1.5f,
            PlayerHealth = 4,
            PlayerPositionX = 128f,
            PlayerPositionY = 196f
        };

        world.ApplySaveData(saveData);

        Assert.Equal(1.5f, world.Player.CurrentAbilityPoints);
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
            return _justPressedActions.Remove(action);
        }

        public bool IsJustReleased(GameAction action)
        {
            return false;
        }
    }

    private static PlayerActor CreatePlayer(params GameAction[] pressedActions)
    {
        var movementSettings = new PlayerMovementSettings { MoveSpeed = 180f };
        return new PlayerActor(
            new StubInputService(new InputSnapshot(pressedActions.ToHashSet())),
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash, PlayerAbility.Fireball]),
            new PlayerAttackController(new PlayerAttackSettings()));
    }

    private static PlayerActor CreateRangedPlayer(params GameAction[] justPressedActions)
    {
        var movementSettings = new PlayerMovementSettings { MoveSpeed = 180f };
        return new PlayerActor(
            new StubInputService(InputSnapshot.Empty, justPressedActions),
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash, PlayerAbility.Fireball]),
            new PlayerAttackController(new PlayerAttackSettings()),
            new PlayerRangedAttackController(new PlayerRangedAttackSettings()));
    }
}


