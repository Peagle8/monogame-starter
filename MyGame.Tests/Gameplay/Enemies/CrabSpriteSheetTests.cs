using Microsoft.Xna.Framework;
using MyGame.Gameplay.Enemies;

namespace MyGame.Tests.Gameplay.Enemies;

public sealed class CrabSpriteSheetTests
{
    [Fact]
    public void Rows_UseExpectedSheetDimensions()
    {
        Assert.Equal(32, CrabSpriteSheet.FrameWidth);
        Assert.Equal(16, CrabSpriteSheet.FrameHeight);
        Assert.Equal(3, CrabSpriteSheet.Frames);
        Assert.Equal(CrabSpriteSheet.FrameWidth * CrabSpriteSheet.Frames, CrabSpriteSheet.SheetWidth);
        Assert.Equal(CrabSpriteSheet.FrameHeight, CrabSpriteSheet.SheetHeight);
        Assert.All(CrabSpriteSheet.Rows, row => Assert.Equal(CrabSpriteSheet.SheetWidth, row.Length));
    }

    [Fact]
    public void GetSourceRectangle_ReturnsExpectedFrameRegion()
    {
        var rectangle = CrabSpriteSheet.GetSourceRectangle(2);

        Assert.Equal(new Rectangle(64, 0, 32, 16), rectangle);
    }
}
