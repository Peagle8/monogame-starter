using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;

namespace MyGame.Rendering.Enemies;

public sealed class HornedRabbitBossEnemyRenderer : IEnemyKindRenderer
{
    private static readonly Color DefeatedTint = new(137, 86, 84);
    private static readonly Color HitFlashTint = new(255, 240, 220);
    private static readonly Color StageOneBodyTint = new(191, 170, 154);
    private static readonly Color StageTwoBodyTint = new(172, 145, 120);
    private static readonly Color StageThreeBodyTint = new(148, 118, 98);
    private static readonly Color StageOneHornTint = new(255, 228, 150);
    private static readonly Color StageTwoHornTint = new(255, 188, 120);
    private static readonly Color StageThreeHornTint = new(255, 132, 108);
    private static readonly Color LeapTrailTint = new(245, 196, 128, 120);
    private static readonly Color BombOuterColor = new(248, 210, 84);
    private static readonly Color BombInnerColor = new(255, 244, 164);
    private static readonly Color FuseColor = new(255, 238, 182);
    private static readonly Color ExplosionColor = new(255, 188, 90);

    private readonly IWorldRectangleRenderer _worldRectangleRenderer;

    public HornedRabbitBossEnemyRenderer(IWorldRectangleRenderer worldRectangleRenderer)
    {
        _worldRectangleRenderer = worldRectangleRenderer;
    }

    public EnemyKind Kind => EnemyKind.HornedRabbitBoss;

    public void Draw(EnemyActor enemy, FrameTime frameTime)
    {
        foreach (var bomb in enemy.Bombs)
        {
            DrawBomb(bomb);
        }

        var baseBodyColor = GetBodyColor(enemy.BossStage);
        var baseHornColor = GetHornColor(enemy.BossStage);
        var bodyColor = enemy.State == EnemyState.Dead
            ? DefeatedTint * Math.Max(enemy.DefeatedVisibilityAlpha, 0.35f)
            : baseBodyColor;
        var hornColor = enemy.State == EnemyState.Dead
            ? DefeatedTint * Math.Max(enemy.DefeatedVisibilityAlpha, 0.35f)
            : baseHornColor;

        if (enemy.IsFlashingFromHit)
        {
            bodyColor = Color.Lerp(bodyColor, HitFlashTint, enemy.HitFlashAlpha);
            hornColor = Color.Lerp(hornColor, HitFlashTint, enemy.HitFlashAlpha);
        }

        if (enemy.State == EnemyState.Dashing)
        {
            _worldRectangleRenderer.Draw(GetLeapTrail(enemy), LeapTrailTint);
        }

        DrawRabbit(enemy, bodyColor, hornColor);
    }

    private void DrawRabbit(EnemyActor enemy, Color bodyColor, Color hornColor)
    {
        var body = new Rectangle(enemy.Bounds.X + 8, enemy.Bounds.Y + 18, 32, 22);
        var head = new Rectangle(enemy.Bounds.X + 28, enemy.Bounds.Y + 8, 16, 14);
        var backLeg = new Rectangle(enemy.Bounds.X + 6, enemy.Bounds.Y + 30, 10, 12);
        var frontLeg = new Rectangle(enemy.Bounds.X + 28, enemy.Bounds.Y + 32, 10, 10);
        var leftHorn = new Rectangle(enemy.Bounds.X + 30, enemy.Bounds.Y + 1, 5, 10);
        var rightHorn = new Rectangle(enemy.Bounds.X + 38, enemy.Bounds.Y + 0, 5, 11);

        _worldRectangleRenderer.Draw(body, bodyColor);
        _worldRectangleRenderer.Draw(head, bodyColor);
        _worldRectangleRenderer.Draw(backLeg, bodyColor * 0.88f);
        _worldRectangleRenderer.Draw(frontLeg, bodyColor * 0.92f);
        _worldRectangleRenderer.Draw(leftHorn, hornColor);
        _worldRectangleRenderer.Draw(rightHorn, hornColor);
    }

    private void DrawBomb(EnemyBomb bomb)
    {
        if (bomb.IsExploding)
        {
            _worldRectangleRenderer.Draw(bomb.ExplosionBounds, ExplosionColor * Math.Max(bomb.ExplosionAlpha, 0.35f));
            return;
        }

        _worldRectangleRenderer.Draw(bomb.Bounds, BombOuterColor);
        var innerBounds = new Rectangle(bomb.Bounds.X + 2, bomb.Bounds.Y + 2, bomb.Bounds.Width - 4, bomb.Bounds.Height - 4);
        _worldRectangleRenderer.Draw(innerBounds, BombInnerColor);
        var fuseBounds = new Rectangle(bomb.Bounds.X + 4, bomb.Bounds.Y - 2, 4, 4);
        _worldRectangleRenderer.Draw(fuseBounds, FuseColor);
    }

    private static Rectangle GetLeapTrail(EnemyActor enemy)
    {
        return enemy.DashDirection switch
        {
            Direction.Up => new Rectangle(enemy.Bounds.X + 16, enemy.Bounds.Y + 34, 16, 18),
            Direction.Down => new Rectangle(enemy.Bounds.X + 16, enemy.Bounds.Y - 10, 16, 18),
            Direction.Left => new Rectangle(enemy.Bounds.X + 30, enemy.Bounds.Y + 18, 18, 14),
            Direction.Right => new Rectangle(enemy.Bounds.X - 10, enemy.Bounds.Y + 18, 18, 14),
            _ => enemy.Bounds
        };
    }

    private static Color GetBodyColor(int stage)
    {
        return stage switch
        {
            2 => StageTwoBodyTint,
            3 => StageThreeBodyTint,
            _ => StageOneBodyTint
        };
    }

    private static Color GetHornColor(int stage)
    {
        return stage switch
        {
            2 => StageTwoHornTint,
            3 => StageThreeHornTint,
            _ => StageOneHornTint
        };
    }
}
