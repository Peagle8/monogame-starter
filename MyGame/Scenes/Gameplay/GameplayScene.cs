using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Configuration;
using MyGame.Core;
using MyGame.Core.Assets;
using MyGame.Core.Diagnostics;
using MyGame.Core.Input;
using MyGame.Core.Rendering;
using MyGame.Core.Scenes;
using MyGame.Gameplay.Props;
using MyGame.Gameplay.Shops;
using MyGame.Gameplay.World;
using MyGame.Infrastructure.Save;
using MyGame.Rendering.Gameplay;

namespace MyGame.Scenes.Gameplay;

public sealed class GameplayScene : IScene
{
    private readonly string _name;
    private readonly Action _onRestart;
    private readonly Action _onReturnToMainMenu;
    private readonly Action<WorldSceneTransition> _onSceneTransition;
    private readonly IInputService _inputService;
    private readonly IRenderer<GameplayScene> _renderer;
    private readonly IRenderContext _renderContext;
    private readonly ISaveGameService _saveGameService;
    private readonly GameRecorder _gameRecorder;
    private readonly GameplayPauseMenu _pauseMenu;
    private readonly ShopDialogueController _shopDialogueController = new();
    private ShopDialogueState _shopDialogueState = ShopDialogueState.Default;

    public GameplayScene(
        IInputService inputService,
        World world,
        IRenderer<GameplayScene> renderer,
        IRenderContext renderContext,
        ISaveGameService saveGameService,
        GameRecorder gameRecorder,
        DiagnosticsSettings diagnosticsSettings,
        Action onRestart,
        Action onReturnToMainMenu)
        : this(
            GameplaySceneNames.Overworld,
            inputService,
            world,
            renderer,
            renderContext,
            saveGameService,
            gameRecorder,
            diagnosticsSettings,
            onRestart,
            onReturnToMainMenu,
            _ => { })
    {
    }

    public GameplayScene(
        string name,
        IInputService inputService,
        World world,
        IRenderer<GameplayScene> renderer,
        IRenderContext renderContext,
        ISaveGameService saveGameService,
        GameRecorder gameRecorder,
        DiagnosticsSettings diagnosticsSettings,
        Action onRestart,
        Action onReturnToMainMenu,
        Action<WorldSceneTransition> onSceneTransition)
    {
        _name = name;
        _onRestart = onRestart;
        _onReturnToMainMenu = onReturnToMainMenu;
        _onSceneTransition = onSceneTransition;
        _inputService = inputService;
        World = world;
        _renderer = renderer;
        _renderContext = renderContext;
        _saveGameService = saveGameService;
        _gameRecorder = gameRecorder;
        _pauseMenu = GameplayPauseMenu.CreateGameplayMenu(
            _saveGameService,
            _gameRecorder,
            diagnosticsSettings,
            Name,
            () => World.CreateSaveData(Name),
            World.ApplySaveData,
            _onRestart,
            ReturnToMainMenu);
    }

    public string Name => _name;

    public World World { get; }

    public GameplayPauseMenu PauseMenu => _pauseMenu;

    public bool IsPlayerDead => World.Player.IsDead;

    public bool IsRecording => _gameRecorder.IsRecording;

    public bool IsReplaying => _gameRecorder.IsReplaying;

    public bool IsReplayPaused => _gameRecorder.IsReplayPaused;

    public ShopDialogueState ShopDialogue => _shopDialogueState;

    public void Enter()
    {
    }

    public void Exit()
    {
    }

    public void Update(FrameTime frameTime)
    {
        if (IsPlayerDead)
        {
            HandleDeathInput();
            return;
        }

        if (_inputService.IsJustPressed(GameAction.Pause))
        {
            _gameRecorder.PauseReplay();
            _pauseMenu.Toggle();
        }

        if (_pauseMenu.IsOpen)
        {
            _pauseMenu.Update(_inputService);
            return;
        }

        if (_shopDialogueState.IsOpen)
        {
            UpdateShopDialogue();
            return;
        }

        World.Update(frameTime);
        UpdateShopDialogue();

        var pendingTransition = World.ConsumePendingSceneTransition();
        if (pendingTransition is not null)
        {
            _onSceneTransition(pendingTransition);
        }
    }

    public void Draw(FrameTime frameTime, SpriteBatch spriteBatch, IAssetCatalog assetCatalog)
    {
        var viewport = spriteBatch.GraphicsDevice.Viewport;
        var camera = GameplayCamera.Create(
            World.Player.Position,
            new Point(viewport.Width, viewport.Height),
            new Point(World.Player.Bounds.Width, World.Player.Bounds.Height),
            World.WorldBounds);
        _renderContext.Bind(spriteBatch, assetCatalog, camera);
        spriteBatch.Begin();
        _renderer.Draw(this, frameTime);
        spriteBatch.End();
    }

    public IReadOnlyDictionary<string, string> GetDebugState()
    {
        var debugState = new Dictionary<string, string>(World.GetDebugState())
        {
            ["PlayerDead"] = IsPlayerDead.ToString(),
            ["PauseMenuOpen"] = _pauseMenu.IsOpen.ToString(),
            ["PauseMenuSelection"] = _pauseMenu.SelectedText,
            ["PauseMenuInventoryOpen"] = _pauseMenu.IsShowingInventoryMenu.ToString(),
            ["PauseMenuInventoryTab"] = _pauseMenu.InventoryTab.ToString(),
            ["ReplayMenuOpen"] = _pauseMenu.IsShowingReplayMenu.ToString(),
            ["PauseMenuFooterText"] = _pauseMenu.FooterText,
            ["RecorderRecording"] = _gameRecorder.IsRecording.ToString(),
            ["RecorderReplaying"] = _gameRecorder.IsReplaying.ToString(),
            ["RecorderReplayPaused"] = _gameRecorder.IsReplayPaused.ToString(),
            ["ShopDialogueOpen"] = _shopDialogueState.IsOpen.ToString(),
            ["ShopDialoguePromptVisible"] = _shopDialogueState.IsPromptVisible.ToString(),
            ["ShopDialogueTab"] = _shopDialogueState.ActiveTab.ToString()
        };

        return debugState;
    }

    private void HandleDeathInput()
    {
        if (_inputService.IsJustPressed(GameAction.Confirm))
        {
            _gameRecorder.StopReplay();
            _onRestart();
            return;
        }

        if (_inputService.IsJustPressed(GameAction.Cancel) || _inputService.IsJustPressed(GameAction.Pause))
        {
            ReturnToMainMenu();
        }
    }

    private void ReturnToMainMenu()
    {
        _onReturnToMainMenu();
    }

    private void UpdateShopDialogue()
    {
        var counterBounds = World.GetProps<CounterProp>().FirstOrDefault()?.Bounds;
        _shopDialogueState = _shopDialogueController.Update(
            _shopDialogueState,
            World.Player.Bounds,
            counterBounds,
            _inputService.IsJustPressed(GameAction.Interact),
            _inputService.IsJustPressed(GameAction.Cancel),
            _inputService.IsJustPressed(GameAction.PreviousTab),
            _inputService.IsJustPressed(GameAction.NextTab));
    }
}
