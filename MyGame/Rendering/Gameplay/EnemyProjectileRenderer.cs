using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.World;

namespace MyGame.Rendering.Gameplay;

public sealed class EnemyProjectileRenderer : IGameplayEntityRenderer
{
    private static readonly Color ArrowShaftColor = new(133, 96, 72);
    private static readonly Color ArrowHeadColor = new(214, 224, 236);
    private static readonly Color ArrowFletchingColor = new(182, 78, 58);

    private readonly IWorldRectangleRenderer _worldRectangleRenderer;

    public EnemyProjectileRenderer(IWorldRectangleRenderer worldRectangleRenderer)
    {
        _worldRectangleRenderer = worldRectangleRenderer;
    }

    public int DrawOrder => 96;

    public void Draw(World world, FrameTime frameTime)
    {
        foreach (var projectile in world.EnemyProjectiles)
        {
            if (!projectile.IsActive)
            {
                continue;
            }

            DrawProjectile(projectile);
        }
    }

    private void DrawProjectile(EnemyProjectile projectile)
    {
        var facing = DirectionHelper.FromDominantAxis(projectile.Velocity, Direction.Right);
        var bounds = projectile.Bounds;
        var shaftBounds = facing is Direction.Left or Direction.Right
            ? new Rectangle(bounds.X, bounds.Center.Y - 1, bounds.Width, 2)
            : new Rectangle(bounds.Center.X - 1, bounds.Y, 2, bounds.Height);
        var headBounds = facing switch
        {
            Direction.Left => new Rectangle(bounds.X - 2, bounds.Center.Y - 2, 4, 4),
            Direction.Right => new Rectangle(bounds.Right - 2, bounds.Center.Y - 2, 4, 4),
            Direction.Up => new Rectangle(bounds.Center.X - 2, bounds.Y - 2, 4, 4),
            Direction.Down => new Rectangle(bounds.Center.X - 2, bounds.Bottom - 2, 4, 4),
            _ => bounds
        };
        var fletchingBounds = facing switch
        {
            Direction.Left => new Rectangle(bounds.Right - 2, bounds.Center.Y - 3, 3, 6),
            Direction.Right => new Rectangle(bounds.X - 1, bounds.Center.Y - 3, 3, 6),
            Direction.Up => new Rectangle(bounds.Center.X - 3, bounds.Bottom - 2, 6, 3),
            Direction.Down => new Rectangle(bounds.Center.X - 3, bounds.Y - 1, 6, 3),
            _ => bounds
        };

        _worldRectangleRenderer.Draw(shaftBounds, ArrowShaftColor);
        _worldRectangleRenderer.Draw(headBounds, ArrowHeadColor);
        _worldRectangleRenderer.Draw(fletchingBounds, ArrowFletchingColor);
    }
}
