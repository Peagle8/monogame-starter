using Microsoft.Xna.Framework;

namespace MyGame.Rendering.MainMenu;

public static class MainMenuLayout
{
    public static Vector2 GetTitlePosition(Point viewportSize)
    {
        return new Vector2(viewportSize.X * 0.18f, viewportSize.Y * 0.18f);
    }

    public static Vector2 GetItemsStartPosition(Point viewportSize)
    {
        return new Vector2(viewportSize.X * 0.22f, viewportSize.Y * 0.34f);
    }

    public static Vector2 GetFooterPosition(Point viewportSize)
    {
        return new Vector2(viewportSize.X * 0.16f, viewportSize.Y * 0.76f);
    }

    public static float GetFooterWidth(Point viewportSize)
    {
        return Math.Clamp(viewportSize.X * 0.24f, 360f, 520f);
    }
}
