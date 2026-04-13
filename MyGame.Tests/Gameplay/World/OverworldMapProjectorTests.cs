using Microsoft.Xna.Framework;
using MyGame.Gameplay.World;
using MyGame.Scenes.Gameplay;

namespace MyGame.Tests.Gameplay.World;

public sealed class OverworldMapProjectorTests
{
    [Fact]
    public void Create_WhenPlayerIsInTown_ProjectsMarkerIntoTownRegion()
    {
        var snapshot = OverworldMapProjector.Create(GameplaySceneNames.Overworld, new Vector2(400f, 240f));

        Assert.True(snapshot.HasPlayerMarker);
        Assert.Equal(new Vector2(1360f, 1200f), snapshot.PlayerMapPosition);
    }

    [Fact]
    public void Create_WhenPlayerIsInWildernessNorth_ProjectsMarkerIntoNorthRegion()
    {
        var snapshot = OverworldMapProjector.Create(GameplaySceneNames.WildernessNorth, new Vector2(120f, 240f));

        Assert.True(snapshot.HasPlayerMarker);
        Assert.Equal(new Vector2(1080f, 240f), snapshot.PlayerMapPosition);
    }

    [Fact]
    public void Create_WhenSceneIsInterior_HidesPlayerMarker()
    {
        var snapshot = OverworldMapProjector.Create(GameplaySceneNames.ShopInterior, new Vector2(120f, 240f));

        Assert.False(snapshot.HasPlayerMarker);
    }
}
