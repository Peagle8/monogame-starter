using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.Props;
using MyGame.Gameplay.World;

namespace MyGame.Tests.Gameplay.World;

public sealed class PlayerBombResolverTests
{
    [Fact]
    public void Resolve_WhenBombExplodes_DamagesEnemyAndRemovesGrass()
    {
        var bomb = new PlayerBomb(
            new Rectangle(100, 100, 12, 12),
            damage: 1,
            fuseSeconds: 0.01f,
            explosionDurationSeconds: 0.18f,
            explosionPadding: 12);
        bomb.Update(0.02f);

        var grass = new GrassProp(new Vector2(94f, 94f), new Point(30, 30));
        var enemy = new EnemyActor(
            new EnemySettings { MaxHealth = 3, MoveSpeed = 0f, ChaseRange = 10f },
            new Vector2(96f, 96f));
        var props = new List<IWorldProp> { grass };
        var resolver = new PlayerBombResolver();

        var hitEnemy = resolver.Resolve([bomb], props, [enemy]);

        Assert.True(hitEnemy);
        Assert.Equal(2, enemy.CurrentHealth);
        Assert.Empty(props.OfType<GrassProp>());
    }

    [Fact]
    public void Resolve_DoesNotDamageDeadEnemies()
    {
        var bomb = new PlayerBomb(
            new Rectangle(100, 100, 12, 12),
            damage: 1,
            fuseSeconds: 0.01f,
            explosionDurationSeconds: 0.18f,
            explosionPadding: 12);
        bomb.Update(0.02f);

        var enemy = new EnemyActor(
            new EnemySettings { MaxHealth = 1, MoveSpeed = 0f, ChaseRange = 10f },
            new Vector2(96f, 96f));
        enemy.TakeDamage(1);
        var resolver = new PlayerBombResolver();

        var hitEnemy = resolver.Resolve([bomb], [], [enemy]);

        Assert.False(hitEnemy);
        Assert.Equal(0, enemy.CurrentHealth);
    }
}
