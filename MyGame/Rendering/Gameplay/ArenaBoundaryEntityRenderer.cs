using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Props;
using MyGame.Gameplay.World;

namespace MyGame.Rendering.Gameplay;

public sealed class ArenaBoundaryEntityRenderer : IGameplayEntityRenderer
{
    private static readonly Color OuterColor = new(78, 84, 91);
    private static readonly Color InnerColor = new(110, 118, 126);
    private static readonly Color ShadowColor = new(43, 49, 55);
    private static readonly Color AccentColor = new(144, 132, 96);

    private readonly IWorldRectangleRenderer _worldRectangleRenderer;

    public ArenaBoundaryEntityRenderer(IWorldRectangleRenderer worldRectangleRenderer)
    {
        _worldRectangleRenderer = worldRectangleRenderer;
    }

    public int DrawOrder => 44;

    public void Draw(World world, FrameTime frameTime)
    {
        foreach (var boundary in world.GetProps<ArenaBoundaryProp>())
        {
            DrawBoundary(boundary.Bounds);
        }
    }

    private void DrawBoundary(Rectangle bounds)
    {
        _worldRectangleRenderer.Draw(bounds, OuterColor);

        var trim = Math.Max(6, Math.Min(bounds.Width, bounds.Height) / 6);
        var innerBounds = Inflate(bounds, -trim);
        if (innerBounds.Width > 0 && innerBounds.Height > 0)
        {
            _worldRectangleRenderer.Draw(innerBounds, InnerColor);
        }

        if (bounds.Width >= bounds.Height)
        {
            DrawHorizontalDetails(bounds);
            return;
        }

        DrawVerticalDetails(bounds);
    }

    private void DrawHorizontalDetails(Rectangle bounds)
    {
        var railHeight = Math.Max(6, bounds.Height / 5);
        var topRail = new Rectangle(bounds.X, bounds.Y, bounds.Width, railHeight);
        var bottomRail = new Rectangle(bounds.X, bounds.Bottom - railHeight, bounds.Width, railHeight);
        _worldRectangleRenderer.Draw(topRail, AccentColor);
        _worldRectangleRenderer.Draw(bottomRail, ShadowColor);

        var seatBandHeight = Math.Max(8, bounds.Height / 4);
        var seatBand = new Rectangle(
            bounds.X,
            bounds.Center.Y - (seatBandHeight / 2),
            bounds.Width,
            seatBandHeight);
        _worldRectangleRenderer.Draw(seatBand, ShadowColor);
    }

    private void DrawVerticalDetails(Rectangle bounds)
    {
        var railWidth = Math.Max(6, bounds.Width / 5);
        var leftRail = new Rectangle(bounds.X, bounds.Y, railWidth, bounds.Height);
        var rightRail = new Rectangle(bounds.Right - railWidth, bounds.Y, railWidth, bounds.Height);
        _worldRectangleRenderer.Draw(leftRail, ShadowColor);
        _worldRectangleRenderer.Draw(rightRail, AccentColor);

        var pillarHeight = Math.Max(28, bounds.Height / 5);
        for (var y = bounds.Y + 18; y < bounds.Bottom - pillarHeight; y += pillarHeight + 18)
        {
            var pillar = new Rectangle(
                bounds.Center.X - (railWidth / 2),
                y,
                railWidth,
                pillarHeight);
            _worldRectangleRenderer.Draw(pillar, ShadowColor);
        }
    }

    private static Rectangle Inflate(Rectangle rectangle, int amount)
    {
        return new Rectangle(
            rectangle.X - amount,
            rectangle.Y - amount,
            rectangle.Width + (amount * 2),
            rectangle.Height + (amount * 2));
    }
}
