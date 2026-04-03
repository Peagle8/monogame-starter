using Microsoft.Xna.Framework;

namespace MyGame.Rendering.Gameplay;

public static class TreeRenderLayout
{
    public static Rectangle GetCanopyBounds(Rectangle treeBounds)
    {
        var canopyWidth = (int)(treeBounds.Width * 0.8f);
        var canopyHeight = (int)(treeBounds.Height * 0.65f);
        var canopyX = treeBounds.X + ((treeBounds.Width - canopyWidth) / 2);

        return new Rectangle(canopyX, treeBounds.Y, canopyWidth, canopyHeight);
    }

    public static Rectangle GetTrunkBounds(Rectangle treeBounds)
    {
        var trunkWidth = Math.Max(8, treeBounds.Width / 4);
        var trunkHeight = (int)(treeBounds.Height * 0.35f);
        var trunkX = treeBounds.X + ((treeBounds.Width - trunkWidth) / 2);
        var trunkY = treeBounds.Bottom - trunkHeight;

        return new Rectangle(trunkX, trunkY, trunkWidth, trunkHeight);
    }
}
