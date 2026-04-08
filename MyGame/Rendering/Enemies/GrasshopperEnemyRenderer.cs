using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;

namespace MyGame.Rendering.Enemies;

public sealed class GrasshopperEnemyRenderer : IEnemyKindRenderer
{
    private static readonly Color DefeatedTint = new(104, 108, 78);
    private static readonly Color HitFlashTint = new(255, 240, 220);
    private static readonly Color BodyTint = new(126, 164, 54);
    private static readonly Color LegTint = new(92, 126, 36);
    private static readonly Color EyeTint = new(72, 40, 14);
    private static readonly Color LeapTrailTint = new(166, 208, 74, 120);

    private readonly IWorldRectangleRenderer _worldRectangleRenderer;

    public GrasshopperEnemyRenderer(IWorldRectangleRenderer worldRectangleRenderer)
    {
        _worldRectangleRenderer = worldRectangleRenderer;
    }

    public EnemyKind Kind => EnemyKind.Grasshopper;

    public void Draw(EnemyActor enemy, FrameTime frameTime)
    {
        var bodyColor = enemy.State == EnemyState.Dead
            ? DefeatedTint * Math.Max(enemy.DefeatedVisibilityAlpha, 0.35f)
            : BodyTint;
        var legColor = enemy.State == EnemyState.Dead
            ? DefeatedTint * Math.Max(enemy.DefeatedVisibilityAlpha, 0.45f)
            : LegTint;

        if (enemy.IsFlashingFromHit)
        {
            bodyColor = Color.Lerp(bodyColor, HitFlashTint, enemy.HitFlashAlpha);
            legColor = Color.Lerp(legColor, HitFlashTint, enemy.HitFlashAlpha * 0.8f);
        }

        if (enemy.State == EnemyState.Dashing)
        {
            _worldRectangleRenderer.Draw(GetLeapTrail(enemy), LeapTrailTint);
        }

        var thorax = new Rectangle(enemy.Bounds.X + 8, enemy.Bounds.Y + 8, 12, 10);
        var abdomen = new Rectangle(enemy.Bounds.X + 6, enemy.Bounds.Y + 15, 16, 8);
        var head = new Rectangle(enemy.Bounds.X + 10, enemy.Bounds.Y + 4, 8, 6);
        var rearLegLeft = new Rectangle(enemy.Bounds.X + 2, enemy.Bounds.Y + 14, 6, 12);
        var rearLegRight = new Rectangle(enemy.Bounds.X + 20, enemy.Bounds.Y + 14, 6, 12);
        var frontLegLeft = new Rectangle(enemy.Bounds.X + 4, enemy.Bounds.Y + 8, 4, 10);
        var frontLegRight = new Rectangle(enemy.Bounds.X + 20, enemy.Bounds.Y + 8, 4, 10);
        var leftEye = new Rectangle(enemy.Bounds.X + 11, enemy.Bounds.Y + 5, 2, 2);
        var rightEye = new Rectangle(enemy.Bounds.X + 15, enemy.Bounds.Y + 5, 2, 2);

        _worldRectangleRenderer.Draw(rearLegLeft, legColor);
        _worldRectangleRenderer.Draw(rearLegRight, legColor);
        _worldRectangleRenderer.Draw(frontLegLeft, legColor * 0.95f);
        _worldRectangleRenderer.Draw(frontLegRight, legColor * 0.95f);
        _worldRectangleRenderer.Draw(abdomen, bodyColor * 0.92f);
        _worldRectangleRenderer.Draw(thorax, bodyColor);
        _worldRectangleRenderer.Draw(head, bodyColor * 1.06f);
        _worldRectangleRenderer.Draw(leftEye, EyeTint);
        _worldRectangleRenderer.Draw(rightEye, EyeTint);
    }

    private static Rectangle GetLeapTrail(EnemyActor enemy)
    {
        return enemy.DashDirection switch
        {
            Direction.Up => new Rectangle(enemy.Bounds.X + 7, enemy.Bounds.Y + 18, 14, 10),
            Direction.Down => new Rectangle(enemy.Bounds.X + 7, enemy.Bounds.Y, 14, 10),
            Direction.Left => new Rectangle(enemy.Bounds.X + 16, enemy.Bounds.Y + 8, 12, 12),
            Direction.Right => new Rectangle(enemy.Bounds.X, enemy.Bounds.Y + 8, 12, 12),
            _ => enemy.Bounds
        };
    }
}
