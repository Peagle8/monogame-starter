using Microsoft.Xna.Framework;
using MyGame.Rendering.Menus;

namespace MyGame.Tests.Rendering.Menus;

public sealed class VerticalMenuLayoutTests
{
    [Fact]
    public void GetItemPosition_OffsetsEachItemBySpacing()
    {
        var position = VerticalMenuLayout.GetItemPosition(new Vector2(300f, 220f), 40f, 2);

        Assert.Equal(new Vector2(300f, 300f), position);
    }

    [Fact]
    public void GetItemColor_ReturnsHighlightForSelectedItem()
    {
        Assert.Equal(Color.Yellow, VerticalMenuLayout.GetItemColor(index: 1, selectedIndex: 1));
        Assert.Equal(Color.Gray, VerticalMenuLayout.GetItemColor(index: 0, selectedIndex: 1));
    }
}
