using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;

namespace MyGame.Rendering.Enemies;

public sealed class HornedRabbitEliteEnemyRenderer : IEnemyKindRenderer
{
    private static readonly Color DefeatedTint = new(137, 86, 84);
    private static readonly Color HitFlashTint = new(255, 240, 220);
    private static readonly Color RabbitBodyTint = new(166, 136, 118);
    private static readonly Color RabbitHornTint = new(255, 208, 118);
    private static readonly Color RabbitTrailTint = new(245, 182, 118, 120);
    private static readonly Color BombOuterColor = new(248, 210, 84);
    private static readonly Color BombInnerColor = new(255, 244, 164);
    private static readonly Color FuseColor = new(255, 238, 182);
    private static readonly Color ExplosionColor = new(255, 188, 90);

    private readonly IWorldRectangleRenderer _worldRectangleRenderer;

    public HornedRabbitEliteEnemyRenderer(IWorldRectangleRenderer worldRectangleRenderer)
    {
        _worldRectangleRenderer = worldRectangleRenderer;
    }

    public EnemyKind Kind => EnemyKind.HornedRabbitElite;

    public void Draw(EnemyActor enemy, FrameTime frameTime)
    {
        foreach (var bomb in enemy.Bombs)
        {
            DrawBomb(bomb);
        }

        var drawColor = enemy.State == EnemyState.Dead
            ? DefeatedTint * Math.Max(enemy.DefeatedVisibilityAlpha, 0.35f)
            : RabbitBodyTint;

        if (enemy.IsFlashingFromHit)
        {
            drawColor = Color.Lerp(drawColor, HitFlashTint, enemy.HitFlashAlpha);
        }

        var hornColor = enemy.IsFlashingFromHit
            ? Color.Lerp(RabbitHornTint, HitFlashTint, enemy.HitFlashAlpha)
            : RabbitHornTint;

        var body = new Rectangle(enemy.Bounds.X + 4, enemy.Bounds.Y + 10, 20, 14);
        var head = new Rectangle(enemy.Bounds.X + 16, enemy.Bounds.Y + 6, 10, 10);
        var backLeg = new Rectangle(enemy.Bounds.X + 2, enemy.Bounds.Y + 18, 7, 8);
        var frontLeg = new Rectangle(enemy.Bounds.X + 18, enemy.Bounds.Y + 20, 7, 6);
        var leftHorn = new Rectangle(enemy.Bounds.X + 16, enemy.Bounds.Y + 0, 4, 8);
        var rightHorn = new Rectangle(enemy.Bounds.X + 22, enemy.Bounds.Y - 1, 4, 9);

        if (enemy.State == EnemyState.Dashing)
        {
            _worldRectangleRenderer.Draw(GetDashTrail(enemy), RabbitTrailTint);
        }

        _worldRectangleRenderer.Draw(body, drawColor);
        _worldRectangleRenderer.Draw(head, drawColor);
        _worldRectangleRenderer.Draw(backLeg, drawColor * 0.88f);
        _worldRectangleRenderer.Draw(frontLeg, drawColor * 0.92f);
        _worldRectangleRenderer.Draw(leftHorn, hornColor);
        _worldRectangleRenderer.Draw(rightHorn, hornColor);
    }

    private void DrawBomb(EnemyBomb bomb)
    {
        if (bomb.IsExploding)
        {
            _worldRectangleRenderer.Draw(bomb.ExplosionBounds, ExplosionColor * Math.Max(bomb.ExplosionAlpha, 0.35f));
            return;
        }

        _worldRectangleRenderer.Draw(bomb.Bounds, BombOuterColor);
        var innerBounds = new Rectangle(bomb.Bounds.X + 2, bomb.Bounds.Y + 2, bomb.Bounds.Width - 4, bomb.Bounds.Height - 4);
        _worldRectangleRenderer.Draw(innerBounds, BombInnerColor);
        var fuseBounds = new Rectangle(bomb.Bounds.X + 4, bomb.Bounds.Y - 2, 4, 4);
        _worldRectangleRenderer.Draw(fuseBounds, FuseColor);
    }

    private static Rectangle GetDashTrail(EnemyActor enemy)
    {
        return enemy.DashDirection switch
        {
            Direction.Up => new Rectangle(enemy.Bounds.X + 8, enemy.Bounds.Y + 20, 12, 14),
            Direction.Down => new Rectangle(enemy.Bounds.X + 8, enemy.Bounds.Y - 8, 12, 14),
            Direction.Left => new Rectangle(enemy.Bounds.X + 18, enemy.Bounds.Y + 10, 14, 10),
            Direction.Right => new Rectangle(enemy.Bounds.X - 8, enemy.Bounds.Y + 10, 14, 10),
            _ => enemy.Bounds
        };
    }
}
