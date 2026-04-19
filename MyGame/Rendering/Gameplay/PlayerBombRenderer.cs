using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.World;

namespace MyGame.Rendering.Gameplay;

public sealed class PlayerBombRenderer : IGameplayEntityRenderer
{
    private static readonly Color BombOuterColor = new(246, 188, 84);
    private static readonly Color BombInnerColor = new(255, 236, 164);
    private static readonly Color ExplosionColor = new(255, 178, 84, 180);

    private readonly IWorldRectangleRenderer _worldRectangleRenderer;

    public PlayerBombRenderer(IWorldRectangleRenderer worldRectangleRenderer)
    {
        _worldRectangleRenderer = worldRectangleRenderer;
    }

    public int DrawOrder => 94;

    public void Draw(World world, FrameTime frameTime)
    {
        foreach (var bomb in world.PlayerBombs)
        {
            DrawBomb(bomb);
        }
    }

    private void DrawBomb(PlayerBomb bomb)
    {
        if (bomb.IsExploding)
        {
            _worldRectangleRenderer.Draw(bomb.ExplosionBounds, ExplosionColor * bomb.ExplosionAlpha);
        }

        if (bomb.FuseAlpha <= 0f)
        {
            return;
        }

        var innerBounds = Inflate(bomb.Bounds, -2);
        _worldRectangleRenderer.Draw(bomb.Bounds, BombOuterColor * bomb.FuseAlpha);
        _worldRectangleRenderer.Draw(innerBounds, BombInnerColor * bomb.FuseAlpha);
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
