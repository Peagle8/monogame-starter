using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Gameplay.Player;

namespace MyGame.Tests.Gameplay.Player;

public sealed class PlayerAnimationFrameSelectorTests
{
    [Fact]
    public void GetFrameIndex_WhenIdle_ReturnsCenterFrame()
    {
        var frameIndex = PlayerAnimationFrameSelector.GetFrameIndex(false, totalSeconds: 8.5f);

        Assert.Equal(1, frameIndex);
    }

    [Theory]
    [InlineData(0.00f, 0)]
    [InlineData(0.16f, 1)]
    [InlineData(0.32f, 2)]
    [InlineData(0.48f, 1)]
    [InlineData(0.64f, 0)]
    public void GetFrameIndex_WhenWalking_CyclesThroughWalkFrames(float totalSeconds, int expectedFrame)
    {
        var frameIndex = PlayerAnimationFrameSelector.GetFrameIndex(true, totalSeconds);

        Assert.Equal(expectedFrame, frameIndex);
    }

    [Fact]
    public void GetSourceRectangle_UsesFacingRowAndSelectedFrame()
    {
        var frameTime = new FrameTime(TimeSpan.FromSeconds(0.16), TimeSpan.FromSeconds(0.32));

        var rectangle = PlayerAnimationFrameSelector.GetSourceRectangle(Direction.Left, isMoving: true, frameTime);

        Assert.Equal(new Rectangle(64, 32, 32, 16), rectangle);
    }
}
