using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Props;
using MyGame.Gameplay.World;

namespace MyGame.Rendering.Gameplay;

public sealed class ShopExteriorEntityRenderer : IGameplayEntityRenderer
{
    private static readonly Color WallColor = new(229, 205, 166);
    private static readonly Color RoofColor = new(120, 58, 42);
    private static readonly Color RoofShadeColor = new(86, 38, 29);
    private static readonly Color WindowColor = new(125, 196, 218);
    private static readonly Color DoorColor = new(91, 61, 41);
    private static readonly Color SignColor = new(201, 159, 87);
    private static readonly Color SignTextColor = new(58, 38, 20);

    private readonly IRenderContext _renderContext;
    private readonly IWorldRectangleRenderer _worldRectangleRenderer;

    public ShopExteriorEntityRenderer(IRenderContext renderContext, IWorldRectangleRenderer worldRectangleRenderer)
    {
        _renderContext = renderContext;
        _worldRectangleRenderer = worldRectangleRenderer;
    }

    public int DrawOrder => 42;

    public void Draw(World world, FrameTime frameTime)
    {
        foreach (var shop in world.GetProps<ShopExteriorProp>())
        {
            DrawShop(shop);
        }
    }

    private void DrawShop(ShopExteriorProp shop)
    {
        var roofBounds = new Rectangle(
            shop.Bounds.X + 8,
            shop.Bounds.Y,
            Math.Max(24, shop.Bounds.Width - 16),
            Math.Max(22, shop.Bounds.Height / 4));
        var roofShadeBounds = new Rectangle(
            roofBounds.X,
            roofBounds.Bottom - 8,
            roofBounds.Width,
            8);
        var wallBounds = new Rectangle(
            shop.Bounds.X + 16,
            roofBounds.Bottom - 4,
            Math.Max(24, shop.Bounds.Width - 32),
            Math.Max(24, shop.Bounds.Height - roofBounds.Height - 8));
        var leftWindowBounds = new Rectangle(
            wallBounds.X + 20,
            wallBounds.Y + 20,
            30,
            28);
        var rightWindowBounds = new Rectangle(
            wallBounds.Right - 50,
            wallBounds.Y + 20,
            30,
            28);
        var signBounds = new Rectangle(
            wallBounds.Center.X - 32,
            wallBounds.Y + 8,
            64,
            18);

        _worldRectangleRenderer.Draw(wallBounds, WallColor);
        _worldRectangleRenderer.Draw(roofBounds, RoofColor);
        _worldRectangleRenderer.Draw(roofShadeBounds, RoofShadeColor);
        _worldRectangleRenderer.Draw(leftWindowBounds, WindowColor);
        _worldRectangleRenderer.Draw(rightWindowBounds, WindowColor);
        _worldRectangleRenderer.Draw(signBounds, SignColor);
        _worldRectangleRenderer.Draw(shop.DoorBounds, DoorColor);
        DrawSignText(shop.SignText, signBounds);
    }

    private void DrawSignText(string text, Rectangle signBounds)
    {
        var font = _renderContext.Assets.DebugFont;
        if (font is null || string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        var screenBounds = _renderContext.Camera.WorldToScreen(signBounds);
        var scale = 0.42f;
        var textSize = font.MeasureString(text) * scale;
        var position = new Vector2(
            screenBounds.Center.X - (textSize.X / 2f),
            screenBounds.Center.Y - (textSize.Y / 2f) - 1f);
        _renderContext.SpriteBatch.DrawString(
            font,
            text,
            position,
            SignTextColor,
            0f,
            Vector2.Zero,
            scale,
            SpriteEffects.None,
            0f);
    }
}
