using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Core;
using MyGame.Gameplay.Enemies;

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
}
