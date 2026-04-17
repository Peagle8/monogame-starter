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

    [Fact]
    public void Create_WhenWorldBoundsAreProvided_ClampsViewInsideRoom()
    {
        var camera = GameplayCamera.Create(
            playerPosition: new Vector2(16f, 16f),
            viewportSize: new Point(800, 480),
            playerSize: new Point(32, 32),
            worldBounds: new Rectangle(0, 0, 800, 480));

        Assert.Equal(0f, camera.WorldTopLeft.X);
        Assert.Equal(0f, camera.WorldTopLeft.Y);
    }

    [Fact]
    public void Create_WhenViewportIsLargerThanWorld_ZoomsToFitAndCentersWorld()
    {
        var camera = GameplayCamera.Create(
            playerPosition: new Vector2(384f, 224f),
            viewportSize: new Point(1920, 1080),
            playerSize: new Point(32, 32),
            worldBounds: new Rectangle(0, 0, 800, 480));

        Assert.Equal(2.25f, camera.Zoom, 3);
        Assert.Equal(-26.6667f, camera.WorldTopLeft.X, 3);
        Assert.Equal(0f, camera.WorldTopLeft.Y, 3);
    }
}
