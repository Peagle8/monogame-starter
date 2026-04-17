using Microsoft.Xna.Framework;
using MyGame.Core.Rendering;

namespace MyGame.Rendering.Gameplay;

public static class GameplayCamera
{
    private const float DefaultZoom = 1.15f;

    public static RenderCamera Create(
        Vector2 playerPosition,
        Point viewportSize,
        Point playerSize,
        Rectangle? worldBounds = null)
    {
        var zoom = ResolveZoom(viewportSize, worldBounds);
        var focusPosition = playerPosition + new Vector2(playerSize.X / 2f, playerSize.Y / 2f);
        var worldViewWidth = viewportSize.X / zoom;
        var worldViewHeight = viewportSize.Y / zoom;
        var unclampedWorldTopLeft = new Vector2(
            focusPosition.X - (worldViewWidth / 2f),
            focusPosition.Y - (worldViewHeight / 2f));
        var worldTopLeft = ClampToWorldBounds(unclampedWorldTopLeft, worldViewWidth, worldViewHeight, worldBounds);

        return new RenderCamera(worldTopLeft, viewportSize, zoom);
    }

    private static float ResolveZoom(Point viewportSize, Rectangle? worldBounds)
    {
        if (worldBounds is not Rectangle bounds)
        {
            return DefaultZoom;
        }

        var fitZoom = MathF.Min(
            viewportSize.X / (float)bounds.Width,
            viewportSize.Y / (float)bounds.Height);

        return MathF.Max(DefaultZoom, fitZoom);
    }

    private static Vector2 ClampToWorldBounds(
        Vector2 worldTopLeft,
        float worldViewWidth,
        float worldViewHeight,
        Rectangle? worldBounds)
    {
        if (worldBounds is not Rectangle bounds)
        {
            return worldTopLeft;
        }

        var maxX = bounds.Right - worldViewWidth;
        var maxY = bounds.Bottom - worldViewHeight;
        return new Vector2(
            ClampAxis(worldTopLeft.X, bounds.Left, maxX),
            ClampAxis(worldTopLeft.Y, bounds.Top, maxY));
    }

    private static float ClampAxis(float value, float min, float max)
    {
        if (max <= min)
        {
            return min + ((max - min) * 0.5f);
        }

        return MathHelper.Clamp(value, min, max);
    }
}
