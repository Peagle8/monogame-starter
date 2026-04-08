using Microsoft.Xna.Framework;
using MyGame.Rendering.Gameplay;

namespace MyGame.Tests.Rendering.Gameplay;

public sealed class GameplayHudLayoutTests
{
    [Fact]
    public void GetHealthPanelBounds_ReturnsTopLeftPanel()
    {
        var bounds = GameplayHudLayout.GetHealthPanelBounds();

        Assert.Equal(new Rectangle(12, 12, 208, 62), bounds);
    }

    [Fact]
    public void GetHealthBarBounds_PlacesBarInsideHealthPanel()
    {
        var bounds = GameplayHudLayout.GetHealthBarBounds();

        Assert.Equal(new Rectangle(56, 24, 148, 10), bounds);
    }

    [Fact]
    public void GetAbilityPointBarBounds_PlacesBarInsideHealthPanel()
    {
        var bounds = GameplayHudLayout.GetAbilityPointBarBounds();

        Assert.Equal(new Rectangle(56, 50, 148, 10), bounds);
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

        Assert.Equal(new Vector2(12f, 86f), position);
    }
}
