using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Rendering.Menus;
using MyGame.Scenes.Gameplay;

namespace MyGame.Rendering.Gameplay;

public sealed class GameplayPauseMenuRenderer : IRenderer<GameplayPauseMenu>
{
    private static readonly Rectangle PanelBounds = new(240, 90, 320, 220);
    private static readonly Vector2 TitlePosition = new(340f, 120f);
    private static readonly Vector2 ItemsStartPosition = new(320f, 200f);
    private const float ItemSpacing = 36f;

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
        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, PanelBounds, Color.Black * 0.85f);
        _renderContext.SpriteBatch.DrawString(_renderContext.Assets.DebugFont, "Paused", TitlePosition, Color.White);

        for (var index = 0; index < model.Items.Count; index++)
        {
            _renderContext.SpriteBatch.DrawString(
                _renderContext.Assets.DebugFont,
                model.Items[index].Text,
                VerticalMenuLayout.GetItemPosition(ItemsStartPosition, ItemSpacing, index),
                VerticalMenuLayout.GetItemColor(index, model.SelectedIndex));
        }
    }
}
