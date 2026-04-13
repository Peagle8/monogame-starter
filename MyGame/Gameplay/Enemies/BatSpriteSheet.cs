using Microsoft.Xna.Framework;

namespace MyGame.Gameplay.Enemies;

public static class BatSpriteSheet
{
    public static int FrameWidth => 128;

    public static int FrameHeight => 128;

    public static int Frames => 8;

    public static int SheetWidth => FrameWidth * Frames;

    public static int SheetHeight => FrameHeight;

    public static Rectangle GetSourceRectangle(int frameIndex)
    {
        return new Rectangle(frameIndex * FrameWidth, 0, FrameWidth, FrameHeight);
    }
}
