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
    private static readonly Color MissileBodyColor = new(174, 186, 201);
    private static readonly Color MissileNoseColor = new(255, 210, 112);
    private static readonly Color MissileFlameColor = new(255, 92, 48);

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
        if (projectile.Kind == PlayerRangedAttackKind.Missile)
        {
            DrawMissile(projectile);
            return;
        }

        var bounds = projectile.Bounds;
        var flareBounds = Inflate(bounds, 2);
        _worldRectangleRenderer.Draw(flareBounds, FireballFlareColor);
        _worldRectangleRenderer.Draw(bounds, FireballCoreColor);
    }

    private void DrawMissile(PlayerProjectile projectile)
    {
        var bounds = projectile.Bounds;
        var bodyBounds = GetMissileBodyBounds(bounds, projectile.Direction);
        var noseBounds = GetMissileNoseBounds(bounds, projectile.Direction);
        var flameBounds = GetMissileFlameBounds(bounds, projectile.Direction);

        _worldRectangleRenderer.Draw(flameBounds, MissileFlameColor);
        _worldRectangleRenderer.Draw(bodyBounds, MissileBodyColor);
        _worldRectangleRenderer.Draw(noseBounds, MissileNoseColor);
    }

    private static Rectangle GetMissileBodyBounds(Rectangle bounds, Direction direction)
    {
        return direction is Direction.Left or Direction.Right
            ? new Rectangle(bounds.X + 4, bounds.Center.Y - 4, Math.Max(8, bounds.Width - 8), 8)
            : new Rectangle(bounds.Center.X - 4, bounds.Y + 4, 8, Math.Max(8, bounds.Height - 8));
    }

    private static Rectangle GetMissileNoseBounds(Rectangle bounds, Direction direction)
    {
        return direction switch
        {
            Direction.Left => new Rectangle(bounds.X, bounds.Center.Y - 4, 6, 8),
            Direction.Right => new Rectangle(bounds.Right - 6, bounds.Center.Y - 4, 6, 8),
            Direction.Up => new Rectangle(bounds.Center.X - 4, bounds.Y, 8, 6),
            _ => new Rectangle(bounds.Center.X - 4, bounds.Bottom - 6, 8, 6)
        };
    }

    private static Rectangle GetMissileFlameBounds(Rectangle bounds, Direction direction)
    {
        return direction switch
        {
            Direction.Left => new Rectangle(bounds.Right - 4, bounds.Center.Y - 3, 4, 6),
            Direction.Right => new Rectangle(bounds.X, bounds.Center.Y - 3, 4, 6),
            Direction.Up => new Rectangle(bounds.Center.X - 3, bounds.Bottom - 4, 6, 4),
            _ => new Rectangle(bounds.Center.X - 3, bounds.Y, 6, 4)
        };
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
