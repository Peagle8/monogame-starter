using Microsoft.Xna.Framework;

namespace MyGame.Gameplay.Props;

public sealed class ArenaEntranceProp : IWorldProp
{
    public ArenaEntranceProp(Vector2 position, Point size, Rectangle doorBounds, string signText = "Arena")
    {
        Position = position;
        Size = size;
        DoorBounds = doorBounds;
        SignText = signText;
    }

    public Vector2 Position { get; }

    public Point Size { get; }

    public Rectangle DoorBounds { get; }

    public string SignText { get; }

    public bool BlocksMovement => false;

    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, Size.X, Size.Y);

    public Rectangle CollisionBounds => Bounds;
}
