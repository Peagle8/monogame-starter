using MyGame.Rendering.MainMenu;

namespace MyGame.Tests.Rendering.MainMenu;

public sealed class MainMenuBackgroundPaletteTests
{
    [Fact]
    public void GetStripeColor_CyclesThroughPalette()
    {
        var first = MainMenuBackgroundPalette.GetStripeColor(0);
        var second = MainMenuBackgroundPalette.GetStripeColor(1);
        var wrapped = MainMenuBackgroundPalette.GetStripeColor(3);

        Assert.NotEqual(first, second);
        Assert.Equal(first, wrapped);
    }
}
