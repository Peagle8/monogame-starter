using Microsoft.Xna.Framework;

namespace MyGame.Core.Rendering;

public static class WorldViewCulling
{
    public static bool IsVisible(Rectangle worldBounds, Rectangle worldViewBounds)
    {
        return worldBounds.Intersects(worldViewBounds);
    }
}
