using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Gameplay.Enemies;

namespace MyGame.Tests.Gameplay.Enemies;

public sealed class BatAnimationFrameSelectorTests
{
    [Theory]
    [InlineData(0.00f, 0)]
    [InlineData(0.09f, 1)]
    [InlineData(0.18f, 2)]
    [InlineData(0.63f, 7)]
    [InlineData(0.72f, 0)]
    public void GetFrameIndex_CyclesThroughAllFrames(float totalSeconds, int expectedFrame)
    {
        var frameIndex = BatAnimationFrameSelector.GetFrameIndex(totalSeconds);

        Assert.Equal(expectedFrame, frameIndex);
    }

    [Fact]
    public void GetSourceRectangle_UsesSelectedFrame()
    {
        var frameTime = new FrameTime(TimeSpan.FromSeconds(0.18), TimeSpan.FromSeconds(0.18));

        var rectangle = BatAnimationFrameSelector.GetSourceRectangle(frameTime);

        Assert.Equal(new Rectangle(256, 0, 128, 128), rectangle);
    }
}
