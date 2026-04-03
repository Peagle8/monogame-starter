using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Gameplay.Enemies;

namespace MyGame.Tests.Gameplay.Enemies;

public sealed class CrabAnimationFrameSelectorTests
{
    [Fact]
    public void GetFrameIndex_WhenIdle_ReturnsCenterFrame()
    {
        var frameIndex = CrabAnimationFrameSelector.GetFrameIndex(false, 5f);

        Assert.Equal(1, frameIndex);
    }

    [Theory]
    [InlineData(0.00f, 0)]
    [InlineData(0.18f, 1)]
    [InlineData(0.36f, 2)]
    [InlineData(0.54f, 1)]
    public void GetFrameIndex_WhenMoving_CyclesClawFrames(float totalSeconds, int expectedFrame)
    {
        var frameIndex = CrabAnimationFrameSelector.GetFrameIndex(true, totalSeconds);

        Assert.Equal(expectedFrame, frameIndex);
    }

    [Fact]
    public void GetSourceRectangle_UsesSelectedFrame()
    {
        var frameTime = new FrameTime(TimeSpan.FromSeconds(0.18), TimeSpan.FromSeconds(0.36));

        var rectangle = CrabAnimationFrameSelector.GetSourceRectangle(true, frameTime);

        Assert.Equal(new Rectangle(64, 0, 32, 16), rectangle);
    }
}
