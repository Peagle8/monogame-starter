using Microsoft.Xna.Framework;

namespace MyGame.Gameplay.World;

public sealed record OverworldMapRegion(
    string SceneName,
    string Label,
    Rectangle Bounds,
    bool IsTown);
