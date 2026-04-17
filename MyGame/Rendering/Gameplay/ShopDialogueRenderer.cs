using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Shops;
using MyGame.Scenes.Gameplay;

namespace MyGame.Rendering.Gameplay;

public sealed class ShopDialogueRenderer : IRenderer<GameplayScene>
{
    private static readonly Color OverlayColor = new(8, 10, 14, 170);
    private static readonly Color ModalFillColor = new(243, 237, 226, 245);
    private static readonly Color ModalBorderColor = new(23, 23, 27);
    private static readonly Color HeaderColor = new(29, 35, 42);
    private static readonly Color PromptFillColor = new(251, 247, 239, 238);
    private static readonly Color ActiveTabFillColor = new(53, 81, 112);
    private static readonly Color InactiveTabFillColor = new(201, 194, 182);
    private static readonly Color ActiveTabTextColor = Color.White;
    private static readonly Color InactiveTabTextColor = new(54, 50, 47);

    private readonly IRenderContext _renderContext;

    public ShopDialogueRenderer(IRenderContext renderContext)
    {
        _renderContext = renderContext;
    }

    public void Draw(GameplayScene model, FrameTime frameTime)
    {
        var font = _renderContext.Assets.DebugFont;
        if (font is null)
        {
            return;
        }

        var viewport = _renderContext.SpriteBatch.GraphicsDevice.Viewport;
        var viewportBounds = viewport.Bounds;
        var viewportSize = new Point(viewport.Width, viewport.Height);

        if (model.ShopDialogue.IsPromptVisible && !model.ShopDialogue.IsOpen)
        {
            DrawPrompt(font, viewportSize);
        }

        if (!model.ShopDialogue.IsOpen)
        {
            return;
        }

        var modalBounds = ShopDialogueLayout.GetModalBounds(viewportSize);
        var tabBounds = ShopDialogueLayout.GetTabBounds(modalBounds);
        var contentBounds = ShopDialogueLayout.GetContentBounds(modalBounds);

        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, viewportBounds, OverlayColor);
        DrawPanel(modalBounds, ModalFillColor, ModalBorderColor);
        DrawHeader(font, modalBounds);
        DrawTabs(font, model.ShopDialogue.ActiveTab, tabBounds);
        DrawBody(font, model.ShopDialogue.ActiveTab, contentBounds);
        DrawHints(font, modalBounds);
    }

    private void DrawPrompt(Microsoft.Xna.Framework.Graphics.SpriteFont font, Point viewportSize)
    {
        var promptBounds = ShopDialogueLayout.GetPromptBounds(viewportSize);
        DrawPanel(promptBounds, PromptFillColor, ModalBorderColor);
        DrawCenteredText(font, "Buy / Sell   B or E", promptBounds, HeaderColor);
    }

    private void DrawHeader(Microsoft.Xna.Framework.Graphics.SpriteFont font, Rectangle modalBounds)
    {
        _renderContext.SpriteBatch.DrawString(
            font,
            "Shopkeeper",
            new Vector2(modalBounds.X + 32, modalBounds.Y + 28),
            HeaderColor);

        _renderContext.SpriteBatch.DrawString(
            font,
            "Choose a counter service.",
            new Vector2(modalBounds.X + 32, modalBounds.Y + 62),
            new Color(82, 77, 72));
    }

    private void DrawTabs(Microsoft.Xna.Framework.Graphics.SpriteFont font, ShopDialogueTab activeTab, Rectangle tabBounds)
    {
        var buyBounds = new Rectangle(tabBounds.X, tabBounds.Y, tabBounds.Width / 2, tabBounds.Height);
        var sellBounds = new Rectangle(buyBounds.Right, tabBounds.Y, tabBounds.Width / 2, tabBounds.Height);

        DrawTab(font, buyBounds, "Buy", activeTab == ShopDialogueTab.Buy);
        DrawTab(font, sellBounds, "Sell", activeTab == ShopDialogueTab.Sell);
    }

    private void DrawBody(Microsoft.Xna.Framework.Graphics.SpriteFont font, ShopDialogueTab activeTab, Rectangle contentBounds)
    {
        DrawPanel(contentBounds, new Color(255, 252, 246), new Color(57, 53, 51));

        var heading = activeTab == ShopDialogueTab.Buy ? "Buy Menu" : "Sell Menu";
        var bodyLines = activeTab == ShopDialogueTab.Buy
            ? new[] { "No wares yet.", "Next step: hook this up to shop stock and player inventory." }
            : new[] { "Nothing to sell yet.", "Next step: add player inventory and sellable item rules." };

        _renderContext.SpriteBatch.DrawString(
            font,
            heading,
            new Vector2(contentBounds.X + 18, contentBounds.Y + 18),
            HeaderColor);

        for (var index = 0; index < bodyLines.Length; index++)
        {
            _renderContext.SpriteBatch.DrawString(
                font,
                bodyLines[index],
                new Vector2(contentBounds.X + 18, contentBounds.Y + 56 + (index * 28)),
                new Color(68, 66, 62));
        }
    }

    private void DrawHints(Microsoft.Xna.Framework.Graphics.SpriteFont font, Rectangle modalBounds)
    {
        _renderContext.SpriteBatch.DrawString(
            font,
            "LB / RB or Q / R to switch tabs",
            new Vector2(modalBounds.X + 32, modalBounds.Bottom - 56),
            new Color(82, 77, 72));

        _renderContext.SpriteBatch.DrawString(
            font,
            "B / Esc to close",
            new Vector2(modalBounds.X + 32, modalBounds.Bottom - 30),
            new Color(82, 77, 72));
    }

    private void DrawTab(Microsoft.Xna.Framework.Graphics.SpriteFont font, Rectangle bounds, string label, bool isActive)
    {
        DrawPanel(bounds, isActive ? ActiveTabFillColor : InactiveTabFillColor, ModalBorderColor);
        DrawCenteredText(font, label, bounds, isActive ? ActiveTabTextColor : InactiveTabTextColor);
    }

    private void DrawCenteredText(Microsoft.Xna.Framework.Graphics.SpriteFont font, string text, Rectangle bounds, Color color)
    {
        var textSize = font.MeasureString(text);
        var position = new Vector2(
            bounds.X + ((bounds.Width - textSize.X) * 0.5f),
            bounds.Y + ((bounds.Height - textSize.Y) * 0.5f));
        _renderContext.SpriteBatch.DrawString(font, text, position, color);
    }

    private void DrawPanel(Rectangle bounds, Color fillColor, Color borderColor)
    {
        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, bounds, fillColor);
        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, new Rectangle(bounds.X, bounds.Y, bounds.Width, 2), borderColor);
        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, new Rectangle(bounds.X, bounds.Bottom - 2, bounds.Width, 2), borderColor);
        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, new Rectangle(bounds.X, bounds.Y, 2, bounds.Height), borderColor);
        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, new Rectangle(bounds.Right - 2, bounds.Y, 2, bounds.Height), borderColor);
    }
}
