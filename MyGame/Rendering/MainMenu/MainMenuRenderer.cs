using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Rendering.Menus;
using MyGame.Scenes.MainMenu;

namespace MyGame.Rendering.MainMenu;

public sealed class MainMenuRenderer : IRenderer<MainMenuScene>
{
    private const float FooterLineSpacing = 18f;
    private const float ItemSpacing = 40f;
    private const float ControlsLineSpacing = 22f;

    private readonly IRenderContext _renderContext;

    public MainMenuRenderer(IRenderContext renderContext)
    {
        _renderContext = renderContext;
    }

    public void Draw(MainMenuScene model, FrameTime frameTime)
    {
        if (_renderContext.Assets.DebugFont is null)
        {
            return;
        }

        var viewport = _renderContext.SpriteBatch.GraphicsDevice.Viewport;
        var viewportSize = new Point(viewport.Width, viewport.Height);

        _renderContext.SpriteBatch.DrawString(
            _renderContext.Assets.DebugFont,
            model.Title,
            MainMenuLayout.GetTitlePosition(viewportSize),
            Color.White);

        for (var index = 0; index < model.Items.Count; index++)
        {
            _renderContext.SpriteBatch.DrawString(
                _renderContext.Assets.DebugFont,
                model.Items[index].Text,
                VerticalMenuLayout.GetItemPosition(MainMenuLayout.GetItemsStartPosition(viewportSize), ItemSpacing, index),
                VerticalMenuLayout.GetItemColor(index, model.SelectedIndex, model.Items[index].IsEnabled));
        }

        DrawFooter(model.FooterText, viewportSize);

        if (model.IsShowingControls)
        {
            DrawControlsPanel(viewportSize);
        }
    }

    private void DrawControlsPanel(Point viewportSize)
    {
        var panelBounds = ControlsOverlayLayout.GetPanelBounds(viewportSize);

        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, panelBounds, Color.Black * 0.88f);
        _renderContext.SpriteBatch.DrawString(
            _renderContext.Assets.DebugFont!,
            "Controls",
            ControlsOverlayLayout.GetTitlePosition(viewportSize),
            new Color(255, 220, 196));

        for (var index = 0; index < ControlsOverlayText.Lines.Count; index++)
        {
            _renderContext.SpriteBatch.DrawString(
                _renderContext.Assets.DebugFont!,
                ControlsOverlayText.Lines[index],
                VerticalMenuLayout.GetItemPosition(
                    ControlsOverlayLayout.GetLinesStartPosition(viewportSize),
                    ControlsLineSpacing,
                    index),
                Color.White);
        }

        _renderContext.SpriteBatch.DrawString(
            _renderContext.Assets.DebugFont!,
            ControlsOverlayText.HintLineOne,
            ControlsOverlayLayout.GetHintLineOnePosition(viewportSize),
            new Color(170, 198, 190));

        _renderContext.SpriteBatch.DrawString(
            _renderContext.Assets.DebugFont!,
            ControlsOverlayText.HintLineTwo,
            ControlsOverlayLayout.GetHintLineTwoPosition(viewportSize),
            new Color(170, 198, 190));
    }

    private void DrawFooter(string text, Point viewportSize)
    {
        var font = _renderContext.Assets.DebugFont!;
        var lines = WrappedTextLayout.WrapText(font, text, MainMenuLayout.GetFooterWidth(viewportSize));
        var footerPosition = MainMenuLayout.GetFooterPosition(viewportSize);

        for (var index = 0; index < lines.Count; index++)
        {
            _renderContext.SpriteBatch.DrawString(
                font,
                lines[index],
                new Vector2(footerPosition.X, footerPosition.Y + (index * FooterLineSpacing)),
                new Color(154, 178, 171));
        }
    }
}
