using Microsoft.Xna.Framework;
using MyGame.Rendering.Gameplay;

namespace MyGame.Tests.Rendering.Gameplay;

public sealed class GameplayHudLayoutTests
{
    [Fact]
    public void GetHealthPanelBounds_ReturnsTopLeftPanel()
    {
        var bounds = GameplayHudLayout.GetHealthPanelBounds();

        Assert.Equal(new Rectangle(12, 12, 160, 68), bounds);
    }

    [Fact]
    public void GetHealthPipBounds_SpacesPipsEvenly()
    {
        var first = GameplayHudLayout.GetHealthPipBounds(0);
        var third = GameplayHudLayout.GetHealthPipBounds(2);

        Assert.Equal(new Rectangle(24, 54, 16, 16), first);
        Assert.Equal(new Rectangle(68, 54, 16, 16), third);
    }

    [Fact]
    public void GetDeathPanelBounds_CentersPanelInViewport()
    {
        var bounds = GameplayHudLayout.GetDeathPanelBounds(new Point(800, 480));

        Assert.Equal(new Rectangle(190, 174, 420, 132), bounds);
    }

    [Fact]
    public void GetDebugOverlayPosition_PlacesOverlayBelowHealthPanel()
    {
        var position = GameplayHudLayout.GetDebugOverlayPosition();

        Assert.Equal(new Vector2(12f, 92f), position);
    }
}
