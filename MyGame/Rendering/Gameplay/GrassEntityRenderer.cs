using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.World;

namespace MyGame.Rendering.Gameplay;

public sealed class GrassEntityRenderer : IGameplayEntityRenderer
{
    private static readonly Color GrassColor = new(76, 175, 80);
    private static readonly Color GrassShadeColor = new(56, 142, 60);

    private readonly IWorldRectangleRenderer _worldRectangleRenderer;

    public GrassEntityRenderer(IWorldRectangleRenderer worldRectangleRenderer)
    {
        _worldRectangleRenderer = worldRectangleRenderer;
    }

    public int DrawOrder => 40;

    public void Draw(World world, FrameTime frameTime)
    {
        foreach (var grass in world.GrassProps)
        {
            var leftBlade = new Rectangle(
                grass.Bounds.X,
                grass.Bounds.Y + 6,
                grass.Bounds.Width / 3,
                grass.Bounds.Height - 6);
            var centerBlade = new Rectangle(
                grass.Bounds.X + (grass.Bounds.Width / 3),
                grass.Bounds.Y,
                grass.Bounds.Width / 3,
                grass.Bounds.Height);
            var rightBlade = new Rectangle(
                grass.Bounds.Right - (grass.Bounds.Width / 3),
                grass.Bounds.Y + 8,
                grass.Bounds.Width / 3,
                grass.Bounds.Height - 8);

            _worldRectangleRenderer.Draw(leftBlade, GrassShadeColor);
            _worldRectangleRenderer.Draw(centerBlade, GrassColor);
            _worldRectangleRenderer.Draw(rightBlade, GrassShadeColor);
        }
    }
}
