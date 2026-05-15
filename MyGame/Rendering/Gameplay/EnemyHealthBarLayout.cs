using Microsoft.Xna.Framework;
using MyGame.Gameplay.Enemies;

namespace MyGame.Rendering.Gameplay;

public static class EnemyHealthBarLayout
{
    private const int BorderThickness = 1;
    private static readonly Point NormalBarSize = new(24, 5);
    private static readonly Point BossBarSize = new(38, 6);
    private static readonly Point AbilityBarSize = new(20, 4);

    public static Rectangle GetFrameBounds(EnemyActor enemy)
    {
        var barSize = IsBoss(enemy.Kind) ? BossBarSize : NormalBarSize;
        var visualTop = GetVisualTop(enemy);
        var verticalGap = IsBoss(enemy.Kind) ? 6 : 4;

        return new Rectangle(
            enemy.Bounds.Center.X - (barSize.X / 2),
            visualTop - verticalGap - barSize.Y,
            barSize.X,
            barSize.Y);
    }

    public static Rectangle GetBackgroundBounds(Rectangle frameBounds)
    {
        var innerWidth = Math.Max(0, frameBounds.Width - (BorderThickness * 2));
        var innerHeight = Math.Max(0, frameBounds.Height - (BorderThickness * 2));

        if (innerWidth == 0 || innerHeight == 0)
        {
            return Rectangle.Empty;
        }

        return new Rectangle(
            frameBounds.X + BorderThickness,
            frameBounds.Y + BorderThickness,
            innerWidth,
            innerHeight);
    }

    public static Rectangle GetFillBounds(Rectangle frameBounds, float currentValue, float maxValue)
    {
        var backgroundBounds = GetBackgroundBounds(frameBounds);
        if (backgroundBounds.IsEmpty || maxValue <= 0f || currentValue <= 0f)
        {
            return Rectangle.Empty;
        }

        var fillRatio = MathHelper.Clamp(currentValue / maxValue, 0f, 1f);
        var fillWidth = Math.Max(1, (int)MathF.Round(backgroundBounds.Width * fillRatio));
        return new Rectangle(backgroundBounds.X, backgroundBounds.Y, fillWidth, backgroundBounds.Height);
    }

    public static Rectangle GetAbilityFrameBounds(EnemyActor enemy)
    {
        if (enemy.MaxAbilityPoints <= 0f)
        {
            return Rectangle.Empty;
        }

        var healthFrameBounds = GetFrameBounds(enemy);
        return new Rectangle(
            healthFrameBounds.Center.X - (AbilityBarSize.X / 2),
            healthFrameBounds.Y - AbilityBarSize.Y - 2,
            AbilityBarSize.X,
            AbilityBarSize.Y);
    }

    private static bool IsBoss(EnemyKind kind)
    {
        return kind is EnemyKind.HornedRabbitBoss or EnemyKind.BatMiniBoss;
    }

    private static int GetVisualTop(EnemyActor enemy)
    {
        return enemy.Kind switch
        {
            EnemyKind.Crab => enemy.Bounds.Top + 6,
            EnemyKind.HornedRabbit => enemy.Bounds.Top - 1,
            EnemyKind.HornedRabbitBoss => enemy.Bounds.Top,
            EnemyKind.HornedRabbitElite => enemy.Bounds.Top - 1,
            EnemyKind.Bat => enemy.Bounds.Top - 24,
            EnemyKind.BatMiniBoss => enemy.Bounds.Top + 4,
            EnemyKind.Grasshopper => enemy.Bounds.Top + 4,
            EnemyKind.Skeleton => enemy.Bounds.Top + 1,
            EnemyKind.SkeletonElite => enemy.Bounds.Top + 2,
            _ => enemy.Bounds.Top
        };
    }
}
