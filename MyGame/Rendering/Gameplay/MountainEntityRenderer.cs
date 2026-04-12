using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Props;
using MyGame.Gameplay.World;

namespace MyGame.Rendering.Gameplay;

public sealed class MountainEntityRenderer : IGameplayEntityRenderer
{
    private static readonly Color BaseColor = new(87, 93, 100);
    private static readonly Color MidColor = new(109, 116, 123);
    private static readonly Color PeakColor = new(151, 156, 161);
    private static readonly Color ShadowColor = new(61, 66, 73);

    private readonly IWorldRectangleRenderer _worldRectangleRenderer;

    public MountainEntityRenderer(IWorldRectangleRenderer worldRectangleRenderer)
    {
        _worldRectangleRenderer = worldRectangleRenderer;
    }

    public int DrawOrder => 41;

    public void Draw(World world, FrameTime frameTime)
    {
        foreach (var mountain in world.GetProps<MountainProp>())
        {
            DrawMountain(mountain.Bounds);
        }
    }

    private void DrawMountain(Rectangle bounds)
    {
        _worldRectangleRenderer.Draw(bounds, BaseColor);

        var ridgeBounds = new Rectangle(
            bounds.X,
            bounds.Y + (bounds.Height / 3),
            bounds.Width,
            Math.Max(12, bounds.Height / 3));
        _worldRectangleRenderer.Draw(ridgeBounds, MidColor);

        var peakBounds = new Rectangle(
            bounds.X,
            bounds.Y,
            bounds.Width,
            Math.Max(10, bounds.Height / 4));
        _worldRectangleRenderer.Draw(peakBounds, PeakColor);

        var shadowBounds = new Rectangle(
            bounds.X,
            bounds.Bottom - Math.Max(12, bounds.Height / 4),
            bounds.Width,
            Math.Max(12, bounds.Height / 4));
        _worldRectangleRenderer.Draw(shadowBounds, ShadowColor);
    }
}
