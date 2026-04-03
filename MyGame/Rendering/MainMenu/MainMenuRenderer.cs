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
                VerticalMenuLayout.GetItemColor(index, model.SelectedIndex));
        }
    }
}
