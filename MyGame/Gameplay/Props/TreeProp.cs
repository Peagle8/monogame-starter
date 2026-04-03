using Microsoft.Xna.Framework;

namespace MyGame.Gameplay.Props;

public sealed class TreeProp
{
    public TreeProp(Vector2 position, Point size)
    {
        Position = position;
        Size = size;
    }

    public Vector2 Position { get; }

    public Point Size { get; }

    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, Size.X, Size.Y);
}
