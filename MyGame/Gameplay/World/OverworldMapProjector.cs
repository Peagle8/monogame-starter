using Microsoft.Xna.Framework;
using MyGame.Scenes.Gameplay;

namespace MyGame.Gameplay.World;

public static class OverworldMapProjector
{
    public static OverworldMapSnapshot Create(string sceneName, Vector2 playerPosition)
    {
        var regions = CreateRegions();
        if (!OverworldLayoutMetrics.IsOverworldScene(sceneName))
        {
            return new OverworldMapSnapshot(
                regions,
                OverworldLayoutMetrics.OverworldMapBounds,
                sceneName,
                Vector2.Zero,
                false);
        }

        var sceneBounds = OverworldLayoutMetrics.GetMapSceneBounds(sceneName);
        var playerMapPosition = new Vector2(
            sceneBounds.X + playerPosition.X,
            sceneBounds.Y + playerPosition.Y);

        return new OverworldMapSnapshot(
            regions,
            OverworldLayoutMetrics.OverworldMapBounds,
            sceneName,
            playerMapPosition,
            true);
    }

    private static IReadOnlyList<OverworldMapRegion> CreateRegions()
    {
        return
        [
            new OverworldMapRegion(
                GameplaySceneNames.WildernessNorth,
                "North",
                OverworldLayoutMetrics.WildernessNorthBounds,
                false),
            new OverworldMapRegion(
                GameplaySceneNames.WildernessWest,
                "West",
                OverworldLayoutMetrics.WildernessWestBounds,
                false),
            new OverworldMapRegion(
                GameplaySceneNames.Overworld,
                "Town",
                OverworldLayoutMetrics.GetMapSceneBounds(GameplaySceneNames.Overworld),
                true),
            new OverworldMapRegion(
                GameplaySceneNames.WildernessEast,
                "East",
                OverworldLayoutMetrics.WildernessEastBounds,
                false),
            new OverworldMapRegion(
                GameplaySceneNames.WildernessSouth,
                "South",
                OverworldLayoutMetrics.WildernessSouthBounds,
                false)
        ];
    }
}
