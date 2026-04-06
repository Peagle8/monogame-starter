using Microsoft.Xna.Framework;

namespace MyGame.Gameplay.Player;

public static class DirectionHelper
{
    public static Vector2 ToVector(Direction direction)
    {
        return direction switch
        {
            Direction.Up => new Vector2(0f, -1f),
            Direction.Down => new Vector2(0f, 1f),
            Direction.Left => new Vector2(-1f, 0f),
            Direction.Right => new Vector2(1f, 0f),
            _ => Vector2.Zero
        };
    }

    public static Direction FromDominantAxis(Vector2 vector, Direction fallback)
    {
        if (vector == Vector2.Zero)
        {
            return fallback;
        }

        if (Math.Abs(vector.X) > Math.Abs(vector.Y))
        {
            return vector.X < 0f ? Direction.Left : Direction.Right;
        }

        return vector.Y < 0f ? Direction.Up : Direction.Down;
    }
}
