using Microsoft.Xna.Framework;

namespace MyGame.Gameplay.Props;

public sealed class ShopExteriorProp : IWorldProp
{
    public ShopExteriorProp(Vector2 position, Point size, Rectangle doorBounds)
    {
        Position = position;
        Size = size;
        DoorBounds = doorBounds;
    }

    public Vector2 Position { get; }

    public Point Size { get; }

    public Rectangle DoorBounds { get; }

    public bool BlocksMovement => false;

    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, Size.X, Size.Y);

    public Rectangle CollisionBounds => Bounds;
}
