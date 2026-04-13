using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;

namespace MyGame.Rendering.Enemies;

public sealed class BatEnemyRenderer : IEnemyKindRenderer
{
    private const int DrawSize = 56;
    private const int DrawVerticalOffset = 10;

    private static readonly Color DefeatedTint = new(96, 74, 88);
    private static readonly Color HitFlashTint = new(255, 240, 220);
    private static readonly Color TrailTint = new(40, 54, 96, 110);

    private readonly IRenderContext _renderContext;
    private readonly IWorldRectangleRenderer _worldRectangleRenderer;
    private readonly IWorldSpriteRenderer _worldSpriteRenderer;

    public BatEnemyRenderer(
        IRenderContext renderContext,
        IWorldRectangleRenderer worldRectangleRenderer,
        IWorldSpriteRenderer worldSpriteRenderer)
    {
        _renderContext = renderContext;
        _worldRectangleRenderer = worldRectangleRenderer;
        _worldSpriteRenderer = worldSpriteRenderer;
    }

    public EnemyKind Kind => EnemyKind.Bat;

    public void Draw(EnemyActor enemy, FrameTime frameTime)
    {
        var drawColor = enemy.State == EnemyState.Dead
            ? DefeatedTint * Math.Max(enemy.DefeatedVisibilityAlpha, 0.35f)
            : Color.White;

        if (enemy.IsFlashingFromHit)
        {
            drawColor = Color.Lerp(drawColor, HitFlashTint, enemy.HitFlashAlpha);
        }

        if (enemy.State == EnemyState.Dashing)
        {
            _worldRectangleRenderer.Draw(GetTrail(enemy), TrailTint);
        }

        _worldSpriteRenderer.Draw(
            texture: _renderContext.Assets.BatSprite,
            worldBounds: GetDrawBounds(enemy.Bounds),
            sourceRectangle: BatAnimationFrameSelector.GetSourceRectangle(frameTime),
            color: drawColor);
    }

    private static Rectangle GetTrail(EnemyActor enemy)
    {
        return enemy.DashDirection switch
        {
            Direction.Up => new Rectangle(enemy.Bounds.X + 6, enemy.Bounds.Y + 18, 16, 10),
            Direction.Down => new Rectangle(enemy.Bounds.X + 6, enemy.Bounds.Y - 2, 16, 10),
            Direction.Left => new Rectangle(enemy.Bounds.X + 16, enemy.Bounds.Y + 6, 12, 16),
            Direction.Right => new Rectangle(enemy.Bounds.X, enemy.Bounds.Y + 6, 12, 16),
            _ => enemy.Bounds
        };
    }

    private static Rectangle GetDrawBounds(Rectangle bounds)
    {
        var centerX = bounds.X + (bounds.Width / 2);
        var centerY = bounds.Y + (bounds.Height / 2);
        return new Rectangle(
            centerX - (DrawSize / 2),
            centerY - (DrawSize / 2) - DrawVerticalOffset,
            DrawSize,
            DrawSize);
    }
}
