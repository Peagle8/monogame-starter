using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Props;
using MyGame.Gameplay.World;

namespace MyGame.Rendering.Gameplay;

public sealed class ArenaEntranceEntityRenderer : IGameplayEntityRenderer
{
    private static readonly Color WallColor = new(150, 132, 104);
    private static readonly Color TrimColor = new(95, 82, 61);
    private static readonly Color RoofColor = new(112, 78, 74);
    private static readonly Color RoofShadeColor = new(77, 50, 47);
    private static readonly Color DoorColor = new(64, 44, 34);
    private static readonly Color BannerColor = new(176, 92, 56);
    private static readonly Color BannerTextColor = new(255, 239, 214);

    private readonly IRenderContext _renderContext;
    private readonly IWorldRectangleRenderer _worldRectangleRenderer;

    public ArenaEntranceEntityRenderer(IRenderContext renderContext, IWorldRectangleRenderer worldRectangleRenderer)
    {
        _renderContext = renderContext;
        _worldRectangleRenderer = worldRectangleRenderer;
    }

    public int DrawOrder => 42;

    public void Draw(World world, FrameTime frameTime)
    {
        foreach (var entrance in world.GetProps<ArenaEntranceProp>())
        {
            DrawEntrance(entrance);
        }
    }

    private void DrawEntrance(ArenaEntranceProp entrance)
    {
        var roofBounds = new Rectangle(
            entrance.Bounds.X + 6,
            entrance.Bounds.Y,
            Math.Max(36, entrance.Bounds.Width - 12),
            Math.Max(26, entrance.Bounds.Height / 3));
        var roofShadeBounds = new Rectangle(
            roofBounds.X,
            roofBounds.Bottom - 8,
            roofBounds.Width,
            8);
        var wallBounds = new Rectangle(
            entrance.Bounds.X + 14,
            roofBounds.Bottom - 4,
            Math.Max(32, entrance.Bounds.Width - 28),
            Math.Max(36, entrance.Bounds.Height - roofBounds.Height - 10));
        var leftTrim = new Rectangle(wallBounds.X, wallBounds.Y, 10, wallBounds.Height);
        var rightTrim = new Rectangle(wallBounds.Right - 10, wallBounds.Y, 10, wallBounds.Height);
        var bannerBounds = new Rectangle(
            wallBounds.Center.X - 18,
            wallBounds.Y + 10,
            36,
            20);

        _worldRectangleRenderer.Draw(wallBounds, WallColor);
        _worldRectangleRenderer.Draw(leftTrim, TrimColor);
        _worldRectangleRenderer.Draw(rightTrim, TrimColor);
        _worldRectangleRenderer.Draw(roofBounds, RoofColor);
        _worldRectangleRenderer.Draw(roofShadeBounds, RoofShadeColor);
        _worldRectangleRenderer.Draw(bannerBounds, BannerColor);
        _worldRectangleRenderer.Draw(entrance.DoorBounds, DoorColor);
        DrawBannerText(entrance.SignText, bannerBounds);
    }

    private void DrawBannerText(string text, Rectangle bannerBounds)
    {
        var font = _renderContext.Assets.DebugFont;
        if (font is null || string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        var screenBounds = _renderContext.Camera.WorldToScreen(bannerBounds);
        var scale = 0.45f;
        var textSize = font.MeasureString(text) * scale;
        var position = new Vector2(
            screenBounds.Center.X - (textSize.X / 2f),
            screenBounds.Center.Y - (textSize.Y / 2f));
        _renderContext.SpriteBatch.DrawString(
            font,
            text,
            position,
            BannerTextColor,
            0f,
            Vector2.Zero,
            scale,
            SpriteEffects.None,
            0f);
    }
}
