using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Rendering.Menus;
using MyGame.Scenes.Gameplay;

namespace MyGame.Rendering.Gameplay;

public sealed class NpcDialogueRenderer : IRenderer<GameplayScene>
{
    private static readonly Color PanelFillColor = new(248, 243, 233, 246);
    private static readonly Color PanelBorderColor = new(29, 31, 34);
    private static readonly Color PromptFillColor = new(251, 247, 239, 238);
    private static readonly Color HeaderColor = new(29, 35, 42);
    private static readonly Color BodyColor = new(62, 60, 56);
    private static readonly Color HintColor = new(91, 84, 76);

    private readonly IRenderContext _renderContext;

    public NpcDialogueRenderer(IRenderContext renderContext)
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
        var viewportSize = new Point(viewport.Width, viewport.Height);

        if (model.NpcDialogue.IsPromptVisible && !model.NpcDialogue.IsOpen)
        {
            DrawPrompt(font, viewportSize);
        }

        if (model.NpcDialogue.IsOpen)
        {
            DrawDialogue(font, viewportSize, model.NpcDialogue.SpeakerName, model.NpcDialogue.Text);
        }
    }

    private void DrawPrompt(SpriteFont font, Point viewportSize)
    {
        var promptBounds = NpcDialogueLayout.GetPromptBounds(viewportSize);
        DrawPanel(promptBounds, PromptFillColor, PanelBorderColor);
        DrawCenteredText(font, "Talk   B or E", promptBounds, HeaderColor);
    }

    private void DrawDialogue(SpriteFont font, Point viewportSize, string speakerName, string text)
    {
        var panelBounds = NpcDialogueLayout.GetPanelBounds(viewportSize);
        var textBounds = NpcDialogueLayout.GetTextBounds(panelBounds);
        DrawPanel(panelBounds, PanelFillColor, PanelBorderColor);

        _renderContext.SpriteBatch.DrawString(
            font,
            speakerName,
            new Vector2(panelBounds.X + 28, panelBounds.Y + 24),
            HeaderColor);

        var lines = WrappedTextLayout.WrapText(font, text, textBounds.Width);
        for (var index = 0; index < lines.Count; index++)
        {
            _renderContext.SpriteBatch.DrawString(
                font,
                lines[index],
                new Vector2(textBounds.X, textBounds.Y + (index * 26)),
                BodyColor);
        }

        _renderContext.SpriteBatch.DrawString(
            font,
            "Enter / B / Esc to close",
            new Vector2(panelBounds.X + 28, panelBounds.Bottom - 32),
            HintColor);
    }

    private void DrawCenteredText(SpriteFont font, string text, Rectangle bounds, Color color)
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
