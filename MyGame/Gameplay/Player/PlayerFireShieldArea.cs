using Microsoft.Xna.Framework;

namespace MyGame.Gameplay.Player;

public static class PlayerFireShieldArea
{
    public static Vector2 GetCenter(Rectangle playerBounds)
    {
        return new Vector2(playerBounds.Center.X, playerBounds.Center.Y);
    }

    public static float GetRadius(Rectangle playerBounds, float radiusMultiplier)
    {
        return Math.Max(playerBounds.Width, playerBounds.Height) * radiusMultiplier;
    }

    public static Rectangle GetVisualBounds(Rectangle playerBounds, float radiusMultiplier, float extraRadius = 0f)
    {
        var radius = GetRadius(playerBounds, radiusMultiplier) + Math.Max(0f, extraRadius);
        var center = GetCenter(playerBounds);
        var diameter = (int)MathF.Ceiling(radius * 2f);

        return new Rectangle(
            (int)MathF.Round(center.X - radius),
            (int)MathF.Round(center.Y - radius),
            diameter,
            diameter);
    }

    public static bool Intersects(Rectangle playerBounds, Rectangle targetBounds, float radiusMultiplier)
    {
        var center = GetCenter(playerBounds);
        var radius = GetRadius(playerBounds, radiusMultiplier);
        var closestX = Math.Clamp(center.X, targetBounds.Left, targetBounds.Right);
        var closestY = Math.Clamp(center.Y, targetBounds.Top, targetBounds.Bottom);
        var deltaX = center.X - closestX;
        var deltaY = center.Y - closestY;
        return (deltaX * deltaX) + (deltaY * deltaY) <= radius * radius;
    }
}
