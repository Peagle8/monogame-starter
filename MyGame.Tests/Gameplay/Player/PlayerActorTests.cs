using MyGame.Configuration;
using MyGame.Core.Input;
using MyGame.Gameplay.Player;
using Microsoft.Xna.Framework;

namespace MyGame.Tests.Gameplay.Player;

public sealed class PlayerActorTests
{
    [Fact]
    public void Constructor_StartsWithDefaultHealth()
    {
        var player = new PlayerActor(
            new StubInputService(),
            new PlayerCombatSettings(),
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));

        Assert.Equal(20, player.CurrentHealth);
        Assert.Equal(20, player.MaxHealth);
        Assert.Equal(3f, player.CurrentAbilityPoints);
        Assert.Equal(3f, player.MaxAbilityPoints);
        Assert.False(player.IsShieldActive);
        Assert.False(player.IsFireShieldActive);
        Assert.Equal(0, player.ShieldCharges);
        Assert.False(player.IsDead);
    }

    [Fact]
    public void TakeDamage_ReducesHealthWithoutGoingBelowZero()
    {
        var player = new PlayerActor(
            new StubInputService(),
            new PlayerCombatSettings(),
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));

        player.TakeDamage(2);
        player.TakeDamage(30);

        Assert.Equal(0, player.CurrentHealth);
        Assert.True(player.IsDead);
    }

    [Fact]
    public void Update_RegeneratesAbilityPointsUpToMaximum()
    {
        var player = new PlayerActor(
            new StubInputService(),
            new PlayerCombatSettings { MaxAbilityPoints = 3f, AbilityPointRegenPerSecond = 0.05f },
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));
        Assert.True(player.TrySpendAbilityPoints(1f));

        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(20)));

        Assert.Equal(3f, player.CurrentAbilityPoints);
    }

    [Fact]
    public void TrySpendAbilityPoints_WhenEnoughPoints_ReducesCurrentAbilityPoints()
    {
        var player = new PlayerActor(
            new StubInputService(),
            new PlayerCombatSettings { MaxAbilityPoints = 3f },
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));

        var spent = player.TrySpendAbilityPoints(1.5f);

        Assert.True(spent);
        Assert.Equal(1.5f, player.CurrentAbilityPoints);
    }

    [Fact]
    public void TrySpendAbilityPoints_WhenNotEnoughPoints_DoesNotChangeCurrentAbilityPoints()
    {
        var player = new PlayerActor(
            new StubInputService(),
            new PlayerCombatSettings { MaxAbilityPoints = 2f },
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));

        var spent = player.TrySpendAbilityPoints(3f);

        Assert.False(spent);
        Assert.Equal(2f, player.CurrentAbilityPoints);
    }

    [Fact]
    public void Update_WhenAttackPressed_StartsPlayerAttack()
    {
        var player = new PlayerActor(
            new StubInputService(GameAction.Attack),
            new PlayerCombatSettings(),
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));

        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.True(player.IsAttacking);
        Assert.Equal(1, player.AttackSequence);
        Assert.Equal(new Microsoft.Xna.Framework.Rectangle(400, 272, 32, 30), player.AttackBounds);
    }

    [Fact]
    public void Update_WhenAttackingWhileMovingUpAndLeft_PrefersVerticalAttackDirection()
    {
        var player = new PlayerActor(
            new StubInputService(
                new InputSnapshot(new HashSet<GameAction> { GameAction.MoveUp, GameAction.MoveLeft }),
                GameAction.Attack),
            new PlayerCombatSettings(),
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));

        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.Equal(new Rectangle(389, 199, 32, 30), player.AttackBounds);
    }

    [Fact]
    public void Update_WhenRangedAttackPressed_SpawnsFireball()
    {
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.RangedAttack),
            new PlayerCombatSettings(),
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash, PlayerAbility.Fireball]),
            new PlayerAttackController(new PlayerAttackSettings()),
            new PlayerRangedAttackController(new PlayerRangedAttackSettings()));

        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        var projectiles = player.ConsumeSpawnedProjectiles();

        var projectile = Assert.Single(projectiles);
        Assert.Equal(PlayerRangedAttackKind.Fireball, projectile.Kind);
        Assert.Equal(Direction.Down, projectile.Direction);
        Assert.Equal(1, projectile.Damage);
        Assert.Equal(new Rectangle(404, 272, 24, 24), projectile.Bounds);
    }

    [Fact]
    public void Update_WhenRangedAttackPressedWhileMovingUpAndLeft_PrefersVerticalAttackDirection()
    {
        var player = new PlayerActor(
            new StubInputService(
                new InputSnapshot(new HashSet<GameAction> { GameAction.MoveUp, GameAction.MoveLeft }),
                GameAction.RangedAttack),
            new PlayerCombatSettings(),
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash, PlayerAbility.Fireball]),
            new PlayerAttackController(new PlayerAttackSettings()),
            new PlayerRangedAttackController(new PlayerRangedAttackSettings()));

        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        var projectile = Assert.Single(player.ConsumeSpawnedProjectiles());

        Assert.Equal(Direction.Up, projectile.Direction);
        Assert.Equal(new Rectangle(393, 205, 24, 24), projectile.Bounds);
    }

    [Fact]
    public void Update_WhenRangedAttackIsOnCooldown_DoesNotSpawnSecondFireball()
    {
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.RangedAttack),
            new PlayerCombatSettings(),
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash, PlayerAbility.Fireball]),
            new PlayerAttackController(new PlayerAttackSettings()),
            new PlayerRangedAttackController(new PlayerRangedAttackSettings { CooldownSeconds = 0.35f }));

        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        _ = player.ConsumeSpawnedProjectiles();

        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.2)));
        var secondProjectiles = player.ConsumeSpawnedProjectiles();

        Assert.Empty(secondProjectiles);
    }

    [Fact]
    public void Update_WhenDefenseAbilityPressed_ActivatesShieldAndSpendsAbilityPoints()
    {
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.DefenseAbility),
            new PlayerCombatSettings { MaxAbilityPoints = 3f, AbilityPointRegenPerSecond = 0f },
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash, PlayerAbility.Fireball]),
            new PlayerAttackController(new PlayerAttackSettings()),
            new PlayerDefenseAbilityController(new PlayerDefenseAbilitySettings()),
            new PlayerRangedAttackController(new PlayerRangedAttackSettings()));

        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.True(player.IsShieldActive);
        Assert.Equal(3, player.ShieldCharges);
        Assert.Equal(0f, player.CurrentAbilityPoints);
    }

    [Fact]
    public void Update_WhenDefenseAbilityPressedWithoutEnoughAbilityPoints_DoesNotActivateShield()
    {
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.DefenseAbility),
            new PlayerCombatSettings { MaxAbilityPoints = 2f, AbilityPointRegenPerSecond = 0f },
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash, PlayerAbility.Fireball]),
            new PlayerAttackController(new PlayerAttackSettings()),
            new PlayerDefenseAbilityController(new PlayerDefenseAbilitySettings()),
            new PlayerRangedAttackController(new PlayerRangedAttackSettings()));

        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.False(player.IsShieldActive);
        Assert.Equal(0, player.ShieldCharges);
        Assert.Equal(2f, player.CurrentAbilityPoints);
    }

    [Fact]
    public void Update_WhenShieldIsAlreadyActive_RepeatDefensePressDoesNotDeactivateOrReactivateIt()
    {
        var inputService = new StubInputService(InputSnapshot.Empty, GameAction.DefenseAbility);
        var player = new PlayerActor(
            inputService,
            new PlayerCombatSettings { MaxAbilityPoints = 6f, AbilityPointRegenPerSecond = 0f },
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash, PlayerAbility.Fireball]),
            new PlayerAttackController(new PlayerAttackSettings()),
            new PlayerDefenseAbilityController(new PlayerDefenseAbilitySettings()),
            new PlayerRangedAttackController(new PlayerRangedAttackSettings()));

        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        Assert.True(player.IsShieldActive);
        Assert.Equal(3, player.ShieldCharges);
        Assert.Equal(3f, player.CurrentAbilityPoints);

        inputService.SetJustPressedActions(GameAction.DefenseAbility);
        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.2)));

        Assert.True(player.IsShieldActive);
        Assert.Equal(3, player.ShieldCharges);
        Assert.Equal(3f, player.CurrentAbilityPoints);
    }

    [Fact]
    public void Update_WhenFireShieldEquippedAndDefenseAbilityPressed_ActivatesFireShieldAndSpendsAbilityPoints()
    {
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.DefenseAbility),
            new PlayerCombatSettings { MaxAbilityPoints = 3f, AbilityPointRegenPerSecond = 0f },
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash, PlayerAbility.Fireball]),
            new PlayerAttackController(new PlayerAttackSettings()),
            new PlayerDefenseAbilityController(new PlayerDefenseAbilitySettings()),
            new PlayerRangedAttackController(new PlayerRangedAttackSettings()));
        player.EquipDefenseAbility(PlayerDefenseAbilityKind.FireShield);

        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.True(player.IsFireShieldActive);
        Assert.False(player.IsShieldActive);
        Assert.Equal(0f, player.CurrentAbilityPoints);
        Assert.Equal(3, player.ShieldCharges);
    }

    [Fact]
    public void Update_WhenFireShieldIsActive_RemainsActiveWithoutTakingHits()
    {
        var inputService = new StubInputService(InputSnapshot.Empty, GameAction.DefenseAbility);
        var player = new PlayerActor(
            inputService,
            new PlayerCombatSettings { MaxAbilityPoints = 1f, AbilityPointRegenPerSecond = 0f },
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash, PlayerAbility.Fireball]),
            new PlayerAttackController(new PlayerAttackSettings()),
            new PlayerDefenseAbilityController(new PlayerDefenseAbilitySettings
            {
                FireShieldActivationCost = 1f
            }),
            new PlayerRangedAttackController(new PlayerRangedAttackSettings()));
        player.EquipDefenseAbility(PlayerDefenseAbilityKind.FireShield);

        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        Assert.True(player.IsFireShieldActive);

        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5.1)));

        Assert.True(player.IsFireShieldActive);
        Assert.Equal(3, player.ShieldCharges);
    }

    [Fact]
    public void TryAbsorbShieldHit_WhenShieldIsActive_ConsumesShieldCharge()
    {
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.DefenseAbility),
            new PlayerCombatSettings { MaxAbilityPoints = 3f, AbilityPointRegenPerSecond = 0f },
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash, PlayerAbility.Fireball]),
            new PlayerAttackController(new PlayerAttackSettings()),
            new PlayerDefenseAbilityController(new PlayerDefenseAbilitySettings()),
            new PlayerRangedAttackController(new PlayerRangedAttackSettings()));
        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.True(player.TryAbsorbShieldHit());
        Assert.True(player.IsShieldActive);
        Assert.Equal(2, player.ShieldCharges);
    }

    [Fact]
    public void TryAbsorbShieldHit_WhenFinalShieldChargeIsConsumed_BreaksShield()
    {
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.DefenseAbility),
            new PlayerCombatSettings { MaxAbilityPoints = 3f, AbilityPointRegenPerSecond = 0f },
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash, PlayerAbility.Fireball]),
            new PlayerAttackController(new PlayerAttackSettings()),
            new PlayerDefenseAbilityController(new PlayerDefenseAbilitySettings()),
            new PlayerRangedAttackController(new PlayerRangedAttackSettings()));
        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.True(player.TryAbsorbShieldHit());
        Assert.True(player.TryAbsorbShieldHit());
        Assert.True(player.TryAbsorbShieldHit());

        Assert.False(player.IsShieldActive);
        Assert.Equal(0, player.ShieldCharges);
    }

    [Fact]
    public void TryAbsorbShieldHit_WhenFireShieldIsActive_ConsumesShieldChargeAndEventuallyBreaksShield()
    {
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.DefenseAbility),
            new PlayerCombatSettings { MaxAbilityPoints = 3f, AbilityPointRegenPerSecond = 0f },
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash, PlayerAbility.Fireball]),
            new PlayerAttackController(new PlayerAttackSettings()),
            new PlayerDefenseAbilityController(new PlayerDefenseAbilitySettings()),
            new PlayerRangedAttackController(new PlayerRangedAttackSettings()));
        player.EquipDefenseAbility(PlayerDefenseAbilityKind.FireShield);
        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.True(player.TryAbsorbShieldHit());
        Assert.True(player.IsFireShieldActive);
        Assert.Equal(2, player.ShieldCharges);

        Assert.True(player.TryAbsorbShieldHit());
        Assert.True(player.TryAbsorbShieldHit());

        Assert.False(player.IsFireShieldActive);
        Assert.Equal(0, player.ShieldCharges);
    }

    [Fact]
    public void ApplyTransitionState_PreservesActiveShieldAndCharges()
    {
        var sourcePlayer = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.DefenseAbility),
            new PlayerCombatSettings { MaxAbilityPoints = 3f, AbilityPointRegenPerSecond = 0f },
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash, PlayerAbility.Fireball]),
            new PlayerAttackController(new PlayerAttackSettings()),
            new PlayerDefenseAbilityController(new PlayerDefenseAbilitySettings()),
            new PlayerRangedAttackController(new PlayerRangedAttackSettings()));
        sourcePlayer.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        Assert.True(sourcePlayer.TryAbsorbShieldHit());

        var targetPlayer = new PlayerActor(
            new StubInputService(),
            new PlayerCombatSettings { MaxAbilityPoints = 3f, AbilityPointRegenPerSecond = 0f },
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash, PlayerAbility.Fireball]),
            new PlayerAttackController(new PlayerAttackSettings()),
            new PlayerDefenseAbilityController(new PlayerDefenseAbilitySettings()),
            new PlayerRangedAttackController(new PlayerRangedAttackSettings()));

        targetPlayer.ApplyTransitionState(new Vector2(128f, 196f), sourcePlayer.CreateTransitionState());

        Assert.Equal(new Vector2(128f, 196f), targetPlayer.Position);
        Assert.True(targetPlayer.IsShieldActive);
        Assert.Equal(2, targetPlayer.ShieldCharges);
        Assert.Equal(sourcePlayer.CurrentAbilityPoints, targetPlayer.CurrentAbilityPoints);
    }

    [Fact]
    public void ApplyTransitionState_PreservesEquippedLoadoutSelections()
    {
        var sourcePlayer = new PlayerActor(
            new StubInputService(),
            new PlayerCombatSettings(),
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash, PlayerAbility.Fireball]),
            new PlayerAttackController(new PlayerAttackSettings()));
        sourcePlayer.EquipDashAbility(PlayerDashAbilityKind.BombDash);
        sourcePlayer.EquipDefenseAbility(PlayerDefenseAbilityKind.FireShield);
        sourcePlayer.EquipRangedAttack(PlayerRangedAttackKind.Bow);
        sourcePlayer.EquipMeleeAbility(PlayerMeleeAbilityKind.FireSword);

        var targetPlayer = new PlayerActor(
            new StubInputService(),
            new PlayerCombatSettings(),
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash, PlayerAbility.Fireball]),
            new PlayerAttackController(new PlayerAttackSettings()));

        targetPlayer.ApplyTransitionState(new Vector2(96f, 144f), sourcePlayer.CreateTransitionState());

        Assert.Equal(PlayerDashAbilityKind.BombDash, targetPlayer.EquippedDashAbility);
        Assert.Equal(PlayerDefenseAbilityKind.FireShield, targetPlayer.EquippedDefenseAbility);
        Assert.Equal(PlayerRangedAttackKind.Bow, targetPlayer.EquippedRangedAttack);
        Assert.Equal(PlayerMeleeAbilityKind.FireSword, targetPlayer.EquippedMeleeAbility);
    }

    [Fact]
    public void ApplyKnockback_MovesPlayerImmediatelyAndStartsRecoil()
    {
        var player = new PlayerActor(
            new StubInputService(),
            new PlayerCombatSettings(),
            new PlayerMovementController(new PlayerMovementSettings { ContactKnockbackDistance = 20f, ContactKnockbackSeconds = 0.2f }),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));

        player.ApplyKnockback(new Microsoft.Xna.Framework.Vector2(1f, 0f));

        Assert.Equal(new Microsoft.Xna.Framework.Vector2(410f, 240f), player.Position);
        Assert.True(player.IsRecoiling);
    }

    [Fact]
    public void Update_WhenRecoiling_ContinuesKnockbackAndSkipsInputMovement()
    {
        var player = new PlayerActor(
            new StubInputService(),
            new PlayerCombatSettings(),
            new PlayerMovementController(new PlayerMovementSettings { ContactKnockbackDistance = 20f, ContactKnockbackSeconds = 0.2f, MoveSpeed = 180f }),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));
        player.ApplyKnockback(new Microsoft.Xna.Framework.Vector2(1f, 0f));

        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.Equal(new Microsoft.Xna.Framework.Vector2(415f, 240f), player.Position);
        Assert.True(player.IsRecoiling);
    }

    [Fact]
    public void Update_WhenDashPressed_StartsDashing()
    {
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.Dash),
            new PlayerCombatSettings(),
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings { DashDistance = 72f, DashSeconds = 0.18f }),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));

        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.True(player.IsDashing);
        Assert.True(player.Position.Y > 240f);
    }

    [Fact]
    public void Update_WhenDashPressedWithoutDashAbility_DoesNotStartDashing()
    {
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.Dash),
            new PlayerCombatSettings(),
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings { DashDistance = 72f, DashSeconds = 0.18f }),
            new PlayerAbilityService(),
            new PlayerAttackController(new PlayerAttackSettings()));

        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.False(player.IsDashing);
        Assert.Equal(new Microsoft.Xna.Framework.Vector2(400f, 240f), player.Position);
    }

    [Fact]
    public void Update_WhenBombDashIsEquippedAndUnlocked_SpawnsTrailBombs()
    {
        var movementSettings = new PlayerMovementSettings
        {
            DashDistance = 72f,
            DashSeconds = 0.20f,
            DashCooldownSeconds = 0.35f
        };
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.Dash),
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash, PlayerAbility.BombDash]),
            new PlayerAttackController(new PlayerAttackSettings()));
        player.EquipDashAbility(PlayerDashAbilityKind.BombDash);

        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.05), TimeSpan.FromSeconds(0.05)));
        var firstBombs = player.ConsumeSpawnedBombs();

        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.09), TimeSpan.FromSeconds(0.14)));
        var secondBombs = player.ConsumeSpawnedBombs();

        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.05), TimeSpan.FromSeconds(0.19)));
        var thirdBombs = player.ConsumeSpawnedBombs();

        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.10), TimeSpan.FromSeconds(0.29)));
        var fourthBombs = player.ConsumeSpawnedBombs();

        Assert.Equal(2, firstBombs.Count);
        Assert.Equal(2, secondBombs.Count);
        Assert.Equal(2, thirdBombs.Count);
        Assert.Equal(4, fourthBombs.Count);
        Assert.Equal(10, firstBombs.Count + secondBombs.Count + thirdBombs.Count + fourthBombs.Count);
    }

    [Fact]
    public void Update_WhenBombDashIsEquippedButLocked_DoesNotStartDash()
    {
        var movementSettings = new PlayerMovementSettings
        {
            DashDistance = 72f,
            DashSeconds = 0.20f,
            DashCooldownSeconds = 0.35f
        };
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.Dash),
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));
        player.EquipDashAbility(PlayerDashAbilityKind.BombDash);

        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.05), TimeSpan.FromSeconds(0.05)));

        Assert.False(player.IsDashing);
        Assert.Empty(player.ConsumeSpawnedBombs());
        Assert.Equal(new Microsoft.Xna.Framework.Vector2(400f, 240f), player.Position);
    }

    [Fact]
    public void ApplyStun_PreventsMovementUntilDurationExpires()
    {
        var player = new PlayerActor(
            new StubInputService(new InputSnapshot(new HashSet<GameAction> { GameAction.MoveRight })),
            new PlayerCombatSettings(),
            new PlayerMovementController(new PlayerMovementSettings { MoveSpeed = 180f }),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));

        player.ApplyStun(2f);
        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)));

        Assert.True(player.IsStunned);
        Assert.Equal(new Vector2(400f, 240f), player.Position);

        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(1.1), TimeSpan.FromSeconds(2.1)));

        Assert.False(player.IsStunned);
    }

    [Fact]
    public void ApplyStun_WhenExpired_AllowsMovementAgain()
    {
        var inputService = new StubInputService(new InputSnapshot(new HashSet<GameAction> { GameAction.MoveRight }));
        var player = new PlayerActor(
            inputService,
            new PlayerCombatSettings(),
            new PlayerMovementController(new PlayerMovementSettings { MoveSpeed = 180f }),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));

        player.ApplyStun(0.5f);
        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5)));
        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(1.0)));

        Assert.False(player.IsStunned);
        Assert.Equal(new Vector2(490f, 240f), player.Position);
    }

    private sealed class StubInputService : IInputService
    {
        private HashSet<GameAction> _justPressedActions;
        private readonly InputSnapshot _current;

        public StubInputService(params GameAction[] justPressedActions)
        {
            _current = InputSnapshot.Empty;
            _justPressedActions = justPressedActions.ToHashSet();
        }

        public StubInputService(InputSnapshot current, params GameAction[] justPressedActions)
        {
            _current = current;
            _justPressedActions = justPressedActions.ToHashSet();
        }

        public InputSnapshot Current => _current;

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
            return _justPressedActions.Remove(action);
        }

        public bool IsJustReleased(GameAction action)
        {
            return false;
        }

        public void SetJustPressedActions(params GameAction[] justPressedActions)
        {
            _justPressedActions = justPressedActions.ToHashSet();
        }
    }
}
