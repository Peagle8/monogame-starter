using Microsoft.Xna.Framework;

namespace MyGame.Gameplay.Props;

public sealed class TreeProp : IWorldProp
{
    public TreeProp(Vector2 position, Point size)
    {
        Position = position;
        Size = size;
    }

    public Vector2 Position { get; }

    public Point Size { get; }

    public bool BlocksMovement => true;

    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, Size.X, Size.Y);

    public Rectangle CollisionBounds
    {
        get
        {
            var collisionWidth = Math.Max(16, Size.X / 3);
            var collisionHeight = Math.Max(18, Size.Y / 4);
            var collisionX = (int)Position.X + ((Size.X - collisionWidth) / 2);
            var collisionY = ((int)Position.Y + Size.Y) - collisionHeight;
            return new Rectangle(collisionX, collisionY, collisionWidth, collisionHeight);
        }
    }
}
