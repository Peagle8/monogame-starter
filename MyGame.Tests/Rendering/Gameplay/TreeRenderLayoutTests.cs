using Microsoft.Xna.Framework;
using MyGame.Rendering.Gameplay;

namespace MyGame.Tests.Rendering.Gameplay;

public sealed class TreeRenderLayoutTests
{
    [Fact]
    public void GetCanopyBounds_FitsInsideTreeBounds()
    {
        var treeBounds = new Rectangle(100, 120, 64, 96);

        var canopyBounds = TreeRenderLayout.GetCanopyBounds(treeBounds);

        Assert.True(treeBounds.Contains(canopyBounds));
        Assert.Equal(treeBounds.Y, canopyBounds.Y);
    }

    [Fact]
    public void GetTrunkBounds_AnchorsToBottomOfTreeBounds()
    {
        var treeBounds = new Rectangle(100, 120, 64, 96);

        var trunkBounds = TreeRenderLayout.GetTrunkBounds(treeBounds);

        Assert.Equal(treeBounds.Bottom, trunkBounds.Bottom);
        Assert.True(treeBounds.Contains(trunkBounds));
    }
}
