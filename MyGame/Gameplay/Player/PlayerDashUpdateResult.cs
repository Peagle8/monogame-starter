using Microsoft.Xna.Framework;

namespace MyGame.Gameplay.Player;

public sealed record PlayerDashUpdateResult(
    Vector2 Position,
    Direction Facing,
    PlayerDashState State);
