using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Props;
using MyGame.Gameplay.World;

namespace MyGame.Rendering.Gameplay;

public sealed class WallEntityRenderer : IGameplayEntityRenderer
{
    private static readonly Color WallColor = new(171, 122, 74);
    private static readonly Color WallShadeColor = new(128, 87, 49);

    private readonly IWorldRectangleRenderer _worldRectangleRenderer;

    public WallEntityRenderer(IWorldRectangleRenderer worldRectangleRenderer)
    {
        _worldRectangleRenderer = worldRectangleRenderer;
    }

    public int DrawOrder => 45;

    public void Draw(World world, FrameTime frameTime)
    {
        foreach (var wall in world.GetProps<WallProp>())
        {
            _worldRectangleRenderer.Draw(wall.Bounds, WallColor);

            var shadeBounds = new Rectangle(
                wall.Bounds.X,
                wall.Bounds.Bottom - Math.Max(4, wall.Bounds.Height / 5),
                wall.Bounds.Width,
                Math.Max(4, wall.Bounds.Height / 5));
            _worldRectangleRenderer.Draw(shadeBounds, WallShadeColor);
        }
    }
}
