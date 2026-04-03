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
        Action onLoadGame,
        Func<bool> canLoadGame,
        Action onExitGame)
    {
        _inputService = inputService;
        _sceneRenderer = sceneRenderer;
        _renderContext = renderContext;
        _items =
        [
            new MenuItem("Start Game", onStartGame),
            new MenuItem("Load Game", onLoadGame, canLoadGame),
            new MenuItem("Controls", OpenControls),
            new MenuItem("Exit", onExitGame)
        ];
    }

    public string Name => "Main Menu";

    public string Title => "MonoGame Starter";

    public IReadOnlyList<MenuItem> Items => _items;

    public int SelectedIndex => _selectedIndex;

    public bool IsShowingControls { get; private set; }

    public void Enter()
    {
        _selectedIndex = 0;
        IsShowingControls = false;
    }

    public void Exit()
    {
    }

    public void Update(FrameTime frameTime)
    {
        if (IsShowingControls)
        {
            if (_inputService.IsJustPressed(GameAction.Confirm)
                || _inputService.IsJustPressed(GameAction.Cancel)
                || _inputService.IsJustPressed(GameAction.Pause))
            {
                IsShowingControls = false;
            }

            return;
        }

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
            var selectedItem = _items[_selectedIndex];
            if (selectedItem.IsEnabled)
            {
                selectedItem.OnSelected();
            }
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
            ["ShowingControls"] = IsShowingControls.ToString(),
            ["SelectedMenuIndex"] = _selectedIndex.ToString(),
            ["SelectedMenuText"] = _items[_selectedIndex].Text
        };
    }

    private void OpenControls()
    {
        IsShowingControls = true;
    }
}
