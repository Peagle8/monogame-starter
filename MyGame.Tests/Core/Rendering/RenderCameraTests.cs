using Microsoft.Xna.Framework;
using MyGame.Core.Rendering;

namespace MyGame.Tests.Core.Rendering;

public sealed class RenderCameraTests
{
    [Fact]
    public void WorldToScreen_ShiftsRectangleByCameraOrigin()
    {
        var camera = new RenderCamera(new Vector2(100f, 80f), new Point(800, 480));

        var result = camera.WorldToScreen(new Rectangle(120, 95, 32, 32));

        Assert.Equal(new Rectangle(20, 15, 32, 32), result);
    }

    [Fact]
    public void WorldToScreen_WithZoom_ScalesPositionAndSize()
    {
        var camera = new RenderCamera(new Vector2(100f, 80f), new Point(800, 480), zoom: 1.15f);

        var result = camera.WorldToScreen(new Rectangle(120, 95, 32, 32));

        Assert.Equal(new Rectangle(23, 17, 37, 37), result);
    }

    [Fact]
    public void CreateIdentity_UsesZeroOrigin()
    {
        var camera = RenderCamera.CreateIdentity(new Point(800, 480));

        Assert.Equal(Vector2.Zero, camera.WorldTopLeft);
        Assert.Equal(new Rectangle(0, 0, 800, 480), camera.WorldViewBounds);
        Assert.Equal(1f, camera.Zoom);
    }
}
