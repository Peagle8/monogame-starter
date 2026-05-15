using Microsoft.Xna.Framework;

namespace MyGame.Rendering.Gameplay;

public static class NpcDialogueLayout
{
    public static Rectangle GetPromptBounds(Point viewportSize)
    {
        var width = Math.Clamp((int)MathF.Round(viewportSize.X * 0.24f), 260, 460);
        return new Rectangle(
            (viewportSize.X - width) / 2,
            viewportSize.Y - 76,
            width,
            38);
    }

    public static Rectangle GetPanelBounds(Point viewportSize)
    {
        var width = Math.Clamp((int)MathF.Round(viewportSize.X * 0.62f), 460, 920);
        var height = Math.Clamp((int)MathF.Round(viewportSize.Y * 0.22f), 150, 230);
        return new Rectangle(
            (viewportSize.X - width) / 2,
            viewportSize.Y - height - 44,
            width,
            height);
    }

    public static Rectangle GetTextBounds(Rectangle panelBounds)
    {
        return new Rectangle(
            panelBounds.X + 28,
            panelBounds.Y + 68,
            panelBounds.Width - 56,
            panelBounds.Height - 104);
    }
}
