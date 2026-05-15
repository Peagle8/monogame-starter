using Microsoft.Xna.Framework;

namespace MyGame.Gameplay.Props;

public sealed class ArenaBoundaryProp : IWorldProp
{
    public ArenaBoundaryProp(Vector2 position, Point size, bool isVisible = true)
    {
        Position = position;
        Size = size;
        IsVisible = isVisible;
    }

    public Vector2 Position { get; }

    public Point Size { get; }

    public bool IsVisible { get; }

    public bool BlocksMovement => true;

    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, Size.X, Size.Y);

    public Rectangle CollisionBounds => Bounds;
}
