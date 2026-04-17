using Microsoft.Xna.Framework;

namespace MyGame.Rendering.Gameplay;

public static class GameplayPauseMenuLayout
{
    public static Rectangle GetMenuPanelBounds(Point viewportSize)
    {
        return CreateCenteredBounds(viewportSize, 0.34f, 0.56f, 360, 344, 560, 520);
    }

    public static Vector2 GetMenuTitlePosition(Rectangle panelBounds)
    {
        return new Vector2(panelBounds.X + (panelBounds.Width * 0.28f), panelBounds.Y + 36f);
    }

    public static Vector2 GetMenuItemsStartPosition(Rectangle panelBounds)
    {
        return new Vector2(panelBounds.X + 72f, panelBounds.Y + 110f);
    }

    public static Vector2 GetFooterPosition(Rectangle panelBounds)
    {
        return new Vector2(panelBounds.X + 32f, panelBounds.Bottom - 64f);
    }

    public static float GetFooterWidth(Rectangle panelBounds)
    {
        return panelBounds.Width - 64f;
    }

    public static Rectangle GetMapModalBounds(Point viewportSize)
    {
        return CreateCenteredBounds(viewportSize, 0.68f, 0.74f, 564, 384, 1200, 860);
    }

    public static Rectangle GetMapContentBounds(Rectangle modalBounds)
    {
        return new Rectangle(
            modalBounds.X + 32,
            modalBounds.Y + 104,
            modalBounds.Width - 64,
            modalBounds.Height - 176);
    }

    public static Rectangle GetInventoryModalBounds(Point viewportSize)
    {
        return CreateCenteredBounds(viewportSize, 0.6f, 0.68f, 464, 336, 1040, 760);
    }

    public static Rectangle GetInventoryTabBounds(Rectangle modalBounds)
    {
        return new Rectangle(
            modalBounds.X + 32,
            modalBounds.Y + 104,
            modalBounds.Width - 64,
            44);
    }

    public static Rectangle GetInventoryContentBounds(Rectangle modalBounds)
    {
        return new Rectangle(
            modalBounds.X + 32,
            modalBounds.Y + 164,
            modalBounds.Width - 64,
            modalBounds.Height - 258);
    }

    private static Rectangle CreateCenteredBounds(
        Point viewportSize,
        float widthRatio,
        float heightRatio,
        int minWidth,
        int minHeight,
        int maxWidth,
        int maxHeight)
    {
        var width = ClampDimension((int)MathF.Round(viewportSize.X * widthRatio), minWidth, maxWidth);
        var height = ClampDimension((int)MathF.Round(viewportSize.Y * heightRatio), minHeight, maxHeight);

        return new Rectangle(
            (viewportSize.X - width) / 2,
            (viewportSize.Y - height) / 2,
            width,
            height);
    }

    private static int ClampDimension(int value, int min, int max)
    {
        return Math.Clamp(value, min, max);
    }
}
