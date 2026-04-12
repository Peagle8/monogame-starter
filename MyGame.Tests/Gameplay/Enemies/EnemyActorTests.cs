using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Core;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;

namespace MyGame.Tests.Gameplay.Enemies;

public sealed class EnemyActorTests
{
    [Fact]
    public void Constructor_UsesSettingsForInitialHealth()
    {
        var enemy = new EnemyActor(
            new EnemySettings { MaxHealth = 5, MoveSpeed = 120f, ChaseRange = 200f, RecoverySeconds = 0.65f, DefeatedVisibleSeconds = 0.8f },
            new Vector2(100f, 50f));

        Assert.Equal(5, enemy.CurrentHealth);
        Assert.Equal(5, enemy.MaxHealth);
        Assert.Equal(EnemyState.Idle, enemy.State);
    }

    [Fact]
    public void Update_WhenPlayerIsInRange_ChasesPlayerUsingConfiguredMoveSpeed()
    {
        var enemy = new EnemyActor(
            new EnemySettings { MaxHealth = 3, MoveSpeed = 90f, ChaseRange = 200f, RecoverySeconds = 0.65f, DefeatedVisibleSeconds = 0.8f },
            new Vector2(100f, 100f));

        enemy.Update(new Vector2(190f, 100f), new FrameTime(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)));

