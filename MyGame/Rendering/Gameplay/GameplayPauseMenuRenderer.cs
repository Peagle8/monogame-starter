using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Rendering.Menus;
using MyGame.Scenes.Gameplay;

namespace MyGame.Rendering.Gameplay;

public sealed class GameplayPauseMenuRenderer : IRenderer<GameplayPauseMenu>
{
    private static readonly Rectangle PanelBounds = new(240, 90, 320, 252);
    private static readonly Vector2 TitlePosition = new(340f, 120f);
    private static readonly Vector2 ItemsStartPosition = new(320f, 200f);
    private const float ItemSpacing = 36f;
    private const float ControlsLineSpacing = 24f;

    private readonly IRenderContext _renderContext;

    public GameplayPauseMenuRenderer(IRenderContext renderContext)
    {
        _renderContext = renderContext;
    }

    public void Draw(GameplayPauseMenu model, FrameTime frameTime)
    {
        if (!model.IsOpen || _renderContext.Assets.DebugFont is null)
        {
            return;
        }

        var viewportBounds = _renderContext.SpriteBatch.GraphicsDevice.Viewport.Bounds;
        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, viewportBounds, Color.Black * 0.55f);

        if (model.IsShowingControls)
        {
            DrawControlsPanel();
            return;
        }

        if (model.IsShowingReplayMenu)
        {
            DrawReplayPanel(model);
            return;
        }

        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, PanelBounds, Color.Black * 0.85f);
        _renderContext.SpriteBatch.DrawString(_renderContext.Assets.DebugFont, "Paused", TitlePosition, Color.White);

        for (var index = 0; index < model.Items.Count; index++)
        {
            _renderContext.SpriteBatch.DrawString(
                _renderContext.Assets.DebugFont,
                model.Items[index].Text,
                VerticalMenuLayout.GetItemPosition(ItemsStartPosition, ItemSpacing, index),
                VerticalMenuLayout.GetItemColor(index, model.SelectedIndex, model.Items[index].IsEnabled));
        }
    }

    private void DrawControlsPanel()
    {
        var viewport = _renderContext.SpriteBatch.GraphicsDevice.Viewport;
        var viewportSize = new Point(viewport.Width, viewport.Height);
        var panelBounds = ControlsOverlayLayout.GetPanelBounds(viewportSize);

        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, panelBounds, Color.Black * 0.9f);
        _renderContext.SpriteBatch.DrawString(
            _renderContext.Assets.DebugFont!,
            "Controls",
            ControlsOverlayLayout.GetTitlePosition(viewportSize),
            Color.White);

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

    private void DrawReplayPanel(GameplayPauseMenu model)
    {
        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, PanelBounds, Color.Black * 0.88f);
        _renderContext.SpriteBatch.DrawString(_renderContext.Assets.DebugFont!, "Replay", TitlePosition, Color.White);

        for (var index = 0; index < model.ReplayItems.Count; index++)
        {
            _renderContext.SpriteBatch.DrawString(
                _renderContext.Assets.DebugFont!,
                model.ReplayItems[index].Text,
                VerticalMenuLayout.GetItemPosition(ItemsStartPosition, ItemSpacing, index),
                VerticalMenuLayout.GetItemColor(index, model.ReplaySelectedIndex, model.ReplayItems[index].IsEnabled));
        }
    }
}
