using Microsoft.Xna.Framework;
using MyGame.Gameplay.Props;

namespace MyGame.Tests.Gameplay.Props;

public sealed class TreePropTests
{
    [Fact]
    public void Bounds_UsesPositionAndSize()
    {
        var tree = new TreeProp(new Vector2(96f, 144f), new Point(64, 96));

        Assert.Equal(new Rectangle(96, 144, 64, 96), tree.Bounds);
        Assert.True(tree.BlocksMovement);
        Assert.Equal(new Rectangle(117, 216, 21, 24), tree.CollisionBounds);
    }
}
