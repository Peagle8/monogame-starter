using Microsoft.Xna.Framework;

namespace MyGame.Core.Rendering;

public sealed class RenderCamera
{
    public RenderCamera(Vector2 worldTopLeft, Point viewportSize, float zoom = 1f)
    {
        WorldTopLeft = worldTopLeft;
        ViewportSize = viewportSize;
        Zoom = Math.Max(zoom, 0.01f);
    }

    public Vector2 WorldTopLeft { get; }

    public Point ViewportSize { get; }

    public float Zoom { get; }

    public Rectangle WorldViewBounds => new(
        (int)WorldTopLeft.X,
        (int)WorldTopLeft.Y,
        (int)MathF.Ceiling(ViewportSize.X / Zoom),
        (int)MathF.Ceiling(ViewportSize.Y / Zoom));

    public static RenderCamera CreateIdentity(Point viewportSize)
    {
        return new RenderCamera(Vector2.Zero, viewportSize);
    }

    public Vector2 WorldToScreen(Vector2 worldPosition)
    {
        return (worldPosition - WorldTopLeft) * Zoom;
    }

    public Rectangle WorldToScreen(Rectangle worldBounds)
    {
        var screenTopLeft = WorldToScreen(new Vector2(worldBounds.X, worldBounds.Y));
        return new Rectangle(
            (int)MathF.Round(screenTopLeft.X),
            (int)MathF.Round(screenTopLeft.Y),
            Math.Max(1, (int)MathF.Round(worldBounds.Width * Zoom)),
            Math.Max(1, (int)MathF.Round(worldBounds.Height * Zoom)));
    }
}
