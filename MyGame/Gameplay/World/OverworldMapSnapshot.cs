using Microsoft.Xna.Framework;

namespace MyGame.Gameplay.World;

public sealed record OverworldMapSnapshot(
    IReadOnlyList<OverworldMapRegion> Regions,
    Rectangle MapBounds,
    string CurrentSceneName,
    Vector2 PlayerMapPosition,
    bool HasPlayerMarker);
