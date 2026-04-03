using Microsoft.Xna.Framework;

namespace MyGame.Core.Rendering;

public interface IWorldRectangleRenderer
{
    void Draw(Rectangle worldBounds, Color color);

    bool IsVisible(Rectangle worldBounds);
}
