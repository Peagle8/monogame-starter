using Microsoft.Xna.Framework;

namespace MyGame.Rendering.Menus;

public static class ControlsOverlayLayout
{
    private const int PanelWidth = 460;
    private const int PanelHeight = 268;
    private const float TitleOffsetY = 24f;
    private const float LinesOffsetX = 28f;
    private const float LinesOffsetY = 76f;
    private const float HintOffsetX = 28f;
    private const float HintLineOneOffsetY = 214f;
    private const float HintLineTwoOffsetY = 234f;

    public static Rectangle GetPanelBounds(Point viewportSize)
    {
        return new Rectangle(
            (viewportSize.X - PanelWidth) / 2,
            (viewportSize.Y - PanelHeight) / 2,
            PanelWidth,
            PanelHeight);
    }

    public static Vector2 GetTitlePosition(Point viewportSize)
    {
        var bounds = GetPanelBounds(viewportSize);
        return new Vector2(bounds.X + 136f, bounds.Y + TitleOffsetY);
    }

    public static Vector2 GetLinesStartPosition(Point viewportSize)
    {
        var bounds = GetPanelBounds(viewportSize);
        return new Vector2(bounds.X + LinesOffsetX, bounds.Y + LinesOffsetY);
    }

    public static Vector2 GetHintLineOnePosition(Point viewportSize)
    {
        var bounds = GetPanelBounds(viewportSize);
        return new Vector2(bounds.X + HintOffsetX, bounds.Y + HintLineOneOffsetY);
    }

    public static Vector2 GetHintLineTwoPosition(Point viewportSize)
    {
        var bounds = GetPanelBounds(viewportSize);
        return new Vector2(bounds.X + HintOffsetX, bounds.Y + HintLineTwoOffsetY);
    }
}
