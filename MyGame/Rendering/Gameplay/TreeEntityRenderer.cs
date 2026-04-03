using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Scenes.Gameplay;

namespace MyGame.Rendering.Gameplay;

public sealed class TreeEntityRenderer : IGameplayEntityRenderer
{
    private static readonly Color CanopyColor = new(46, 125, 50);
    private static readonly Color CanopyShadeColor = new(27, 94, 32);
    private static readonly Color TrunkColor = new(109, 76, 65);

    private readonly IWorldRectangleRenderer _worldRectangleRenderer;

    public TreeEntityRenderer(IWorldRectangleRenderer worldRectangleRenderer)
    {
        _worldRectangleRenderer = worldRectangleRenderer;
    }

    public int DrawOrder => 50;

    public void Draw(GameplayScene scene, FrameTime frameTime)
    {
        foreach (var tree in scene.TreeProps)
        {
            var canopyBounds = TreeRenderLayout.GetCanopyBounds(tree.Bounds);
            var trunkBounds = TreeRenderLayout.GetTrunkBounds(tree.Bounds);
            var canopyShadeBounds = new Rectangle(
                canopyBounds.X,
                canopyBounds.Y + (canopyBounds.Height / 2),
                canopyBounds.Width,
                canopyBounds.Height / 2);

            _worldRectangleRenderer.Draw(canopyBounds, CanopyColor);
            _worldRectangleRenderer.Draw(canopyShadeBounds, CanopyShadeColor);
            _worldRectangleRenderer.Draw(trunkBounds, TrunkColor);
        }
    }
}
