using Microsoft.Xna.Framework;
using MyGame.Rendering.Gameplay;

namespace MyGame.Tests.Rendering.Gameplay;

public sealed class ShopDialogueLayoutTests
{
    [Fact]
    public void GetModalBounds_UsesBaselineViewportSize()
    {
        var bounds = ShopDialogueLayout.GetModalBounds(new Point(800, 480));

        Assert.Equal(new Rectangle(168, 72, 464, 336), bounds);
    }

    [Fact]
    public void GetPromptBounds_GrowsForFullscreenViewport()
    {
        var bounds = ShopDialogueLayout.GetPromptBounds(new Point(1920, 1080));

        Assert.Equal(new Rectangle(710, 1004, 499, 38), bounds);
    }
}
