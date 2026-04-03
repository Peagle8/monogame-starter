using Microsoft.Xna.Framework;
using MyGame.Core.Rendering;

namespace MyGame.Tests.Core.Rendering;

public sealed class WorldViewCullingTests
{
    [Fact]
    public void IsVisible_ReturnsTrueForIntersectingBounds()
    {
        var visible = WorldViewCulling.IsVisible(
            new Rectangle(120, 80, 32, 32),
            new Rectangle(100, 60, 80, 80));

        Assert.True(visible);
    }

    [Fact]
    public void IsVisible_ReturnsFalseForNonIntersectingBounds()
    {
        var visible = WorldViewCulling.IsVisible(
            new Rectangle(300, 220, 32, 32),
            new Rectangle(100, 60, 80, 80));

        Assert.False(visible);
    }
}
