using Microsoft.Xna.Framework;

namespace MyGame.Gameplay.Props;

public interface IWorldProp
{
    Vector2 Position { get; }

    Point Size { get; }

    bool BlocksMovement { get; }

    Rectangle Bounds { get; }

    Rectangle CollisionBounds { get; }
}
