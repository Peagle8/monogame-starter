using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Scenes.MainMenu;

namespace MyGame.Rendering.MainMenu;

public sealed class MainMenuSceneRenderer : IRenderer<MainMenuScene>
{
    private readonly MainMenuBackgroundRenderer _backgroundRenderer;
    private readonly MainMenuRenderer _menuRenderer;

    public MainMenuSceneRenderer(MainMenuBackgroundRenderer backgroundRenderer, MainMenuRenderer menuRenderer)
    {
        _backgroundRenderer = backgroundRenderer;
        _menuRenderer = menuRenderer;
    }

    public void Draw(MainMenuScene model, FrameTime frameTime)
    {
        _backgroundRenderer.Draw(model, frameTime);
        _menuRenderer.Draw(model, frameTime);
    }
}
