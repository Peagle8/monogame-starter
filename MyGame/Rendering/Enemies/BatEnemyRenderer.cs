using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;

namespace MyGame.Rendering.Enemies;

public sealed class BatEnemyRenderer : IEnemyKindRenderer
{
    private static readonly Color DefeatedTint = new(96, 74, 88);
    private static readonly Color HitFlashTint = new(255, 240, 220);
    private static readonly Color WingTint = new(26, 34, 68);
    private static readonly Color BodyTint = new(12, 12, 18);
    private static readonly Color EyeTint = new(118, 18, 26);
    private static readonly Color TrailTint = new(40, 54, 96, 110);

    private readonly IWorldRectangleRenderer _worldRectangleRenderer;

    public BatEnemyRenderer(IWorldRectangleRenderer worldRectangleRenderer)
    {
        _worldRectangleRenderer = worldRectangleRenderer;
    }

    public EnemyKind Kind => EnemyKind.Bat;

    public void Draw(EnemyActor enemy, FrameTime frameTime)
    {
        var bodyColor = enemy.State == EnemyState.Dead
            ? DefeatedTint * Math.Max(enemy.DefeatedVisibilityAlpha, 0.35f)
            : BodyTint;
        var wingColor = enemy.State == EnemyState.Dead
            ? DefeatedTint * Math.Max(enemy.DefeatedVisibilityAlpha, 0.45f)
            : WingTint;

        if (enemy.IsFlashingFromHit)
        {
            bodyColor = Color.Lerp(bodyColor, HitFlashTint, enemy.HitFlashAlpha);
            wingColor = Color.Lerp(wingColor, HitFlashTint, enemy.HitFlashAlpha * 0.85f);
        }

        if (enemy.State == EnemyState.Dashing)
        {
            _worldRectangleRenderer.Draw(GetTrail(enemy), TrailTint);
        }

        var leftWing = new Rectangle(enemy.Bounds.X + 1, enemy.Bounds.Y + 8, 11, 12);
        var rightWing = new Rectangle(enemy.Bounds.X + 16, enemy.Bounds.Y + 8, 11, 12);
        var body = new Rectangle(enemy.Bounds.X + 9, enemy.Bounds.Y + 9, 10, 12);
        var head = new Rectangle(enemy.Bounds.X + 10, enemy.Bounds.Y + 5, 8, 6);
        var leftEar = new Rectangle(enemy.Bounds.X + 10, enemy.Bounds.Y + 2, 2, 4);
        var rightEar = new Rectangle(enemy.Bounds.X + 16, enemy.Bounds.Y + 2, 2, 4);
        var leftEye = new Rectangle(enemy.Bounds.X + 11, enemy.Bounds.Y + 7, 2, 2);
        var rightEye = new Rectangle(enemy.Bounds.X + 15, enemy.Bounds.Y + 7, 2, 2);

        _worldRectangleRenderer.Draw(leftWing, wingColor);
        _worldRectangleRenderer.Draw(rightWing, wingColor);
        _worldRectangleRenderer.Draw(body, bodyColor);
        _worldRectangleRenderer.Draw(head, bodyColor * 1.08f);
        _worldRectangleRenderer.Draw(leftEar, bodyColor);
        _worldRectangleRenderer.Draw(rightEar, bodyColor);
        _worldRectangleRenderer.Draw(leftEye, EyeTint);
        _worldRectangleRenderer.Draw(rightEye, EyeTint);
    }

    private static Rectangle GetTrail(EnemyActor enemy)
    {
        return enemy.DashDirection switch
        {
            Direction.Up => new Rectangle(enemy.Bounds.X + 6, enemy.Bounds.Y + 18, 16, 10),
            Direction.Down => new Rectangle(enemy.Bounds.X + 6, enemy.Bounds.Y - 2, 16, 10),
            Direction.Left => new Rectangle(enemy.Bounds.X + 16, enemy.Bounds.Y + 6, 12, 16),
            Direction.Right => new Rectangle(enemy.Bounds.X, enemy.Bounds.Y + 6, 12, 16),
            _ => enemy.Bounds
        };
    }
}
