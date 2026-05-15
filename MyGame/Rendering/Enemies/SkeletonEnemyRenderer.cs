using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;

namespace MyGame.Rendering.Enemies;

public sealed class SkeletonEnemyRenderer : IEnemyKindRenderer
{
    private static readonly Color DefeatedTint = new(116, 100, 94);
    private static readonly Color HitFlashTint = new(255, 247, 224);
    private static readonly Color BoneColor = new(220, 214, 202);
    private static readonly Color ShadowBoneColor = new(178, 170, 156);
    private static readonly Color BowColor = new(122, 92, 70);
    private static readonly Color ShieldColor = new(118, 216, 255, 170);

    private readonly IWorldRectangleRenderer _worldRectangleRenderer;

    public SkeletonEnemyRenderer(IWorldRectangleRenderer worldRectangleRenderer)
    {
        _worldRectangleRenderer = worldRectangleRenderer;
    }

    public EnemyKind Kind => EnemyKind.Skeleton;

    public void Draw(EnemyActor enemy, FrameTime frameTime)
    {
        var boneColor = enemy.State == EnemyState.Dead
            ? DefeatedTint * Math.Max(enemy.DefeatedVisibilityAlpha, 0.35f)
            : BoneColor;
        if (enemy.IsFlashingFromHit)
        {
            boneColor = Color.Lerp(boneColor, HitFlashTint, enemy.HitFlashAlpha);
        }

        var shadowBoneColor = enemy.IsFlashingFromHit
            ? Color.Lerp(ShadowBoneColor, HitFlashTint, enemy.HitFlashAlpha)
            : ShadowBoneColor;
        var bowColor = enemy.IsFlashingFromHit
            ? Color.Lerp(BowColor, HitFlashTint, enemy.HitFlashAlpha)
            : BowColor;

        var skull = new Rectangle(enemy.Bounds.X + 9, enemy.Bounds.Y + 1, 14, 12);
        var spine = new Rectangle(enemy.Bounds.X + 14, enemy.Bounds.Y + 12, 4, 11);
        var ribs = new Rectangle(enemy.Bounds.X + 9, enemy.Bounds.Y + 12, 14, 7);
        var leftArm = new Rectangle(enemy.Bounds.X + 6, enemy.Bounds.Y + 12, 4, 10);
        var rightArm = new Rectangle(enemy.Bounds.X + 22, enemy.Bounds.Y + 12, 4, 10);
        var leftLeg = new Rectangle(enemy.Bounds.X + 11, enemy.Bounds.Y + 22, 4, 9);
        var rightLeg = new Rectangle(enemy.Bounds.X + 17, enemy.Bounds.Y + 22, 4, 9);
        var bow = GetBowBounds(enemy);

        _worldRectangleRenderer.Draw(skull, boneColor);
        _worldRectangleRenderer.Draw(spine, shadowBoneColor);
        _worldRectangleRenderer.Draw(ribs, boneColor);
        _worldRectangleRenderer.Draw(leftArm, shadowBoneColor);
        _worldRectangleRenderer.Draw(rightArm, shadowBoneColor);
        _worldRectangleRenderer.Draw(leftLeg, shadowBoneColor);
        _worldRectangleRenderer.Draw(rightLeg, shadowBoneColor);
        _worldRectangleRenderer.Draw(bow, bowColor);

        if (!enemy.IsShieldActive)
        {
            return;
        }

        foreach (var segment in CreateShieldSegments(enemy.Bounds))
        {
            _worldRectangleRenderer.Draw(segment, ShieldColor);
        }
    }

    private static Rectangle GetBowBounds(EnemyActor enemy)
    {
        return enemy.DashDirection switch
        {
            Direction.Left => new Rectangle(enemy.Bounds.X + 1, enemy.Bounds.Y + 8, 4, 16),
            Direction.Right => new Rectangle(enemy.Bounds.Right - 5, enemy.Bounds.Y + 8, 4, 16),
            Direction.Up => new Rectangle(enemy.Bounds.X + 8, enemy.Bounds.Y + 1, 16, 4),
            Direction.Down => new Rectangle(enemy.Bounds.X + 8, enemy.Bounds.Bottom - 5, 16, 4),
            _ => new Rectangle(enemy.Bounds.Right - 5, enemy.Bounds.Y + 8, 4, 16)
        };
    }

    private static IReadOnlyList<Rectangle> CreateShieldSegments(Rectangle bounds)
    {
        var shieldBounds = new Rectangle(bounds.X - 5, bounds.Y - 5, bounds.Width + 10, bounds.Height + 10);

        return
        [
            new Rectangle(shieldBounds.X + 6, shieldBounds.Y, shieldBounds.Width - 12, 2),
            new Rectangle(shieldBounds.X + 6, shieldBounds.Bottom - 2, shieldBounds.Width - 12, 2),
            new Rectangle(shieldBounds.X, shieldBounds.Y + 6, 2, shieldBounds.Height - 12),
            new Rectangle(shieldBounds.Right - 2, shieldBounds.Y + 6, 2, shieldBounds.Height - 12)
        ];
    }
}
