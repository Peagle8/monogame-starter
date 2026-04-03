using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Player;

namespace MyGame.Rendering.Player;

public sealed class PlayerRenderer : IRenderer<PlayerActor>
{
    private readonly IRenderContext _renderContext;
    private readonly IWorldSpriteRenderer _worldSpriteRenderer;

    public PlayerRenderer(IRenderContext renderContext, IWorldSpriteRenderer worldSpriteRenderer)
    {
        _renderContext = renderContext;
        _worldSpriteRenderer = worldSpriteRenderer;
    }

    public void Draw(PlayerActor model, FrameTime frameTime)
    {
        var sourceRectangle = PlayerAnimationFrameSelector.GetSourceRectangle(model.Facing, model.IsMoving, frameTime);
        _worldSpriteRenderer.Draw(
            texture: _renderContext.Assets.PlayerSprite,
            worldBounds: model.Bounds,
            sourceRectangle: sourceRectangle,
            color: Color.White);
    }
}
