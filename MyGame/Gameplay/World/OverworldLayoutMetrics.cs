using Microsoft.Xna.Framework;

namespace MyGame.Gameplay.World;

public static class OverworldLayoutMetrics
{
    public const int TownSize = 1920;
    public const int TownWallThickness = 80;
    public const int TownGateWidth = 176;
    public const int TransitionThickness = 44;
    public const int WildernessShortSize = 960;
    public const int WildernessLongSize = 1920;
    public const int MountainThickness = 156;
    public const float WildernessEdgeEntryInset = 72f;

    public static readonly Rectangle TownBounds = new(0, 0, TownSize, TownSize);
    public static readonly Rectangle TownCentralDistrictBounds = new(576, 576, 768, 768);
    public static readonly Rectangle TownNorthGateTrigger = new((TownSize - TownGateWidth) / 2, TownWallThickness, TownGateWidth, TransitionThickness);
    public static readonly Rectangle TownSouthGateTrigger = new((TownSize - TownGateWidth) / 2, TownSize - TownWallThickness - TransitionThickness, TownGateWidth, TransitionThickness);
    public static readonly Rectangle TownWestGateTrigger = new(TownWallThickness, (TownSize - TownGateWidth) / 2, TransitionThickness, TownGateWidth);
    public static readonly Rectangle TownEastGateTrigger = new(TownSize - TownWallThickness - TransitionThickness, (TownSize - TownGateWidth) / 2, TransitionThickness, TownGateWidth);

    public static readonly Rectangle WildernessNorthBounds = new(WildernessShortSize, 0, WildernessLongSize, WildernessShortSize);
    public static readonly Rectangle WildernessSouthBounds = new(WildernessShortSize, WildernessShortSize + TownSize, WildernessLongSize, WildernessShortSize);
    public static readonly Rectangle WildernessWestBounds = new(0, WildernessShortSize, WildernessShortSize, WildernessLongSize);
    public static readonly Rectangle WildernessEastBounds = new(WildernessShortSize + TownSize, WildernessShortSize, WildernessShortSize, WildernessLongSize);
    public static readonly Rectangle OverworldMapBounds = new(0, 0, TownSize + (WildernessShortSize * 2), TownSize + (WildernessShortSize * 2));

    public static bool IsOverworldScene(string sceneName)
    {
        return sceneName == Scenes.Gameplay.GameplaySceneNames.Overworld
            || sceneName == Scenes.Gameplay.GameplaySceneNames.WildernessNorth
            || sceneName == Scenes.Gameplay.GameplaySceneNames.WildernessSouth
            || sceneName == Scenes.Gameplay.GameplaySceneNames.WildernessWest
            || sceneName == Scenes.Gameplay.GameplaySceneNames.WildernessEast;
    }

    public static Rectangle GetMapSceneBounds(string sceneName)
    {
        return sceneName switch
        {
            Scenes.Gameplay.GameplaySceneNames.Overworld => new Rectangle(WildernessShortSize, WildernessShortSize, TownSize, TownSize),
            Scenes.Gameplay.GameplaySceneNames.WildernessNorth => WildernessNorthBounds,
            Scenes.Gameplay.GameplaySceneNames.WildernessSouth => WildernessSouthBounds,
            Scenes.Gameplay.GameplaySceneNames.WildernessWest => WildernessWestBounds,
            Scenes.Gameplay.GameplaySceneNames.WildernessEast => WildernessEastBounds,
            _ => Rectangle.Empty
        };
    }
}
