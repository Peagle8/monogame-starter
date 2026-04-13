using Microsoft.Xna.Framework;
using MyGame.Core;

namespace MyGame.Gameplay.Enemies;

public static class BatAnimationFrameSelector
{
    private const float HoverFrameDurationSeconds = 0.09f;
    private const float FrameBoundaryEpsilonSeconds = 0.0001f;

    public static int GetFrameIndex(float totalSeconds)
    {
        var animationStep = (int)MathF.Floor((totalSeconds + FrameBoundaryEpsilonSeconds) / HoverFrameDurationSeconds);
        return animationStep % BatSpriteSheet.Frames;
    }

    public static Rectangle GetSourceRectangle(FrameTime frameTime)
    {
        return BatSpriteSheet.GetSourceRectangle(GetFrameIndex(frameTime.TotalSeconds));
    }
}
