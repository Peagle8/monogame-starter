using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;

namespace MyGame.Rendering.Enemies;

public sealed class SkeletonEliteEnemyRenderer : IEnemyKindRenderer
{
    private static readonly Color DefeatedTint = new(92, 82, 78);
    private static readonly Color HitFlashTint = new(255, 246, 226);
    private static readonly Color BoneColor = new(164, 158, 152);
    private static readonly Color ShadowBoneColor = new(112, 108, 104);
    private static readonly Color BowColor = new(78, 58, 44);
    private static readonly Color EyeColor = new(224, 88, 68);
    private static readonly Color ShieldColor = new(118, 216, 255, 170);

    private readonly IWorldRectangleRenderer _worldRectangleRenderer;

    public SkeletonEliteEnemyRenderer(IWorldRectangleRenderer worldRectangleRenderer)
    {
        _worldRectangleRenderer = worldRectangleRenderer;
    }

    public EnemyKind Kind => EnemyKind.SkeletonElite;

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
        var eyeColor = enemy.IsFlashingFromHit
            ? Color.Lerp(EyeColor, HitFlashTint, enemy.HitFlashAlpha)
            : EyeColor;

        var skull = new Rectangle(enemy.Bounds.X + 9, enemy.Bounds.Y + 2, 18, 13);
        var leftEye = new Rectangle(enemy.Bounds.X + 13, enemy.Bounds.Y + 7, 3, 2);
        var rightEye = new Rectangle(enemy.Bounds.X + 20, enemy.Bounds.Y + 7, 3, 2);
        var spine = new Rectangle(enemy.Bounds.X + 16, enemy.Bounds.Y + 15, 4, 11);
        var ribs = new Rectangle(enemy.Bounds.X + 9, enemy.Bounds.Y + 15, 18, 8);
        var leftArm = new Rectangle(enemy.Bounds.X + 5, enemy.Bounds.Y + 14, 4, 12);
        var rightArm = new Rectangle(enemy.Bounds.X + 27, enemy.Bounds.Y + 14, 4, 12);
        var leftLeg = new Rectangle(enemy.Bounds.X + 12, enemy.Bounds.Y + 25, 4, 10);
        var rightLeg = new Rectangle(enemy.Bounds.X + 20, enemy.Bounds.Y + 25, 4, 10);
        var bow = GetBowBounds(enemy);

        _worldRectangleRenderer.Draw(skull, boneColor);
        _worldRectangleRenderer.Draw(leftEye, eyeColor);
        _worldRectangleRenderer.Draw(rightEye, eyeColor);
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
            Direction.Left => new Rectangle(enemy.Bounds.X + 1, enemy.Bounds.Y + 8, 5, 20),
            Direction.Right => new Rectangle(enemy.Bounds.Right - 6, enemy.Bounds.Y + 8, 5, 20),
            Direction.Up => new Rectangle(enemy.Bounds.X + 8, enemy.Bounds.Y + 1, 20, 5),
            Direction.Down => new Rectangle(enemy.Bounds.X + 8, enemy.Bounds.Bottom - 6, 20, 5),
            _ => new Rectangle(enemy.Bounds.Right - 6, enemy.Bounds.Y + 8, 5, 20)
        };
    }

    private static IReadOnlyList<Rectangle> CreateShieldSegments(Rectangle bounds)
    {
        var shieldBounds = new Rectangle(bounds.X - 6, bounds.Y - 6, bounds.Width + 12, bounds.Height + 12);

        return
        [
            new Rectangle(shieldBounds.X + 6, shieldBounds.Y, shieldBounds.Width - 12, 2),
            new Rectangle(shieldBounds.X + 6, shieldBounds.Bottom - 2, shieldBounds.Width - 12, 2),
            new Rectangle(shieldBounds.X, shieldBounds.Y + 6, 2, shieldBounds.Height - 12),
            new Rectangle(shieldBounds.Right - 2, shieldBounds.Y + 6, 2, shieldBounds.Height - 12)
        ];
    }
}
