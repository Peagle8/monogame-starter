using Microsoft.Xna.Framework;
using MyGame.Rendering.MainMenu;

namespace MyGame.Tests.Rendering.MainMenu;

public sealed class MainMenuLayoutTests
{
    [Fact]
    public void GetTitlePosition_UsesBaselineViewportPlacement()
    {
        var position = MainMenuLayout.GetTitlePosition(new Point(800, 480));

        Assert.Equal(new Vector2(144f, 86.4f), position);
    }

    [Fact]
    public void GetFooterWidth_GrowsForFullscreenViewport()
    {
        var width = MainMenuLayout.GetFooterWidth(new Point(1920, 1080));

        Assert.Equal(460.8f, width);
    }
}
