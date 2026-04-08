using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.World;

namespace MyGame.Rendering.Gameplay;

public sealed class PlayerShieldRenderer : IGameplayEntityRenderer
{
    private static readonly Color ShieldColor = new(112, 224, 255, 180);

    private readonly IWorldRectangleRenderer _worldRectangleRenderer;

    public PlayerShieldRenderer(IWorldRectangleRenderer worldRectangleRenderer)
    {
        _worldRectangleRenderer = worldRectangleRenderer;
    }

    public int DrawOrder => 99;

    public void Draw(World world, FrameTime frameTime)
    {
        if (!world.Player.IsShieldActive)
        {
            return;
        }

        foreach (var segment in CreateShieldSegments(world.Player.Bounds))
        {
            _worldRectangleRenderer.Draw(segment, ShieldColor);
        }
    }

    private static IReadOnlyList<Rectangle> CreateShieldSegments(Rectangle playerBounds)
    {
        var shieldBounds = new Rectangle(
            playerBounds.X - 6,
            playerBounds.Y - 6,
            playerBounds.Width + 12,
            playerBounds.Height + 12);
        const int thickness = 3;

        return
        [
            new Rectangle(shieldBounds.X + 8, shieldBounds.Y, shieldBounds.Width - 16, thickness),
            new Rectangle(shieldBounds.X + 8, shieldBounds.Bottom - thickness, shieldBounds.Width - 16, thickness),
            new Rectangle(shieldBounds.X, shieldBounds.Y + 8, thickness, shieldBounds.Height - 16),
            new Rectangle(shieldBounds.Right - thickness, shieldBounds.Y + 8, thickness, shieldBounds.Height - 16),
            new Rectangle(shieldBounds.X + 2, shieldBounds.Y + 2, 6, 6),
            new Rectangle(shieldBounds.Right - 8, shieldBounds.Y + 2, 6, 6),
            new Rectangle(shieldBounds.X + 2, shieldBounds.Bottom - 8, 6, 6),
            new Rectangle(shieldBounds.Right - 8, shieldBounds.Bottom - 8, 6, 6)
        ];
    }
}
