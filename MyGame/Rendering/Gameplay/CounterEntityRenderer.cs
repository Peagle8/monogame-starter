using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Props;
using MyGame.Gameplay.World;

namespace MyGame.Rendering.Gameplay;

public sealed class CounterEntityRenderer : IGameplayEntityRenderer
{
    private static readonly Color CounterTopColor = new(123, 74, 45);
    private static readonly Color CounterBodyColor = new(96, 58, 35);
    private static readonly Color CounterTrimColor = new(168, 121, 79);

    private readonly IWorldRectangleRenderer _worldRectangleRenderer;

    public CounterEntityRenderer(IWorldRectangleRenderer worldRectangleRenderer)
    {
        _worldRectangleRenderer = worldRectangleRenderer;
    }

    public int DrawOrder => 110;

    public void Draw(World world, FrameTime frameTime)
    {
        foreach (var counter in world.GetProps<CounterProp>())
        {
            var topBounds = new Rectangle(
                counter.Bounds.X,
                counter.Bounds.Y,
                counter.Bounds.Width,
                Math.Max(6, counter.Bounds.Height / 3));
            var bodyBounds = new Rectangle(
                counter.Bounds.X,
                topBounds.Bottom,
                counter.Bounds.Width,
                counter.Bounds.Bottom - topBounds.Bottom);
            var trimBounds = new Rectangle(
                counter.Bounds.X + 8,
                bodyBounds.Y + 6,
                Math.Max(8, counter.Bounds.Width - 16),
                Math.Max(6, bodyBounds.Height / 3));

            _worldRectangleRenderer.Draw(bodyBounds, CounterBodyColor);
            _worldRectangleRenderer.Draw(topBounds, CounterTopColor);
            _worldRectangleRenderer.Draw(trimBounds, CounterTrimColor);
        }
    }
}
