using Microsoft.Xna.Framework;

namespace MyGame.Core.Rendering;

public sealed class RenderCamera
{
    public RenderCamera(Vector2 worldTopLeft, Point viewportSize)
    {
        WorldTopLeft = worldTopLeft;
        ViewportSize = viewportSize;
    }

    public Vector2 WorldTopLeft { get; }

    public Point ViewportSize { get; }

    public Rectangle WorldViewBounds => new((int)WorldTopLeft.X, (int)WorldTopLeft.Y, ViewportSize.X, ViewportSize.Y);

    public static RenderCamera CreateIdentity(Point viewportSize)
    {
        return new RenderCamera(Vector2.Zero, viewportSize);
    }

    public Vector2 WorldToScreen(Vector2 worldPosition)
    {
        return worldPosition - WorldTopLeft;
    }

    public Rectangle WorldToScreen(Rectangle worldBounds)
    {
        return new Rectangle(
            worldBounds.X - (int)WorldTopLeft.X,
            worldBounds.Y - (int)WorldTopLeft.Y,
            worldBounds.Width,
            worldBounds.Height);
    }
}
