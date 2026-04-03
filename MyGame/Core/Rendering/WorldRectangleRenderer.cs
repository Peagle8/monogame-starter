using Microsoft.Xna.Framework;

namespace MyGame.Core.Rendering;

public sealed class WorldRectangleRenderer : IWorldRectangleRenderer
{
    private readonly IRenderContext _renderContext;

    public WorldRectangleRenderer(IRenderContext renderContext)
    {
        _renderContext = renderContext;
    }

    public void Draw(Rectangle worldBounds, Color color)
    {
        if (!IsVisible(worldBounds))
        {
            return;
        }

        _renderContext.SpriteBatch.Draw(
            _renderContext.Assets.Pixel,
            _renderContext.Camera.WorldToScreen(worldBounds),
            color);
    }

    public bool IsVisible(Rectangle worldBounds)
    {
        return WorldViewCulling.IsVisible(worldBounds, _renderContext.Camera.WorldViewBounds);
    }
}
