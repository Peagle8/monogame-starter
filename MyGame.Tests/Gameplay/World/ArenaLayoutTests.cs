using Microsoft.Xna.Framework;
using MyGame.Gameplay.Props;
using MyGame.Gameplay.World;

namespace MyGame.Tests.Gameplay.World;

public sealed class ArenaLayoutTests
{
    [Fact]
    public void CollisionBounds_IncludeFullTopWallBarrier()
    {
        Assert.Contains(ArenaLayout.CollisionBounds, bounds => bounds == new Rectangle(0, 0, 960, 120));
    }

    [Fact]
    public void CollisionBounds_BlockSideWallsButLeaveArenaDoorwayOpen()
    {
        Assert.Contains(ArenaLayout.CollisionBounds, bounds => bounds.Contains(24, 300));
        Assert.DoesNotContain(ArenaLayout.CollisionBounds, bounds => bounds.Contains(120, 300));
        Assert.DoesNotContain(ArenaLayout.CollisionBounds, bounds => bounds.Contains(468, 564));
        Assert.Contains(ArenaLayout.CollisionBounds, bounds => bounds.Contains(120, 564));
    }

    [Fact]
    public void CreateBoundaryProps_DefaultsToHiddenArenaCollisionProps()
    {
        var props = ArenaLayout.CreateBoundaryProps();

        Assert.NotEmpty(props);
        Assert.All(props, boundary => Assert.False(boundary.IsVisible));
        Assert.All(props, boundary => Assert.Equal(boundary.Bounds, boundary.CollisionBounds));
    }
}
