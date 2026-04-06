using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Enemies;

namespace MyGame.Rendering.Enemies;

public sealed class CrabEnemyRenderer : IEnemyKindRenderer
{
    private static readonly Color DefeatedTint = new(137, 86, 84);
    private static readonly Color HitFlashTint = new(255, 240, 220);

    private readonly IRenderContext _renderContext;
    private readonly IWorldSpriteRenderer _worldSpriteRenderer;

    public CrabEnemyRenderer(IRenderContext renderContext, IWorldSpriteRenderer worldSpriteRenderer)
    {
        _renderContext = renderContext;
        _worldSpriteRenderer = worldSpriteRenderer;
    }

    public EnemyKind Kind => EnemyKind.Crab;

    public void Draw(EnemyActor enemy, FrameTime frameTime)
    {
        var sourceRectangle = CrabAnimationFrameSelector.GetSourceRectangle(enemy.IsMoving, frameTime);
        var drawBounds = new Rectangle(enemy.Bounds.X - 2, enemy.Bounds.Y + 6, 32, 16);
        var drawColor = enemy.State == EnemyState.Dead
            ? DefeatedTint * Math.Max(enemy.DefeatedVisibilityAlpha, 0.35f)
            : Color.White;

        if (enemy.IsFlashingFromHit)
        {
            drawColor = Color.Lerp(drawColor, HitFlashTint, enemy.HitFlashAlpha);
        }

        _worldSpriteRenderer.Draw(
            texture: _renderContext.Assets.CrabSprite,
            worldBounds: drawBounds,
            sourceRectangle: sourceRectangle,
            color: drawColor);
    }
}
