using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Props;
using MyGame.Gameplay.World;

namespace MyGame.Rendering.Gameplay;

public sealed class DungeonEntranceEntityRenderer : IGameplayEntityRenderer
{
    private static readonly Color StoneColor = new(96, 101, 109);
    private static readonly Color ShadowColor = new(50, 55, 62);
    private static readonly Color OpeningColor = new(20, 24, 28);
    private static readonly Color AccentColor = new(152, 127, 82);

    private readonly IWorldRectangleRenderer _worldRectangleRenderer;

    public DungeonEntranceEntityRenderer(IWorldRectangleRenderer worldRectangleRenderer)
    {
        _worldRectangleRenderer = worldRectangleRenderer;
    }

    public int DrawOrder => 42;

    public void Draw(World world, FrameTime frameTime)
    {
        foreach (var entrance in world.GetProps<DungeonEntranceProp>())
        {
            DrawEntrance(entrance.Bounds);
        }
    }

    private void DrawEntrance(Rectangle bounds)
    {
        var frameBounds = new Rectangle(bounds.X + 10, bounds.Y + 18, bounds.Width - 20, bounds.Height - 18);
        var openingBounds = new Rectangle(
            frameBounds.X + 18,
            frameBounds.Y + 18,
            frameBounds.Width - 36,
            frameBounds.Height - 18);
        var lintelBounds = new Rectangle(frameBounds.X, frameBounds.Y, frameBounds.Width, 20);
        var stepBounds = new Rectangle(bounds.X + 18, bounds.Bottom - 16, bounds.Width - 36, 16);

        _worldRectangleRenderer.Draw(frameBounds, StoneColor);
        _worldRectangleRenderer.Draw(lintelBounds, AccentColor);
        _worldRectangleRenderer.Draw(openingBounds, OpeningColor);
        _worldRectangleRenderer.Draw(stepBounds, ShadowColor);
    }
}
