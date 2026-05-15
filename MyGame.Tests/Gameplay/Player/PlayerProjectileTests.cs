using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Core;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;

namespace MyGame.Tests.Gameplay.Player;

public sealed class PlayerProjectileTests
{
    [Fact]
    public void Update_WhenMissileHasTarget_HomesTowardEnemy()
    {
        var projectile = new PlayerProjectile(
            PlayerRangedAttackKind.Missile,
            Vector2.Zero,
            Direction.Down,
            100f,
            1f,
            24,
            2);
        var enemy = new EnemyActor(new EnemySettings(), new Vector2(120f, 0f));
        projectile.AssignTarget(enemy);

        projectile.Update(new FrameTime(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5)));

        Assert.Equal(Direction.Right, projectile.Direction);
        Assert.True(projectile.Position.X > 40f);
        Assert.True(projectile.Position.Y < 10f);
    }

    [Fact]
    public void Update_WhenMissileHasNoTarget_ContinuesStraight()
    {
        var projectile = new PlayerProjectile(
            PlayerRangedAttackKind.Missile,
            Vector2.Zero,
            Direction.Down,
            100f,
            1f,
            24,
            2);

        projectile.Update(new FrameTime(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5)));

        Assert.Equal(Direction.Down, projectile.Direction);
        Assert.Equal(0f, projectile.Position.X);
        Assert.Equal(50f, projectile.Position.Y);
    }
}
