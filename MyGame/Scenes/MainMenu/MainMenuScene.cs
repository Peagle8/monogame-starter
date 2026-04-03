using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Core;
using MyGame.Core.Assets;
using MyGame.Core.Input;
using MyGame.Core.Rendering;
using MyGame.Core.Scenes;

namespace MyGame.Scenes.MainMenu;

public sealed class MainMenuScene : IScene
{
    private readonly IInputService _inputService;
    private readonly IRenderer<MainMenuScene> _sceneRenderer;
    private readonly IRenderContext _renderContext;
    private readonly List<MenuItem> _items;
    private int _selectedIndex;

    public MainMenuScene(
        IInputService inputService,
        IRenderer<MainMenuScene> sceneRenderer,
        IRenderContext renderContext,
        Action onStartGame,
        Action onExitGame)
    {
        _inputService = inputService;
        _sceneRenderer = sceneRenderer;
        _renderContext = renderContext;
        _items =
        [
            new MenuItem("Start Game", onStartGame),
            new MenuItem("Exit", onExitGame)
        ];
    }

    public string Name => "Main Menu";

    public string Title => "MonoGame Starter";

    public IReadOnlyList<MenuItem> Items => _items;

    public int SelectedIndex => _selectedIndex;

    public void Enter()
    {
        _selectedIndex = 0;
    }

    public void Exit()
    {
    }

    public void Update(FrameTime frameTime)
    {
        if (_inputService.IsJustPressed(GameAction.MoveDown))
        {
            _selectedIndex = (_selectedIndex + 1) % _items.Count;
        }

        if (_inputService.IsJustPressed(GameAction.MoveUp))
        {
            _selectedIndex = (_selectedIndex - 1 + _items.Count) % _items.Count;
        }

        if (_inputService.IsJustPressed(GameAction.Confirm))
        {
            _items[_selectedIndex].OnSelected();
        }
    }

    public void Draw(FrameTime frameTime, SpriteBatch spriteBatch, IAssetCatalog assetCatalog)
    {
        var viewport = spriteBatch.GraphicsDevice.Viewport;
        _renderContext.Bind(
            spriteBatch,
            assetCatalog,
            RenderCamera.CreateIdentity(new Point(viewport.Width, viewport.Height)));
        spriteBatch.Begin();
        _sceneRenderer.Draw(this, frameTime);
        spriteBatch.End();
    }

    public IReadOnlyDictionary<string, string> GetDebugState()
    {
        return new Dictionary<string, string>
        {
            ["SelectedMenuIndex"] = _selectedIndex.ToString(),
            ["SelectedMenuText"] = _items[_selectedIndex].Text
        };
    }
}
