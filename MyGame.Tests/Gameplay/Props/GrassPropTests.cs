using Microsoft.Xna.Framework;
using MyGame.Gameplay.Props;

namespace MyGame.Tests.Gameplay.Props;

public sealed class GrassPropTests
{
    [Fact]
    public void Bounds_UsesPositionAndSize()
    {
        var grass = new GrassProp(new Vector2(96f, 144f), new Point(48, 24));

        Assert.Equal(new Rectangle(96, 144, 48, 24), grass.Bounds);
        Assert.False(grass.BlocksMovement);
        Assert.Equal(grass.Bounds, grass.CollisionBounds);
    }
}
