using Microsoft.Xna.Framework;
using MyGame.Rendering.Gameplay;

namespace MyGame.Tests.Rendering.Gameplay;

public sealed class GameplayPauseMenuLayoutTests
{
    [Fact]
    public void GetMenuPanelBounds_UsesBaselineSizeAtDefaultViewport()
    {
        var bounds = GameplayPauseMenuLayout.GetMenuPanelBounds(new Point(800, 480));

        Assert.Equal(new Rectangle(220, 68, 360, 344), bounds);
    }

    [Fact]
    public void GetMapModalBounds_GrowsForFullscreenViewport()
    {
        var bounds = GameplayPauseMenuLayout.GetMapModalBounds(new Point(1920, 1080));

        Assert.Equal(new Rectangle(360, 140, 1200, 799), bounds);
    }

    [Fact]
    public void GetInventoryModalBounds_UsesLargerBaselineForInventoryContent()
    {
        var bounds = GameplayPauseMenuLayout.GetInventoryModalBounds(new Point(800, 480));

        Assert.Equal(new Rectangle(112, 20, 576, 440), bounds);
    }

    [Fact]
    public void GetInventoryContentBounds_StaysInsideModal()
    {
        var modalBounds = GameplayPauseMenuLayout.GetInventoryModalBounds(new Point(1920, 1080));
        var contentBounds = GameplayPauseMenuLayout.GetInventoryContentBounds(modalBounds);

        Assert.True(contentBounds.Left > modalBounds.Left);
        Assert.True(contentBounds.Right < modalBounds.Right);
        Assert.True(contentBounds.Top > modalBounds.Top);
        Assert.True(contentBounds.Bottom < modalBounds.Bottom);
        Assert.Equal(584, contentBounds.Height);
    }
}
