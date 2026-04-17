using Microsoft.Xna.Framework;

namespace MyGame.Rendering.Menus;

public static class ControlsOverlayLayout
{
    public static Rectangle GetPanelBounds(Point viewportSize)
    {
        var width = Math.Clamp((int)MathF.Round(viewportSize.X * 0.42f), 460, 920);
        var height = viewportSize.Y <= 480
            ? 268
            : Math.Clamp((int)MathF.Round(viewportSize.Y * 0.57f), 268, 620);
        return new Rectangle(
            (viewportSize.X - width) / 2,
            (viewportSize.Y - height) / 2,
            width,
            height);
    }

    public static Vector2 GetTitlePosition(Point viewportSize)
    {
        var bounds = GetPanelBounds(viewportSize);
        return new Vector2(bounds.X + (bounds.Width * 0.33f), bounds.Y + 26f);
    }

    public static Vector2 GetLinesStartPosition(Point viewportSize)
    {
        var bounds = GetPanelBounds(viewportSize);
        return new Vector2(bounds.X + 28f, bounds.Y + 76f);
    }

    public static Vector2 GetHintLineOnePosition(Point viewportSize)
    {
        var bounds = GetPanelBounds(viewportSize);
        return new Vector2(bounds.X + 28f, bounds.Bottom - 54f);
    }

    public static Vector2 GetHintLineTwoPosition(Point viewportSize)
    {
        var bounds = GetPanelBounds(viewportSize);
        return new Vector2(bounds.X + 28f, bounds.Bottom - 30f);
    }
}
