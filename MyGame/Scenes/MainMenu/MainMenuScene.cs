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
    private readonly Func<bool> _onLoadGame;
    private int _selectedIndex;

    public MainMenuScene(
        IInputService inputService,
        IRenderer<MainMenuScene> sceneRenderer,
        IRenderContext renderContext,
        Action onStartGame,
        Func<bool> onLoadGame,
        Func<bool> canLoadGame,
        Action onExitGame)
    {
        _inputService = inputService;
        _sceneRenderer = sceneRenderer;
        _renderContext = renderContext;
        _onLoadGame = onLoadGame;
        _items =
        [
            new MenuItem("Start Game", onStartGame),
            new MenuItem("Load Game", HandleLoadGameSelected, canLoadGame),
            new MenuItem("Controls", OpenControls),
            new MenuItem("Exit", onExitGame)
        ];
    }

    public string Name => "Main Menu";

    public string Title => "MonoGame Starter";

    public IReadOnlyList<MenuItem> Items => _items;

    public int SelectedIndex => _selectedIndex;

    public bool IsShowingControls { get; private set; }

    public string? StatusMessage { get; private set; }

    public string FooterText => StatusMessage ?? GetSelectionHint();

    public void Enter()
    {
        _selectedIndex = 0;
        IsShowingControls = false;
        StatusMessage = null;
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
            else if (selectedItem.Text == "Load Game")
            {
                StatusMessage = "No save yet. Start a run and save from the pause menu.";
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
            ["SelectedMenuText"] = _items[_selectedIndex].Text,
            ["FooterText"] = FooterText
        };
    }

    private void OpenControls()
    {
        IsShowingControls = true;
    }

    private void HandleLoadGameSelected()
    {
        StatusMessage = _onLoadGame()
            ? null
            : "That save could not be loaded.";
    }

    private string GetSelectionHint()
    {
        var selectedItem = _items[_selectedIndex];

        return selectedItem.Text switch
        {
            "Start Game" => "Start a new run from the beginning.",
            "Load Game" when !selectedItem.IsEnabled => "No save yet. Start a run and save from the pause menu.",
            "Load Game" => "Continue from your most recent save.",
            "Controls" => "Review the current keyboard controls.",
            "Exit" => "Close the game.",
            _ => "Press Enter to select."
        };
    }
}
