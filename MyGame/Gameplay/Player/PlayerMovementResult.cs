using Microsoft.Xna.Framework;

namespace MyGame.Gameplay.Player;

public sealed record PlayerMovementResult(Vector2 Position, Direction Facing);
