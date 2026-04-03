using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Scenes.MainMenu;
using MyGame.Rendering.Menus;

namespace MyGame.Rendering.MainMenu;

public sealed class MainMenuRenderer : IRenderer<MainMenuScene>
{
    private static readonly Vector2 TitlePosition = new(260f, 120f);
    private static readonly Vector2 ItemsStartPosition = new(300f, 220f);
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

        _renderContext.SpriteBatch.DrawString(_renderContext.Assets.DebugFont, model.Title, TitlePosition, Color.White);

        for (var index = 0; index < model.Items.Count; index++)
        {
            _renderContext.SpriteBatch.DrawString(
                _renderContext.Assets.DebugFont,
                model.Items[index].Text,
                VerticalMenuLayout.GetItemPosition(ItemsStartPosition, ItemSpacing, index),
                VerticalMenuLayout.GetItemColor(index, model.SelectedIndex, model.Items[index].IsEnabled));
        }

        if (model.IsShowingControls)
        {
            DrawControlsPanel();
        }
    }

    private void DrawControlsPanel()
    {
        var viewport = _renderContext.SpriteBatch.GraphicsDevice.Viewport;
        var viewportSize = new Point(viewport.Width, viewport.Height);
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
}
