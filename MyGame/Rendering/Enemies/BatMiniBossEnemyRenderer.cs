using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;

namespace MyGame.Rendering.Enemies;

public sealed class BatMiniBossEnemyRenderer : IEnemyKindRenderer
{
    private static readonly Color DefeatedTint = new(88, 60, 72);
    private static readonly Color HitFlashTint = new(255, 240, 220);
    private static readonly Color WingTint = new(52, 38, 90);
    private static readonly Color BodyTint = new(18, 16, 28);
    private static readonly Color EyeTint = new(215, 72, 88);
    private static readonly Color TrailTint = new(74, 88, 138, 120);
    private static readonly Color TelegraphTint = new(244, 210, 94, 110);
    private static readonly Color BlastTint = new(244, 148, 86, 170);

    private readonly IWorldRectangleRenderer _worldRectangleRenderer;

    public BatMiniBossEnemyRenderer(IWorldRectangleRenderer worldRectangleRenderer)
    {
        _worldRectangleRenderer = worldRectangleRenderer;
    }

    public EnemyKind Kind => EnemyKind.BatMiniBoss;

    public void Draw(EnemyActor enemy, FrameTime frameTime)
    {
        if (enemy.IsSpecialAttackTelegraphVisible)
        {
            DrawChargeTelegraph(enemy);
        }

        if (enemy.IsSpecialAttackActive)
        {
            DrawCone(enemy, BlastTint);
        }

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

        var bounds = enemy.Bounds;
        var leftWing = new Rectangle(bounds.X + 2, bounds.Y + 16, 21, 22);
        var rightWing = new Rectangle(bounds.X + 33, bounds.Y + 16, 21, 22);
        var body = new Rectangle(bounds.X + 16, bounds.Y + 17, 24, 25);
        var head = new Rectangle(bounds.X + 18, bounds.Y + 9, 20, 11);
        var leftEar = new Rectangle(bounds.X + 18, bounds.Y + 4, 5, 8);
        var rightEar = new Rectangle(bounds.X + 33, bounds.Y + 4, 5, 8);
        var leftEye = new Rectangle(bounds.X + 22, bounds.Y + 13, 4, 4);
        var rightEye = new Rectangle(bounds.X + 30, bounds.Y + 13, 4, 4);

        _worldRectangleRenderer.Draw(leftWing, wingColor);
        _worldRectangleRenderer.Draw(rightWing, wingColor);
        _worldRectangleRenderer.Draw(body, bodyColor);
        _worldRectangleRenderer.Draw(head, bodyColor * 1.08f);
        _worldRectangleRenderer.Draw(leftEar, bodyColor);
        _worldRectangleRenderer.Draw(rightEar, bodyColor);
        _worldRectangleRenderer.Draw(leftEye, EyeTint);
        _worldRectangleRenderer.Draw(rightEye, EyeTint);
    }

    private void DrawCone(EnemyActor enemy, Color color)
    {
        foreach (var segment in BatMiniBossConeShape.GetSegments(enemy))
        {
            _worldRectangleRenderer.Draw(segment, color);
        }
    }

    private void DrawChargeTelegraph(EnemyActor enemy)
    {
        foreach (var indicator in GetChargeTelegraphSegments(enemy))
        {
            _worldRectangleRenderer.Draw(indicator, TelegraphTint);
        }
    }

    private static Rectangle GetTrail(EnemyActor enemy)
    {
        return enemy.DashDirection switch
        {
            Direction.Up => new Rectangle(enemy.Bounds.X + 12, enemy.Bounds.Y + 38, 32, 14),
            Direction.Down => new Rectangle(enemy.Bounds.X + 12, enemy.Bounds.Y + 4, 32, 14),
            Direction.Left => new Rectangle(enemy.Bounds.X + 34, enemy.Bounds.Y + 12, 18, 30),
            Direction.Right => new Rectangle(enemy.Bounds.X + 4, enemy.Bounds.Y + 12, 18, 30),
            _ => enemy.Bounds
        };
    }

    private static IEnumerable<Rectangle> GetChargeTelegraphSegments(EnemyActor enemy)
    {
        var origin = enemy.AttackOrigin;

        return enemy.DashDirection switch
        {
            Direction.Up =>
            [
                new Rectangle((int)origin.X - 6, (int)origin.Y - 16, 12, 12),
                new Rectangle((int)origin.X - 12, (int)origin.Y - 32, 24, 8)
            ],
            Direction.Down =>
            [
                new Rectangle((int)origin.X - 6, (int)origin.Y + 4, 12, 12),
                new Rectangle((int)origin.X - 12, (int)origin.Y + 20, 24, 8)
            ],
            Direction.Left =>
            [
                new Rectangle((int)origin.X - 16, (int)origin.Y - 6, 12, 12),
                new Rectangle((int)origin.X - 32, (int)origin.Y - 12, 8, 24)
            ],
            _ =>
            [
                new Rectangle((int)origin.X + 4, (int)origin.Y - 6, 12, 12),
                new Rectangle((int)origin.X + 20, (int)origin.Y - 12, 8, 24)
            ]
        };
    }

}
