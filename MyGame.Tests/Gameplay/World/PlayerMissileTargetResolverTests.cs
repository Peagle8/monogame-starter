using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.World;

namespace MyGame.Tests.Gameplay.World;

public sealed class PlayerMissileTargetResolverTests
{
    [Fact]
    public void FindTarget_PrefersClosestEnemyInFrontOfProjectile()
    {
        var resolver = new PlayerMissileTargetResolver();
        var projectile = new PlayerProjectile(
            PlayerRangedAttackKind.Missile,
            new Vector2(100f, 100f),
            Direction.Right,
            100f,
            1f,
            24,
            2);
        var enemyBehind = new EnemyActor(new EnemySettings(), new Vector2(80f, 100f));
        var enemyAhead = new EnemyActor(new EnemySettings(), new Vector2(180f, 100f));

        var target = resolver.FindTarget(projectile, [enemyBehind, enemyAhead]);

        Assert.Same(enemyAhead, target);
    }

    [Fact]
    public void FindTarget_WhenNoEnemyIsInFront_FallsBackToClosestEnemy()
    {
        var resolver = new PlayerMissileTargetResolver();
        var projectile = new PlayerProjectile(
            PlayerRangedAttackKind.Missile,
            new Vector2(100f, 100f),
            Direction.Right,
            100f,
            1f,
            24,
            2);
        var closerEnemy = new EnemyActor(new EnemySettings(), new Vector2(90f, 100f));
        var fartherEnemy = new EnemyActor(new EnemySettings(), new Vector2(40f, 100f));

        var target = resolver.FindTarget(projectile, [fartherEnemy, closerEnemy]);

        Assert.Same(closerEnemy, target);
    }
}