        Assert.Equal(new Vector2(190f, 100f), enemy.Position);
        Assert.True(enemy.IsMoving);
        Assert.Equal(EnemyState.Chasing, enemy.State);
    }

    [Fact]
    public void Update_WhenPlayerIsOutOfRange_RemainsIdle()
    {
        var enemy = new EnemyActor(
            new EnemySettings { MaxHealth = 3, MoveSpeed = 120f, ChaseRange = 50f, RecoverySeconds = 0.65f, DefeatedVisibleSeconds = 0.8f },
            new Vector2(100f, 100f));

        enemy.Update(new Vector2(300f, 100f), new FrameTime(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)));

        Assert.Equal(new Vector2(100f, 100f), enemy.Position);
        Assert.False(enemy.IsMoving);
        Assert.Equal(EnemyState.Idle, enemy.State);
    }

    [Fact]
    public void TakeDamage_WhenHealthDropsToZero_BecomesDead()
    {
        var enemy = new EnemyActor(new EnemySettings { MaxHealth = 2, DefeatedVisibleSeconds = 0.8f }, new Vector2(10f, 10f));

        enemy.TakeDamage(2);

        Assert.Equal(0, enemy.CurrentHealth);
        Assert.Equal(EnemyState.Dead, enemy.State);
        Assert.False(enemy.IsMoving);
        Assert.True(enemy.IsRenderable);
        Assert.True(enemy.IsFlashingFromHit);
    }

    [Fact]
    public void BeginRecovery_PutsEnemyIntoRecoveringState()
    {
        var enemy = new EnemyActor(
            new EnemySettings { RecoverySeconds = 0.5f },
            new Vector2(10f, 10f));

        enemy.BeginRecovery();

        Assert.Equal(EnemyState.Recovering, enemy.State);
        Assert.False(enemy.IsMoving);
        Assert.False(enemy.CanDealContactDamage);
    }

    [Fact]
    public void Update_WhenRecovering_DoesNotMoveUntilRecoveryExpires()
    {
        var enemy = new EnemyActor(
            new EnemySettings { MoveSpeed = 90f, ChaseRange = 200f, RecoverySeconds = 0.5f },
            new Vector2(100f, 100f));
        enemy.BeginRecovery();

        enemy.Update(new Vector2(190f, 100f), new FrameTime(TimeSpan.FromSeconds(0.25), TimeSpan.FromSeconds(0.25)));

        Assert.Equal(new Vector2(100f, 100f), enemy.Position);
        Assert.Equal(EnemyState.Recovering, enemy.State);
        Assert.False(enemy.CanDealContactDamage);
    }

    [Fact]
    public void Update_AfterRecoveryExpires_ResumesChasing()
    {
        var enemy = new EnemyActor(
            new EnemySettings { MoveSpeed = 90f, ChaseRange = 200f, RecoverySeconds = 0.25f },
            new Vector2(100f, 100f));
        enemy.BeginRecovery();

        enemy.Update(new Vector2(190f, 100f), new FrameTime(TimeSpan.FromSeconds(0.3), TimeSpan.FromSeconds(0.3)));

        Assert.Equal(new Vector2(127f, 100f), enemy.Position);
        Assert.Equal(EnemyState.Chasing, enemy.State);
        Assert.True(enemy.CanDealContactDamage);
    }

    [Fact]
    public void Update_WhenDead_CountsDownDefeatedVisibility()
    {
        var enemy = new EnemyActor(
            new EnemySettings { MaxHealth = 1, DefeatedVisibleSeconds = 0.5f },
            new Vector2(10f, 10f));
        enemy.TakeDamage(1);

        enemy.Update(Vector2.Zero, new FrameTime(TimeSpan.FromSeconds(0.2), TimeSpan.FromSeconds(0.2)));

        Assert.Equal(EnemyState.Dead, enemy.State);
        Assert.True(enemy.IsRenderable);
        Assert.True(enemy.DefeatedVisibilityAlpha < 1f);
    }

    [Fact]
    public void Update_AfterTakingDamage_CountsDownHitFlash()
    {
        var enemy = new EnemyActor(
            new EnemySettings { MaxHealth = 2 },
            new Vector2(10f, 10f));
        enemy.TakeDamage(1);

        enemy.Update(Vector2.Zero, new FrameTime(TimeSpan.FromSeconds(0.2), TimeSpan.FromSeconds(0.2)));

        Assert.False(enemy.IsFlashingFromHit);
        Assert.Equal(0f, enemy.HitFlashAlpha);
    }

    [Fact]
    public void Update_WhenDefeatedVisibilityExpires_IsNoLongerRenderable()
    {
        var enemy = new EnemyActor(
            new EnemySettings { MaxHealth = 1, DefeatedVisibleSeconds = 0.3f },
            new Vector2(10f, 10f));
        enemy.TakeDamage(1);

        enemy.Update(Vector2.Zero, new FrameTime(TimeSpan.FromSeconds(0.4), TimeSpan.FromSeconds(0.4)));

        Assert.False(enemy.IsRenderable);
        Assert.Equal(0f, enemy.DefeatedVisibilityAlpha);
    }

    [Fact]
    public void ApplyKnockback_MovesEnemyAwayUsingConfiguredDistance()
    {
        var enemy = new EnemyActor(
            new EnemySettings { PlayerHitKnockbackDistance = 12f, PlayerHitKnockbackSeconds = 0.2f },
            new Vector2(100f, 100f));

        enemy.ApplyKnockback(new Vector2(1f, 0f));

        Assert.Equal(new Vector2(106f, 100f), enemy.Position);
        Assert.True(enemy.IsMoving);
        Assert.Equal(EnemyState.Recovering, enemy.State);
    }

    [Fact]
    public void Update_AfterKnockbackApplied_ContinuesMovingDuringHitRecoil()
    {
        var enemy = new EnemyActor(
            new EnemySettings { PlayerHitKnockbackDistance = 12f, PlayerHitKnockbackSeconds = 0.2f },
            new Vector2(100f, 100f));
        enemy.ApplyKnockback(new Vector2(1f, 0f));

        enemy.Update(new Vector2(0f, 0f), new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.Equal(new Vector2(109f, 100f), enemy.Position);
        Assert.Equal(EnemyState.Recovering, enemy.State);
    }

    [Fact]
    public void Update_WhenHornedRabbitPlayerInRange_StartsDashInStraightLine()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbit);
        var enemy = new EnemyActor(settings, new Vector2(100f, 100f));

        enemy.Update(new Vector2(160f, 120f), new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.Equal(EnemyState.Dashing, enemy.State);
        Assert.True(enemy.IsMoving);
        Assert.Equal(Direction.Right, enemy.DashDirection);
        Assert.True(enemy.Position.X > 100f);
        Assert.Equal(100f, enemy.Position.Y);
        Assert.True(enemy.CanDealContactDamage);
    }

    [Fact]
    public void Update_WhenHornedRabbitDashEnds_EntersAimingPause()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbit);
        var enemy = new EnemyActor(settings, new Vector2(100f, 100f));

        enemy.Update(
            new Vector2(160f, 120f),
            new FrameTime(TimeSpan.FromSeconds(settings.DashSeconds), TimeSpan.FromSeconds(settings.DashSeconds)));

        Assert.Equal(EnemyState.Aiming, enemy.State);
        Assert.False(enemy.IsMoving);
        Assert.False(enemy.CanDealContactDamage);
    }

    [Fact]
    public void Update_WhenHornedRabbitPauseExpires_RetargetsAndDashesAgain()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbit);
        var enemy = new EnemyActor(settings, new Vector2(100f, 100f));

        enemy.Update(
            new Vector2(160f, 100f),
            new FrameTime(TimeSpan.FromSeconds(settings.DashSeconds), TimeSpan.FromSeconds(settings.DashSeconds)));
        var retargetPosition = new Vector2(enemy.Position.X, 10f);
        enemy.Update(
            retargetPosition,
            new FrameTime(TimeSpan.FromSeconds(settings.DashPauseSeconds - 0.01f), TimeSpan.FromSeconds(settings.DashSeconds + settings.DashPauseSeconds - 0.01f)));
        enemy.Update(
            retargetPosition,
            new FrameTime(TimeSpan.FromSeconds(0.02), TimeSpan.FromSeconds(settings.DashSeconds + settings.DashPauseSeconds + 0.01f)));

        Assert.Equal(EnemyState.Dashing, enemy.State);
        Assert.Equal(Direction.Up, enemy.DashDirection);
        Assert.True(enemy.Position.Y < 100f);
    }

    [Fact]
    public void Update_WhenHornedRabbitHasInitialDashPause_WaitsBeforeFirstDash()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbit);
        var enemy = new EnemyActor(settings, new Vector2(100f, 100f), initialDashPauseSeconds: 0.5f);

        enemy.Update(new Vector2(160f, 100f), new FrameTime(TimeSpan.FromSeconds(0.25), TimeSpan.FromSeconds(0.25)));

        Assert.Equal(EnemyState.Aiming, enemy.State);
        Assert.False(enemy.IsMoving);
        Assert.False(enemy.CanDealContactDamage);
    }

    [Fact]
    public void Update_WhenHornedRabbitPrefersHorizontal_UsesHorizontalDashEvenWhenVerticalGapIsLarger()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbit);
        var enemy = new EnemyActor(
            settings,
            new Vector2(100f, 100f),
            axisPreference: EnemyAxisPreference.Horizontal);

        enemy.Update(new Vector2(120f, 220f), new FrameTime(TimeSpan.FromSeconds(0.05), TimeSpan.FromSeconds(0.05)));

        Assert.Equal(Direction.Right, enemy.DashDirection);
        Assert.Equal(EnemyState.Dashing, enemy.State);
    }

    [Fact]
    public void Update_WhenHornedRabbitPrefersVertical_UsesVerticalDashEvenWhenHorizontalGapIsLarger()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbit);
        var enemy = new EnemyActor(
            settings,
            new Vector2(100f, 100f),
            axisPreference: EnemyAxisPreference.Vertical);

        enemy.Update(new Vector2(220f, 120f), new FrameTime(TimeSpan.FromSeconds(0.05), TimeSpan.FromSeconds(0.05)));

        Assert.Equal(Direction.Down, enemy.DashDirection);
        Assert.Equal(EnemyState.Dashing, enemy.State);
    }

    [Fact]
    public void Update_WhenHornedRabbitBossStartsAttack_LeapsIntoPlayersQuadrant()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbitBoss);
        var enemy = new EnemyActor(settings, new Vector2(360f, 180f));
        var playerBounds = new Rectangle(560, 320, 28, 28);

        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(1.2), TimeSpan.FromSeconds(1.2)));
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(0.01), TimeSpan.FromSeconds(1.21)));
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(0.2), TimeSpan.FromSeconds(1.41)));

        Assert.Equal(EnemyState.Dashing, enemy.State);
        Assert.True(enemy.Position.X > 360f);
        Assert.True(enemy.Position.Y > 180f);
    }

    [Fact]
    public void Update_WhenHornedRabbitBossLands_DropsBombSpread()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbitBoss);
        var enemy = new EnemyActor(settings, new Vector2(360f, 180f));
        var playerBounds = new Rectangle(120, 80, 28, 28);

        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(1.2), TimeSpan.FromSeconds(1.2)));
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(0.01), TimeSpan.FromSeconds(1.21)));
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(0.6), TimeSpan.FromSeconds(1.81)));

        var bombs = GetBombs(enemy).ToArray();

        Assert.Equal(EnemyState.Aiming, enemy.State);
        Assert.Equal(6, bombs.Length);
    }

    [Fact]
    public void Update_WhenHornedRabbitBossStageHealthIsDepleted_AdvancesToNextStage()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbitBoss);
        var enemy = new EnemyActor(settings, new Vector2(360f, 180f));
        var playerBounds = new Rectangle(120, 80, 28, 28);

        enemy.TakeDamage(6);
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.Equal(2, enemy.BossStage);
        Assert.Equal(3, enemy.BossStageCount);
        Assert.Equal(EnemyState.Aiming, enemy.State);
    }

    [Fact]
    public void Update_WhenHornedRabbitBossStageChanges_WaitsDuringPowerUpPause()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbitBoss);
        var enemy = new EnemyActor(settings, new Vector2(360f, 180f));
        var playerBounds = new Rectangle(560, 320, 28, 28);

        enemy.TakeDamage(6);
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0)));

        Assert.Equal(2, enemy.BossStage);
        Assert.Equal(EnemyState.Aiming, enemy.State);
        Assert.Equal(new Vector2(360f, 180f), enemy.Position);
    }

    [Fact]
    public void Update_WhenHornedRabbitBossStageTwoLandsFirstTime_QueuesThreeMinions()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbitBoss);
        var enemy = new EnemyActor(settings, new Vector2(360f, 180f));
        var playerBounds = new Rectangle(560, 320, 28, 28);

        enemy.TakeDamage(6);
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(1.5), TimeSpan.FromSeconds(1.5)));
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(1.1), TimeSpan.FromSeconds(2.6)));
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(0.01), TimeSpan.FromSeconds(2.61)));
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(3.11)));

        var pendingSpawns = GetPendingSpawns(enemy).OfType<global::MyGame.Gameplay.World.EnemySpawnDefinition>().ToArray();

        Assert.Equal(3, pendingSpawns.Length);
        Assert.All(pendingSpawns, spawn => Assert.Equal(EnemyKind.HornedRabbit, spawn.Kind));
    }

    [Fact]
    public void Update_WhenHornedRabbitBossIsInStageTwo_UsesArenaDashBetweenLeapAttacks()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbitBoss);
        var enemy = new EnemyActor(settings, new Vector2(360f, 180f));
        var playerBounds = new Rectangle(560, 320, 28, 28);

        enemy.TakeDamage(6);
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(1.5), TimeSpan.FromSeconds(1.5)));
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(1.1), TimeSpan.FromSeconds(2.6)));
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(0.01), TimeSpan.FromSeconds(2.61)));
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(3.11)));
        var positionAfterLeap = enemy.Position;

        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(1.1), TimeSpan.FromSeconds(4.21)));
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(0.01), TimeSpan.FromSeconds(4.22)));
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(4.32)));

        Assert.Equal(EnemyState.Dashing, enemy.State);
        Assert.True(enemy.Position.X > positionAfterLeap.X || enemy.Position.Y > positionAfterLeap.Y);
    }

    [Fact]
    public void Update_WhenHornedRabbitBossStageThreeLandsFirstTime_QueuesTwoElites()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbitBoss);
        var enemy = new EnemyActor(settings, new Vector2(360f, 180f));
        var playerBounds = new Rectangle(560, 320, 28, 28);

        enemy.TakeDamage(6);
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(1.5), TimeSpan.FromSeconds(1.5)));
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(1.1), TimeSpan.FromSeconds(2.6)));
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(0.01), TimeSpan.FromSeconds(2.61)));
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(3.11)));
        var stageTwoSpawns = GetPendingSpawns(enemy).ToArray();
        Assert.Equal(3, stageTwoSpawns.Length);
        ClearPendingSpawns(enemy);

        enemy.TakeDamage(6);
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(1.5), TimeSpan.FromSeconds(1.5)));
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(0.65), TimeSpan.FromSeconds(2.15)));
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(0.01), TimeSpan.FromSeconds(2.16)));
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(0.45), TimeSpan.FromSeconds(2.61)));

        var pendingSpawns = GetPendingSpawns(enemy).OfType<global::MyGame.Gameplay.World.EnemySpawnDefinition>().ToArray();

        Assert.Equal(5, pendingSpawns.Length);
        Assert.Equal(2, pendingSpawns.Count(spawn => spawn.Kind == EnemyKind.HornedRabbitElite));
        Assert.Equal(3, pendingSpawns.Count(spawn => spawn.Kind == EnemyKind.HornedRabbit));
    }

    [Fact]
    public void Update_WhenHornedRabbitBossIsInStageThree_StartsNextAttackFaster()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbitBoss);
        var enemy = new EnemyActor(settings, new Vector2(360f, 180f));
        var playerBounds = new Rectangle(560, 320, 28, 28);

        enemy.TakeDamage(6);
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(1.5), TimeSpan.FromSeconds(1.5)));
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(1.1), TimeSpan.FromSeconds(2.6)));
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(0.01), TimeSpan.FromSeconds(2.61)));
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(3.11)));
        ClearPendingSpawns(enemy);

        enemy.TakeDamage(6);
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(1.5), TimeSpan.FromSeconds(1.5)));
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(0.65), TimeSpan.FromSeconds(2.15)));
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(0.01), TimeSpan.FromSeconds(2.16)));
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(0.45), TimeSpan.FromSeconds(2.61)));
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(0.65), TimeSpan.FromSeconds(3.26)));
        enemy.Update(new Vector2(playerBounds.X, playerBounds.Y), playerBounds, new FrameTime(TimeSpan.FromSeconds(0.01), TimeSpan.FromSeconds(3.27)));

        Assert.Equal(EnemyState.Dashing, enemy.State);
    }

    [Fact]
    public void ApplyKnockback_WhenEnemyIsHornedRabbitBoss_DoesNothing()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbitBoss);
        var enemy = new EnemyActor(settings, new Vector2(360f, 180f));

        enemy.ApplyKnockback(new Vector2(1f, 0f));

        Assert.Equal(new Vector2(360f, 180f), enemy.Position);
        Assert.Equal(EnemyState.Idle, enemy.State);
    }

    [Fact]
    public void Update_WhenHornedRabbitEliteDashes_DropsBombsInItsWake()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbitElite);
        var enemy = new EnemyActor(settings, new Vector2(100f, 100f));

        enemy.Update(new Vector2(220f, 100f), new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        var bombs = GetBombs(enemy);

        Assert.Equal(EnemyState.Dashing, enemy.State);
        Assert.NotEmpty(bombs);
    }

    [Fact]
    public void Update_WhenHornedRabbitEliteBombFuseExpires_BombStartsExploding()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbitElite);
        var enemy = new EnemyActor(settings, new Vector2(100f, 100f));

        enemy.Update(new Vector2(220f, 100f), new FrameTime(TimeSpan.FromSeconds(0.05), TimeSpan.FromSeconds(0.05)));
        enemy.Update(new Vector2(220f, 100f), new FrameTime(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.55)));

        var bombs = GetBombs(enemy);

        Assert.Contains(bombs, bomb => (bool)(bomb.GetType().GetProperty("IsExploding")?.GetValue(bomb) ?? false));
    }

    [Fact]
    public void Update_WhenHornedRabbitElitePlayerIsOffset_DashesDiagonally()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbitElite);
        var enemy = new EnemyActor(settings, new Vector2(100f, 100f));

        enemy.Update(new Vector2(180f, 180f), new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.Equal(EnemyState.Dashing, enemy.State);
        Assert.True(enemy.Position.X > 100f);
        Assert.True(enemy.Position.Y > 100f);
    }

    [Fact]
    public void Update_WhenHornedRabbitElitePlayerIsBelow_DoesNotStayInHorizontalLane()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbitElite);
        var enemy = new EnemyActor(
            settings,
            new Vector2(100f, 100f),
            axisPreference: EnemyAxisPreference.Horizontal);

        enemy.Update(new Vector2(180f, 220f), new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.Equal(EnemyState.Dashing, enemy.State);
        Assert.True(enemy.Position.X > 100f);
        Assert.True(enemy.Position.Y > 100f);
    }

    [Fact]
    public void Update_WhenHornedRabbitElitePlayerIsArenaFarAway_StillKeepsAggro()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbitElite);
        var enemy = new EnemyActor(settings, new Vector2(40f, 40f));

        enemy.Update(new Vector2(760f, 420f), new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.Equal(EnemyState.Dashing, enemy.State);
        Assert.True(enemy.IsMoving);
    }

    [Fact]
    public void Update_WhenBatPlayerInRange_StartsCurvedSwoop()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.Bat);
        var enemy = new EnemyActor(settings, new Vector2(100f, 100f));

        enemy.Update(new Vector2(160f, 160f), new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.Equal(EnemyState.Dashing, enemy.State);
        Assert.True(enemy.IsMoving);
        Assert.True(enemy.CanDealContactDamage);
        Assert.True(enemy.Position.X > 100f);
        Assert.NotEqual(100f, enemy.Position.Y);
    }

    [Fact]
    public void Update_WhenBatSwoopEnds_EntersAimingPause()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.Bat);
        var enemy = new EnemyActor(settings, new Vector2(100f, 100f));

        enemy.Update(
            new Vector2(160f, 160f),
            new FrameTime(TimeSpan.FromSeconds(settings.DashSeconds), TimeSpan.FromSeconds(settings.DashSeconds)));

        Assert.Equal(EnemyState.Aiming, enemy.State);
        Assert.False(enemy.IsMoving);
        Assert.False(enemy.CanDealContactDamage);
    }

    [Fact]
    public void Update_WhenBatPauseExpires_StartsAnotherSwoop()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.Bat);
        var enemy = new EnemyActor(settings, new Vector2(100f, 100f));

        enemy.Update(
            new Vector2(160f, 160f),
            new FrameTime(TimeSpan.FromSeconds(settings.DashSeconds), TimeSpan.FromSeconds(settings.DashSeconds)));
        enemy.Update(
            new Vector2(160f, 160f),
            new FrameTime(TimeSpan.FromSeconds(settings.DashPauseSeconds - 0.01f), TimeSpan.FromSeconds(settings.DashSeconds + settings.DashPauseSeconds - 0.01f)));
        enemy.Update(
            new Vector2(160f, 160f),
            new FrameTime(TimeSpan.FromSeconds(0.02f), TimeSpan.FromSeconds(settings.DashSeconds + settings.DashPauseSeconds + 0.01f)));

        Assert.Equal(EnemyState.Dashing, enemy.State);
        Assert.True(enemy.CanDealContactDamage);
    }

    [Fact]
    public void ContactBounds_WhenBatIsDashing_AreWiderThanBodyBounds()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.Bat);
        var enemy = new EnemyActor(settings, new Vector2(100f, 100f));

        enemy.Update(new Vector2(160f, 160f), new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.True(enemy.ContactBounds.Width > enemy.Bounds.Width);
        Assert.True(enemy.ContactBounds.Height > enemy.Bounds.Height);
    }

    [Fact]
    public void Update_WhenBatMiniBossPlayerOutsideConeRange_ChasesPlayer()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.BatMiniBoss);
        var enemy = new EnemyActor(settings, new Vector2(100f, 100f));
        var playerBounds = new Rectangle(300, 100, 32, 32);

        enemy.Update(new Vector2(300f, 100f), playerBounds, new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.Equal(EnemyState.Chasing, enemy.State);
        Assert.True(enemy.IsMoving);
        Assert.True(enemy.Position.X > 100f);
    }

    [Fact]
    public void Update_WhenBatMiniBossInRange_ShowsConeTelegraphBeforeBlast()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.BatMiniBoss);
        var enemy = new EnemyActor(settings, new Vector2(100f, 100f));
        var playerBounds = new Rectangle(170, 100, 32, 32);

        enemy.Update(new Vector2(170f, 100f), playerBounds, new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.Equal(EnemyState.Aiming, enemy.State);
        Assert.True(enemy.IsSpecialAttackTelegraphVisible);
    }

    [Fact]
    public void Update_WhenBatMiniBossTargetIsInRangeButOutsideCone_RepositionsInsteadOfCharging()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.BatMiniBoss);
        var enemy = new EnemyActor(settings, new Vector2(100f, 100f));
        var playerBounds = new Rectangle(70, 210, 32, 32);

        enemy.Update(new Vector2(70f, 210f), playerBounds, new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.Equal(EnemyState.Chasing, enemy.State);
        Assert.True(enemy.IsMoving);
        Assert.False(enemy.IsSpecialAttackTelegraphVisible);
    }

    [Fact]
    public void Update_WhenBatMiniBossConeHits_QueuesAttackAndStartsFollowUpSwoop()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.BatMiniBoss);
        var enemy = new EnemyActor(settings, new Vector2(100f, 100f));
        var playerBounds = new Rectangle(176, 112, 32, 32);

        enemy.Update(
            new Vector2(176f, 112f),
            playerBounds,
            new FrameTime(TimeSpan.FromSeconds(settings.SpecialAttackPauseSeconds - 0.05f), TimeSpan.FromSeconds(settings.SpecialAttackPauseSeconds - 0.05f)));
        enemy.Update(
            new Vector2(176f, 112f),
            playerBounds,
            new FrameTime(TimeSpan.FromSeconds(0.1f), TimeSpan.FromSeconds(settings.SpecialAttackPauseSeconds + 0.1f)));

        Assert.True(enemy.IsSpecialAttackActive);
        Assert.Equal(EnemyState.Dashing, enemy.State);
    }

    [Fact]
    public void Update_WhenGrasshopperPlayerInRange_StartsFirstLeap()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.Grasshopper);
        var enemy = new EnemyActor(settings, new Vector2(100f, 100f));

        enemy.Update(new Vector2(160f, 140f), new FrameTime(TimeSpan.FromSeconds(0.05), TimeSpan.FromSeconds(0.05)));

        Assert.Equal(EnemyState.Dashing, enemy.State);
        Assert.True(enemy.IsMoving);
        Assert.True(enemy.CanDealContactDamage);
        Assert.True(enemy.Position.X > 100f);
        Assert.True(enemy.Position.Y > 100f);
    }

    [Fact]
    public void Update_WhenGrasshopperFirstLeapEnds_RepeatsLeapInSameDirection()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.Grasshopper);
        var enemy = new EnemyActor(settings, new Vector2(100f, 100f));

        enemy.Update(new Vector2(160f, 100f), new FrameTime(TimeSpan.FromSeconds(settings.DashSeconds), TimeSpan.FromSeconds(settings.DashSeconds)));
        var positionAfterFirstLeap = enemy.Position;
        enemy.Update(new Vector2(160f, 180f), new FrameTime(TimeSpan.FromSeconds(0.05), TimeSpan.FromSeconds(settings.DashSeconds + 0.05)));

        Assert.Equal(EnemyState.Dashing, enemy.State);
        Assert.Equal(Direction.Right, enemy.DashDirection);
        Assert.True(enemy.Position.X > positionAfterFirstLeap.X);
        Assert.Equal(positionAfterFirstLeap.Y, enemy.Position.Y);
    }

    [Fact]
    public void Update_WhenGrasshopperThirdLeapStarts_ChangesDirectionBeforePause()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.Grasshopper);
        var enemy = new EnemyActor(settings, new Vector2(100f, 100f));

        enemy.Update(new Vector2(160f, 100f), new FrameTime(TimeSpan.FromSeconds(settings.DashSeconds), TimeSpan.FromSeconds(settings.DashSeconds)));
        enemy.Update(new Vector2(160f, 100f), new FrameTime(TimeSpan.FromSeconds(settings.DashSeconds), TimeSpan.FromSeconds(settings.DashSeconds * 2f)));
        enemy.Update(new Vector2(160f, 180f), new FrameTime(TimeSpan.FromSeconds(0.05), TimeSpan.FromSeconds((settings.DashSeconds * 2f) + 0.05f)));

        Assert.Equal(EnemyState.Dashing, enemy.State);
        Assert.Equal(Direction.Down, enemy.DashDirection);
        Assert.True(enemy.Position.Y > 100f);
    }

    [Fact]
    public void Update_WhenGrasshopperThirdLeapEnds_EntersPause()
    {
        var settings = EnemySettingsCatalog.CreateDefault(EnemyKind.Grasshopper);
        var enemy = new EnemyActor(settings, new Vector2(100f, 100f));

        enemy.Update(new Vector2(160f, 100f), new FrameTime(TimeSpan.FromSeconds(settings.DashSeconds), TimeSpan.FromSeconds(settings.DashSeconds)));
        enemy.Update(new Vector2(160f, 100f), new FrameTime(TimeSpan.FromSeconds(settings.DashSeconds), TimeSpan.FromSeconds(settings.DashSeconds * 2f)));
        enemy.Update(new Vector2(160f, 180f), new FrameTime(TimeSpan.FromSeconds(settings.DashSeconds), TimeSpan.FromSeconds(settings.DashSeconds * 3f)));

        Assert.Equal(EnemyState.Aiming, enemy.State);
        Assert.False(enemy.IsMoving);
        Assert.False(enemy.CanDealContactDamage);
    }

    private static IEnumerable<object> GetBombs(EnemyActor enemy)
    {
        var bombsField = typeof(EnemyActor).GetField("_bombs", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        Assert.NotNull(bombsField);
        var bombs = bombsField!.GetValue(enemy);
        Assert.NotNull(bombs);
        return Assert.IsAssignableFrom<System.Collections.IEnumerable>(bombs).Cast<object>();
    }

    private static IEnumerable<object> GetPendingSpawns(EnemyActor enemy)
    {
        var pendingSpawnsField = typeof(EnemyActor).GetField("_pendingEnemySpawns", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        Assert.NotNull(pendingSpawnsField);
        var pendingSpawns = pendingSpawnsField!.GetValue(enemy);
        Assert.NotNull(pendingSpawns);
        return Assert.IsAssignableFrom<System.Collections.IEnumerable>(pendingSpawns).Cast<object>();
    }

    private static void ClearPendingSpawns(EnemyActor enemy)
    {
        var pendingSpawnsField = typeof(EnemyActor).GetField("_pendingEnemySpawns", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        Assert.NotNull(pendingSpawnsField);
        var pendingSpawns = pendingSpawnsField!.GetValue(enemy);
        var clearMethod = pendingSpawns?.GetType().GetMethod("Clear");
        Assert.NotNull(clearMethod);
        clearMethod!.Invoke(pendingSpawns, null);
    }
}
