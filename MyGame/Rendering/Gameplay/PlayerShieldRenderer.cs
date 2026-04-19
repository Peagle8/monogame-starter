using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.World;

namespace MyGame.Rendering.Gameplay;

public sealed class PlayerShieldRenderer : IGameplayEntityRenderer
{
    private const float FireShieldAccentRadiusPadding = 18f;
    private static readonly Color ShieldColor = new(112, 224, 255, 180);
    private static readonly Color FireShieldOuterColor = new(228, 66, 42, 170);
    private static readonly Color FireShieldInnerColor = new(255, 150, 54, 190);
    private static readonly Color FireShieldSparkColor = new(255, 214, 112, 205);
    private static readonly Color FireShieldAccentColor = new(255, 232, 160, 215);
    private static readonly Color FireShieldAccentCoreColor = new(255, 248, 214, 230);

    private readonly IWorldRectangleRenderer _worldRectangleRenderer;
    private readonly PlayerDefenseAbilitySettings _settings;

    public PlayerShieldRenderer(IWorldRectangleRenderer worldRectangleRenderer, PlayerDefenseAbilitySettings settings)
    {
        _worldRectangleRenderer = worldRectangleRenderer;
        _settings = settings;
    }

    public int DrawOrder => 99;

    public void Draw(World world, FrameTime frameTime)
    {
        if (world.Player.IsFireShieldActive)
        {
            DrawFireShield(world.Player.Bounds, frameTime);
            return;
        }

        if (!world.Player.IsShieldActive)
        {
            return;
        }

        foreach (var segment in CreateShieldSegments(world.Player.Bounds))
        {
            _worldRectangleRenderer.Draw(segment, ShieldColor);
        }
    }

    private void DrawFireShield(Rectangle playerBounds, FrameTime frameTime)
    {
        var pulseRadius = MathF.Sin(frameTime.TotalSeconds * 4.2f) * _settings.FireShieldPulseAmplitude;
        var pulseBounds = PlayerFireShieldArea.GetVisualBounds(
            playerBounds,
            _settings.FireShieldRadiusMultiplier,
            _settings.FireShieldPulseAmplitude + FireShieldAccentRadiusPadding);
        if (!_worldRectangleRenderer.IsVisible(pulseBounds))
        {
            return;
        }

        var center = PlayerFireShieldArea.GetCenter(playerBounds);
        var outerRadius = PlayerFireShieldArea.GetRadius(playerBounds, _settings.FireShieldRadiusMultiplier) + pulseRadius;
        var innerRadius = Math.Max(outerRadius - _settings.FireShieldRingThickness, 1f);

        foreach (var segment in CreateFireShieldSegments(center, outerRadius, innerRadius, frameTime.TotalSeconds))
        {
            _worldRectangleRenderer.Draw(segment.Bounds, segment.Color);
        }
    }

    private static IReadOnlyList<Rectangle> CreateShieldSegments(Rectangle playerBounds)
    {
        var shieldBounds = new Rectangle(
            playerBounds.X - 6,
            playerBounds.Y - 6,
            playerBounds.Width + 12,
            playerBounds.Height + 12);
        const int thickness = 3;

        return
        [
            new Rectangle(shieldBounds.X + 8, shieldBounds.Y, shieldBounds.Width - 16, thickness),
            new Rectangle(shieldBounds.X + 8, shieldBounds.Bottom - thickness, shieldBounds.Width - 16, thickness),
            new Rectangle(shieldBounds.X, shieldBounds.Y + 8, thickness, shieldBounds.Height - 16),
            new Rectangle(shieldBounds.Right - thickness, shieldBounds.Y + 8, thickness, shieldBounds.Height - 16),
            new Rectangle(shieldBounds.X + 2, shieldBounds.Y + 2, 6, 6),
            new Rectangle(shieldBounds.Right - 8, shieldBounds.Y + 2, 6, 6),
            new Rectangle(shieldBounds.X + 2, shieldBounds.Bottom - 8, 6, 6),
            new Rectangle(shieldBounds.Right - 8, shieldBounds.Bottom - 8, 6, 6)
        ];
    }

    private IReadOnlyList<FireShieldSegment> CreateFireShieldSegments(Vector2 center, float outerRadius, float innerRadius, float totalSeconds)
    {
        const int segmentCount = 48;
        const float angleOffset = 0.1308997f; // 7.5 degrees
        var segments = new List<FireShieldSegment>(segmentCount * 4);

        for (var index = 0; index < segmentCount; index++)
        {
            var angle = ((MathF.Tau * index) / segmentCount) + (totalSeconds * 0.4f);
            var outerBounds = CreateSegmentBounds(center, outerRadius, angle, 14, 10);
            var innerBounds = CreateSegmentBounds(center, innerRadius, angle + angleOffset, 10, 8);
            var sparkRadius = Math.Max(innerRadius - (_settings.FireShieldRingThickness * 0.45f), 1f);
            var sparkBounds = CreateSegmentBounds(center, sparkRadius, angle - angleOffset, 6, 6);
            var flameLift = 6f + MathF.Max(0f, MathF.Sin((totalSeconds * 6.6f) + (index * 0.55f)) * 5f);
            var flameHeight = 8 + (int)MathF.Round(flameLift);
            var accentBounds = CreateSegmentBounds(center, outerRadius + flameLift, angle, 8, flameHeight);
            var accentCoreBounds = CreateSegmentBounds(center, outerRadius + (flameLift * 0.5f), angle, 4, Math.Max(6, flameHeight - 6));

            segments.Add(new FireShieldSegment(outerBounds, FireShieldOuterColor));
            segments.Add(new FireShieldSegment(innerBounds, FireShieldInnerColor));

            if (index % 3 == 0)
            {
                segments.Add(new FireShieldSegment(sparkBounds, FireShieldSparkColor));
            }

            if (index % 4 == 0)
            {
                segments.Add(new FireShieldSegment(accentBounds, FireShieldAccentColor));
            }

            if (index % 8 == 0)
            {
                segments.Add(new FireShieldSegment(accentCoreBounds, FireShieldAccentCoreColor));
            }
        }

        return segments;
    }

    private static Rectangle CreateSegmentBounds(Vector2 center, float radius, float angle, int width, int height)
    {
        var position = center + new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * radius;
        return new Rectangle(
            (int)MathF.Round(position.X - (width / 2f)),
            (int)MathF.Round(position.Y - (height / 2f)),
            width,
            height);
    }

    private readonly record struct FireShieldSegment(Rectangle Bounds, Color Color);
}
