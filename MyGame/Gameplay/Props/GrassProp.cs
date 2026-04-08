using Microsoft.Xna.Framework;

namespace MyGame.Gameplay.Props;

public sealed class GrassProp : IWorldProp
{
    public GrassProp(Vector2 position, Point size)
    {
        Position = position;
        Size = size;
    }

    public Vector2 Position { get; }

    public Point Size { get; }

    public bool BlocksMovement => false;

    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, Size.X, Size.Y);

    public Rectangle CollisionBounds => Bounds;
}
