using Microsoft.Xna.Framework;
using MyGame.Gameplay.Player;

namespace MyGame.Tests.Gameplay.Player;

public sealed class PlayerSpriteSheetTests
{
    [Fact]
    public void Rows_UseExpectedSheetDimensions()
    {
        Assert.Equal(32, PlayerSpriteSheet.FrameWidth);
        Assert.Equal(16, PlayerSpriteSheet.FrameHeight);
        Assert.Equal(3, PlayerSpriteSheet.FramesPerDirection);
        Assert.Equal(PlayerSpriteSheet.FrameWidth * PlayerSpriteSheet.FramesPerDirection, PlayerSpriteSheet.SheetWidth);
        Assert.Equal(PlayerSpriteSheet.FrameHeight * 4, PlayerSpriteSheet.SheetHeight);
        Assert.All(PlayerSpriteSheet.Rows, row => Assert.Equal(PlayerSpriteSheet.SheetWidth, row.Length));
    }

    [Fact]
    public void GetSourceRectangle_ReturnsExpectedFrameRegion()
    {
        var rectangle = PlayerSpriteSheet.GetSourceRectangle(Direction.Right, 1);

        Assert.Equal(new Rectangle(32, 48, 32, 16), rectangle);
    }
}
