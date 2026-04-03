using Microsoft.Xna.Framework;
using MyGame.Core;

namespace MyGame.Gameplay.Enemies;

public static class CrabAnimationFrameSelector
{
    public static int GetFrameIndex(bool isMoving, float totalSeconds)
    {
        if (!isMoving)
        {
            return 1;
        }

        var frame = (int)(totalSeconds / 0.18f) % 4;
        return frame switch
        {
            0 => 0,
            1 => 1,
            2 => 2,
            _ => 1
        };
    }

    public static Rectangle GetSourceRectangle(bool isMoving, FrameTime frameTime)
    {
        return CrabSpriteSheet.GetSourceRectangle(GetFrameIndex(isMoving, frameTime.TotalSeconds));
    }
}
