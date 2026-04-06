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
}
