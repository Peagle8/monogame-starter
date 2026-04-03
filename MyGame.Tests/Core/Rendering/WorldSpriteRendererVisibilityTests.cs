using Microsoft.Xna.Framework;
using MyGame.Core.Rendering;

namespace MyGame.Tests.Core.Rendering;

public sealed class WorldSpriteRendererVisibilityTests
{
    [Fact]
    public void IsVisible_ReturnsTrueForIntersectingBounds()
    {
        var camera = new RenderCamera(new Vector2(100f, 50f), new Point(320, 180));

        var visible = WorldViewCulling.IsVisible(new Rectangle(120, 70, 32, 32), camera.WorldViewBounds);

        Assert.True(visible);
    }

    [Fact]
    public void IsVisible_ReturnsFalseForOffscreenBounds()
    {
        var camera = new RenderCamera(new Vector2(100f, 50f), new Point(320, 180));

        var visible = WorldViewCulling.IsVisible(new Rectangle(500, 400, 32, 32), camera.WorldViewBounds);

        Assert.False(visible);
    }
}
