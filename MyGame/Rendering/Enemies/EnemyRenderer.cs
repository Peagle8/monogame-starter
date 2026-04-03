using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Enemies;

namespace MyGame.Rendering.Enemies;

public sealed class EnemyRenderer : IRenderer<EnemyActor>
{
    private static readonly Color DefeatedTint = new(137, 86, 84);
    private static readonly Color HitFlashTint = new(255, 240, 220);

    private readonly IRenderContext _renderContext;
    private readonly IWorldSpriteRenderer _worldSpriteRenderer;

    public EnemyRenderer(IRenderContext renderContext, IWorldSpriteRenderer worldSpriteRenderer)
    {
        _renderContext = renderContext;
        _worldSpriteRenderer = worldSpriteRenderer;
    }

    public void Draw(EnemyActor model, FrameTime frameTime)
    {
        var sourceRectangle = CrabAnimationFrameSelector.GetSourceRectangle(model.IsMoving, frameTime);
        var drawBounds = new Rectangle(model.Bounds.X - 2, model.Bounds.Y + 6, 32, 16);
        var drawColor = model.State == EnemyState.Dead
            ? DefeatedTint * Math.Max(model.DefeatedVisibilityAlpha, 0.35f)
            : Color.White;

        if (model.IsFlashingFromHit)
        {
            drawColor = Color.Lerp(drawColor, HitFlashTint, model.HitFlashAlpha);
        }

        _worldSpriteRenderer.Draw(
            texture: _renderContext.Assets.CrabSprite,
            worldBounds: drawBounds,
            sourceRectangle: sourceRectangle,
            color: drawColor);
    }
}
