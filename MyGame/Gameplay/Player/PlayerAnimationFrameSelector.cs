using Microsoft.Xna.Framework;
using MyGame.Core;

namespace MyGame.Gameplay.Player;

public static class PlayerAnimationFrameSelector
{
    private static readonly int[] WalkingFrameSequence = [0, 1, 2, 1];

    private const float WalkFrameDurationSeconds = 0.16f;

    public static Rectangle GetSourceRectangle(Direction facing, bool isMoving, FrameTime frameTime)
    {
        var frameIndex = GetFrameIndex(isMoving, frameTime.TotalSeconds);
        return PlayerSpriteSheet.GetSourceRectangle(facing, frameIndex);
    }

    public static int GetFrameIndex(bool isMoving, float totalSeconds)
    {
        if (!isMoving)
        {
            return 1;
        }

        var animationStep = (int)(totalSeconds / WalkFrameDurationSeconds);
        return WalkingFrameSequence[animationStep % WalkingFrameSequence.Length];
    }
}
