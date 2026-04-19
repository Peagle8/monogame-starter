using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Gameplay.Enemies;
using MyGame.Rendering.Gameplay;

namespace MyGame.Tests.Rendering.Gameplay;

public sealed class EnemyHealthBarLayoutTests
{
    [Fact]
    public void GetFrameBounds_ForCrab_CentersNormalBarAboveEnemy()
    {
        var enemy = new EnemyActor(EnemySettingsCatalog.CreateDefault(EnemyKind.Crab), new Vector2(100f, 120f));

        var bounds = EnemyHealthBarLayout.GetFrameBounds(enemy);

        Assert.Equal(new Rectangle(102, 117, 24, 5), bounds);
    }

    [Fact]
    public void GetFrameBounds_ForBat_PlacesBarAboveExpandedSpriteHeight()
    {
        var enemy = new EnemyActor(EnemySettingsCatalog.CreateDefault(EnemyKind.Bat), new Vector2(100f, 120f));

        var bounds = EnemyHealthBarLayout.GetFrameBounds(enemy);

        Assert.Equal(new Rectangle(102, 87, 24, 5), bounds);
        Assert.True(bounds.Bottom < enemy.Bounds.Top);
    }

    [Fact]
    public void GetFrameBounds_ForBoss_UsesLargerBar()
    {
        var normalEnemy = new EnemyActor(EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbit), new Vector2(100f, 120f));
        var bossEnemy = new EnemyActor(EnemySettingsCatalog.CreateDefault(EnemyKind.BatMiniBoss), new Vector2(100f, 120f));

        var normalBounds = EnemyHealthBarLayout.GetFrameBounds(normalEnemy);
        var bossBounds = EnemyHealthBarLayout.GetFrameBounds(bossEnemy);

        Assert.True(bossBounds.Width > normalBounds.Width);
        Assert.True(bossBounds.Height > normalBounds.Height);
    }

    [Fact]
    public void GetFillBounds_WhenHealthIsHalf_FillsHalfOfInnerBarWidth()
    {
        var frameBounds = new Rectangle(102, 117, 24, 5);

        var fillBounds = EnemyHealthBarLayout.GetFillBounds(frameBounds, 2f, 4f);

        Assert.Equal(new Rectangle(103, 118, 11, 3), fillBounds);
    }
}
