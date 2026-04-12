using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.World;

namespace MyGame.Rendering.Gameplay;

public sealed class PlayerStunRenderer : IGameplayEntityRenderer
{
    private static readonly Color StunColor = new(255, 228, 112, 220);
    private static readonly Color SparkColor = new(255, 248, 196, 235);

    private readonly IWorldRectangleRenderer _worldRectangleRenderer;

    public PlayerStunRenderer(IWorldRectangleRenderer worldRectangleRenderer)
    {
        _worldRectangleRenderer = worldRectangleRenderer;
    }

    public int DrawOrder => 101;

    public void Draw(World world, FrameTime frameTime)
    {
        if (!world.Player.IsStunned)
        {
            return;
        }

        foreach (var segment in CreateIndicatorSegments(world.Player.Bounds))
        {
            _worldRectangleRenderer.Draw(segment.Bounds, segment.Color);
        }
    }

    private static IReadOnlyList<IndicatorSegment> CreateIndicatorSegments(Rectangle playerBounds)
    {
        var centerX = playerBounds.Center.X;
        var topY = playerBounds.Y - 18;

        return
        [
            new IndicatorSegment(new Rectangle(centerX - 10, topY, 20, 4), StunColor),
            new IndicatorSegment(new Rectangle(centerX - 8, topY + 4, 16, 4), StunColor),
            new IndicatorSegment(new Rectangle(centerX - 6, topY + 8, 12, 4), StunColor),
            new IndicatorSegment(new Rectangle(centerX - 1, topY - 6, 3, 6), SparkColor),
            new IndicatorSegment(new Rectangle(centerX - 14, topY + 2, 3, 3), SparkColor),
            new IndicatorSegment(new Rectangle(centerX + 11, topY + 2, 3, 3), SparkColor)
        ];
    }

    private readonly record struct IndicatorSegment(Rectangle Bounds, Color Color);
}
