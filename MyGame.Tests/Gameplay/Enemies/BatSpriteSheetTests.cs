using Microsoft.Xna.Framework;
using MyGame.Gameplay.Enemies;

namespace MyGame.Tests.Gameplay.Enemies;

public sealed class BatSpriteSheetTests
{
    [Fact]
    public void Sheet_UsesExpectedDimensions()
    {
        Assert.Equal(128, BatSpriteSheet.FrameWidth);
        Assert.Equal(128, BatSpriteSheet.FrameHeight);
        Assert.Equal(8, BatSpriteSheet.Frames);
        Assert.Equal(BatSpriteSheet.FrameWidth * BatSpriteSheet.Frames, BatSpriteSheet.SheetWidth);
        Assert.Equal(BatSpriteSheet.FrameHeight, BatSpriteSheet.SheetHeight);
    }

    [Fact]
    public void GetSourceRectangle_ReturnsExpectedFrameRegion()
    {
        var rectangle = BatSpriteSheet.GetSourceRectangle(3);

        Assert.Equal(new Rectangle(384, 0, 128, 128), rectangle);
    }
}
