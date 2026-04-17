using Microsoft.Xna.Framework;

namespace MyGame.Rendering.Gameplay;

public static class ShopDialogueLayout
{
    public static Rectangle GetModalBounds(Point viewportSize)
    {
        return CreateCenteredBounds(viewportSize, 0.58f, 0.66f, 464, 336, 1020, 760);
    }

    public static Rectangle GetTabBounds(Rectangle modalBounds)
    {
        return new Rectangle(
            modalBounds.X + 32,
            modalBounds.Y + 104,
            modalBounds.Width - 64,
            40);
    }

    public static Rectangle GetContentBounds(Rectangle modalBounds)
    {
        return new Rectangle(
            modalBounds.X + 32,
            modalBounds.Y + 164,
            modalBounds.Width - 64,
            modalBounds.Height - 258);
    }

    public static Rectangle GetPromptBounds(Point viewportSize)
    {
        var width = Math.Clamp((int)MathF.Round(viewportSize.X * 0.26f), 300, 520);
        return new Rectangle(
            (viewportSize.X - width) / 2,
            viewportSize.Y - 76,
            width,
            38);
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
        var width = Math.Clamp((int)MathF.Round(viewportSize.X * widthRatio), minWidth, maxWidth);
        var height = Math.Clamp((int)MathF.Round(viewportSize.Y * heightRatio), minHeight, maxHeight);
        return new Rectangle(
            (viewportSize.X - width) / 2,
            (viewportSize.Y - height) / 2,
            width,
            height);
    }
}
