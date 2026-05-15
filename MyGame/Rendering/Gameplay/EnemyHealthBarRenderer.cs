using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.World;

namespace MyGame.Rendering.Gameplay;

public sealed class EnemyHealthBarRenderer : IGameplayEntityRenderer
{
    private static readonly Color FrameColor = new(16, 14, 18);
    private static readonly Color BackgroundColor = new(73, 50, 52);
    private static readonly Color FillColor = new(211, 78, 68);
    private static readonly Color BossFillColor = new(230, 112, 82);
    private static readonly Color AbilityFrameColor = new(12, 18, 30);
    private static readonly Color AbilityBackgroundColor = new(36, 54, 82);
    private static readonly Color AbilityFillColor = new(110, 196, 255);

    private readonly IWorldRectangleRenderer _worldRectangleRenderer;

    public EnemyHealthBarRenderer(IWorldRectangleRenderer worldRectangleRenderer)
    {
        _worldRectangleRenderer = worldRectangleRenderer;
    }

    public int DrawOrder => 95;

    public void Draw(World world, FrameTime frameTime)
    {
        foreach (var enemy in world.Enemies)
        {
            if (!enemy.IsRenderable || enemy.HealthBarAlpha <= 0f)
            {
                continue;
            }

            DrawHealthBar(enemy);
        }
    }

    private void DrawHealthBar(EnemyActor enemy)
    {
        DrawAbilityBar(enemy);

        var frameBounds = EnemyHealthBarLayout.GetFrameBounds(enemy);
        var backgroundBounds = EnemyHealthBarLayout.GetBackgroundBounds(frameBounds);
        var fillBounds = EnemyHealthBarLayout.GetFillBounds(frameBounds, enemy.CurrentHealth, enemy.MaxHealth);
        var alpha = enemy.HealthBarAlpha;
        var fillColor = IsBoss(enemy.Kind) ? BossFillColor : FillColor;

        _worldRectangleRenderer.Draw(frameBounds, FrameColor * alpha);

        if (!backgroundBounds.IsEmpty)
        {
            _worldRectangleRenderer.Draw(backgroundBounds, BackgroundColor * alpha);
        }

        if (!fillBounds.IsEmpty)
        {
            _worldRectangleRenderer.Draw(fillBounds, fillColor * alpha);
        }
    }

    private void DrawAbilityBar(EnemyActor enemy)
    {
        var frameBounds = EnemyHealthBarLayout.GetAbilityFrameBounds(enemy);
        if (frameBounds.IsEmpty)
        {
            return;
        }

        var backgroundBounds = EnemyHealthBarLayout.GetBackgroundBounds(frameBounds);
        var fillBounds = EnemyHealthBarLayout.GetFillBounds(frameBounds, enemy.CurrentAbilityPoints, enemy.MaxAbilityPoints);
        var alpha = enemy.HealthBarAlpha;

        _worldRectangleRenderer.Draw(frameBounds, AbilityFrameColor * alpha);

        if (!backgroundBounds.IsEmpty)
        {
            _worldRectangleRenderer.Draw(backgroundBounds, AbilityBackgroundColor * alpha);
        }

        if (!fillBounds.IsEmpty)
        {
            _worldRectangleRenderer.Draw(fillBounds, AbilityFillColor * alpha);
        }
    }

    private static bool IsBoss(EnemyKind kind)
    {
        return kind is EnemyKind.HornedRabbitBoss or EnemyKind.BatMiniBoss;
    }
}
