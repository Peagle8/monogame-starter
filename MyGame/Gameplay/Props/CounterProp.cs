using Microsoft.Xna.Framework;

namespace MyGame.Gameplay.Props;

public sealed class CounterProp : IWorldProp
{
    public CounterProp(Vector2 position, Point size)
    {
        Position = position;
        Size = size;
    }

    public Vector2 Position { get; }

    public Point Size { get; }

    public bool BlocksMovement => true;

    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, Size.X, Size.Y);

    public Rectangle CollisionBounds => Bounds;
}
