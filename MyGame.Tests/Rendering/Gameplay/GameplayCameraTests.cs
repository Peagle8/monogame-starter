using Microsoft.Xna.Framework;
using MyGame.Rendering.Gameplay;

namespace MyGame.Tests.Rendering.Gameplay;

public sealed class GameplayCameraTests
{
    [Fact]
    public void Create_CentersViewOnPlayer()
    {
        var camera = GameplayCamera.Create(
            playerPosition: new Vector2(400f, 240f),
            viewportSize: new Point(800, 480),
            playerSize: new Point(32, 32));

        Assert.Equal(1.15f, camera.Zoom);
        Assert.Equal(68.17392f, camera.WorldTopLeft.X, 3);
        Assert.Equal(47.30435f, camera.WorldTopLeft.Y, 3);
    }
}
