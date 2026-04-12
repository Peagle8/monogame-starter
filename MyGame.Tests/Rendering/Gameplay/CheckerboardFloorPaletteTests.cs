using Microsoft.Xna.Framework;
using MyGame.Rendering.Gameplay;
using MyGame.Scenes.Gameplay;

namespace MyGame.Tests.Rendering.Gameplay;

public sealed class CheckerboardFloorPaletteTests
{
    [Fact]
    public void GetTileColor_AlternatesAcrossAdjacentTiles()
    {
        var first = CheckerboardFloorPalette.GetTileColor(0, 0);
        var horizontalNeighbor = CheckerboardFloorPalette.GetTileColor(1, 0);
        var verticalNeighbor = CheckerboardFloorPalette.GetTileColor(0, 1);

        Assert.NotEqual(first, horizontalNeighbor);
        Assert.NotEqual(first, verticalNeighbor);
    }

    [Fact]
    public void GetTileColor_RepeatsPatternOnEvenOffset()
    {
        var first = CheckerboardFloorPalette.GetTileColor(0, 0);
        var repeated = CheckerboardFloorPalette.GetTileColor(2, 0);

        Assert.Equal(first, repeated);
    }

    [Fact]
    public void GetTileColor_WhenArenaSceneUsesWarmPalette()
    {
        var arenaTile = CheckerboardFloorPalette.GetTileColor(0, 0, GameplaySceneNames.Arena);
        var overworldTile = CheckerboardFloorPalette.GetTileColor(0, 0, GameplaySceneNames.Overworld);

        Assert.Equal(new Color(160, 135, 98), arenaTile);
        Assert.NotEqual(overworldTile, arenaTile);
    }
}
