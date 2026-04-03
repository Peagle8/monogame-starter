using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyGame.Core.Rendering;

public sealed class WorldSpriteRenderer : IWorldSpriteRenderer
{
    private readonly IRenderContext _renderContext;

    public WorldSpriteRenderer(IRenderContext renderContext)
    {
        _renderContext = renderContext;
    }

    public void Draw(Texture2D texture, Rectangle worldBounds, Color color)
    {
        Draw(texture, worldBounds, sourceRectangle: null, color);
    }

    public void Draw(Texture2D texture, Rectangle worldBounds, Rectangle? sourceRectangle, Color color)
    {
        if (!IsVisible(worldBounds))
        {
            return;
        }

        _renderContext.SpriteBatch.Draw(
            texture,
            _renderContext.Camera.WorldToScreen(worldBounds),
            sourceRectangle,
            color);
    }

    public bool IsVisible(Rectangle worldBounds)
    {
        return WorldViewCulling.IsVisible(worldBounds, _renderContext.Camera.WorldViewBounds);
    }
}
