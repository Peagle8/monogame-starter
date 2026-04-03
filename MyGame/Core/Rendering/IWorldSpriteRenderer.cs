using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyGame.Core.Rendering;

public interface IWorldSpriteRenderer
{
    void Draw(Texture2D texture, Rectangle worldBounds, Color color);

    void Draw(Texture2D texture, Rectangle worldBounds, Rectangle? sourceRectangle, Color color);

    bool IsVisible(Rectangle worldBounds);
}
