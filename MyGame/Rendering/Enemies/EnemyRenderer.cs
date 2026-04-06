using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;

namespace MyGame.Rendering.Enemies;

public sealed class EnemyRenderer : IRenderer<EnemyActor>
{
    private static readonly Color DefeatedTint = new(137, 86, 84);
    private static readonly Color HitFlashTint = new(255, 240, 220);
    private static readonly Color RabbitBodyTint = new(201, 190, 186);
    private static readonly Color RabbitHornTint = new(164, 235, 255);
    private static readonly Color RabbitTrailTint = new(220, 235, 247, 120);

    private readonly IRenderContext _renderContext;
    private readonly IWorldRectangleRenderer _worldRectangleRenderer;
    private readonly IWorldSpriteRenderer _worldSpriteRenderer;

    public EnemyRenderer(
        IRenderContext renderContext,
        IWorldRectangleRenderer worldRectangleRenderer,
        IWorldSpriteRenderer worldSpriteRenderer)
    {
        _renderContext = renderContext;
        _worldRectangleRenderer = worldRectangleRenderer;
        _worldSpriteRenderer = worldSpriteRenderer;
    }

    // TODO: this whole class needs to be re-thought. We can't keep adding methods per enemy here... imagine if we had four dozen different types of enemies?
    public void Draw(EnemyActor model, FrameTime frameTime)
    {
        if (model.Kind == EnemyKind.HornedRabbit)
        {
            DrawHornedRabbit(model);
            return;
        }

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

    private void DrawHornedRabbit(EnemyActor model)
    {
        var drawColor = model.State == EnemyState.Dead
            ? DefeatedTint * Math.Max(model.DefeatedVisibilityAlpha, 0.35f)
            : RabbitBodyTint;

        if (model.IsFlashingFromHit)
        {
            drawColor = Color.Lerp(drawColor, HitFlashTint, model.HitFlashAlpha);
        }

        var hornColor = model.IsFlashingFromHit
            ? Color.Lerp(RabbitHornTint, HitFlashTint, model.HitFlashAlpha)
            : RabbitHornTint;

        var body = new Rectangle(model.Bounds.X + 4, model.Bounds.Y + 10, 20, 14);
        var head = new Rectangle(model.Bounds.X + 16, model.Bounds.Y + 6, 10, 10);
        var backLeg = new Rectangle(model.Bounds.X + 2, model.Bounds.Y + 18, 7, 8);
        var frontLeg = new Rectangle(model.Bounds.X + 18, model.Bounds.Y + 20, 7, 6);
        var leftHorn = new Rectangle(model.Bounds.X + 17, model.Bounds.Y + 1, 3, 7);
        var rightHorn = new Rectangle(model.Bounds.X + 22, model.Bounds.Y + 0, 3, 8);

        if (model.State == EnemyState.Dashing)
        {
            _worldRectangleRenderer.Draw(GetDashTrail(model), RabbitTrailTint);
        }

        _worldRectangleRenderer.Draw(body, drawColor);
        _worldRectangleRenderer.Draw(head, drawColor);
        _worldRectangleRenderer.Draw(backLeg, drawColor * 0.88f);
        _worldRectangleRenderer.Draw(frontLeg, drawColor * 0.92f);
        _worldRectangleRenderer.Draw(leftHorn, hornColor);
        _worldRectangleRenderer.Draw(rightHorn, hornColor);
    }

    private static Rectangle GetDashTrail(EnemyActor model)
    {
        return model.DashDirection switch
        {
            Direction.Up => new Rectangle(model.Bounds.X + 8, model.Bounds.Y + 20, 12, 14),
            Direction.Down => new Rectangle(model.Bounds.X + 8, model.Bounds.Y - 8, 12, 14),
            Direction.Left => new Rectangle(model.Bounds.X + 18, model.Bounds.Y + 10, 14, 10),
            Direction.Right => new Rectangle(model.Bounds.X - 8, model.Bounds.Y + 10, 14, 10),
            _ => model.Bounds
        };
    }
}
