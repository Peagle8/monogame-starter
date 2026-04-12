using Microsoft.Xna.Framework;
using MyGame.Gameplay.Player;

namespace MyGame.Gameplay.Enemies;

internal static class BatMiniBossConeShape
{
    private const float WidthScale = 1.2f;

    public static IReadOnlyList<Rectangle> GetSegments(EnemyActor enemy)
    {
        var bounds = enemy.Bounds;
        var segmentLength = Math.Max(12, (int)(enemy.SpecialAttackRange / 3f));
        var nearWidth = Scale(20);
        var midWidth = Scale(32);
        var farWidth = Scale(48);

        return enemy.DashDirection switch
        {
            Direction.Up =>
            [
                new Rectangle(bounds.Center.X - (nearWidth / 2), bounds.Y - segmentLength, nearWidth, segmentLength),
                new Rectangle(bounds.Center.X - (midWidth / 2), bounds.Y - (segmentLength * 2), midWidth, segmentLength),
                new Rectangle(bounds.Center.X - (farWidth / 2), bounds.Y - (segmentLength * 3), farWidth, segmentLength)
            ],
            Direction.Down =>
            [
                new Rectangle(bounds.Center.X - (nearWidth / 2), bounds.Bottom, nearWidth, segmentLength),
                new Rectangle(bounds.Center.X - (midWidth / 2), bounds.Bottom + segmentLength, midWidth, segmentLength),
                new Rectangle(bounds.Center.X - (farWidth / 2), bounds.Bottom + (segmentLength * 2), farWidth, segmentLength)
            ],
            Direction.Left =>
            [
                new Rectangle(bounds.X - segmentLength, bounds.Center.Y - (nearWidth / 2), segmentLength, nearWidth),
                new Rectangle(bounds.X - (segmentLength * 2), bounds.Center.Y - (midWidth / 2), segmentLength, midWidth),
                new Rectangle(bounds.X - (segmentLength * 3), bounds.Center.Y - (farWidth / 2), segmentLength, farWidth)
            ],
            _ =>
            [
                new Rectangle(bounds.Right, bounds.Center.Y - (nearWidth / 2), segmentLength, nearWidth),
                new Rectangle(bounds.Right + segmentLength, bounds.Center.Y - (midWidth / 2), segmentLength, midWidth),
                new Rectangle(bounds.Right + (segmentLength * 2), bounds.Center.Y - (farWidth / 2), segmentLength, farWidth)
            ]
        };
    }

    private static int Scale(int value)
    {
        return (int)MathF.Round(value * WidthScale);
    }
}
