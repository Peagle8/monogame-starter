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

        Assert.Equal(new Vector2(16f, 16f), camera.WorldTopLeft);
    }
}
