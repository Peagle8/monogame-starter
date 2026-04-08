using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.World;

namespace MyGame.Rendering.Gameplay;

public sealed class PlayerProjectileRenderer : IGameplayEntityRenderer
{
    private static readonly Color FireballCoreColor = new(255, 182, 72);
    private static readonly Color FireballFlareColor = new(255, 115, 48);

    private readonly IWorldRectangleRenderer _worldRectangleRenderer;

    public PlayerProjectileRenderer(IWorldRectangleRenderer worldRectangleRenderer)
    {
        _worldRectangleRenderer = worldRectangleRenderer;
    }

    public int DrawOrder => 95;

    public void Draw(World world, FrameTime frameTime)
    {
        foreach (var projectile in world.PlayerProjectiles)
        {
            if (!projectile.IsActive)
            {
                continue;
            }

            DrawProjectile(projectile);
        }
    }

    private void DrawProjectile(PlayerProjectile projectile)
    {
        var bounds = projectile.Bounds;
        var flareBounds = Inflate(bounds, 2);
        _worldRectangleRenderer.Draw(flareBounds, FireballFlareColor);
        _worldRectangleRenderer.Draw(bounds, FireballCoreColor);
    }

    private static Rectangle Inflate(Rectangle bounds, int amount)
    {
        return new Rectangle(
            bounds.X - amount,
            bounds.Y - amount,
            bounds.Width + (amount * 2),
            bounds.Height + (amount * 2));
    }
}
