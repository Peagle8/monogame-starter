using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Configuration;
using MyGame.Core;
using MyGame.Core.Assets;
using MyGame.Core.Diagnostics;
using MyGame.Core.Input;
using MyGame.Core.Rendering;
using MyGame.Core.Scenes;
using MyGame.Gameplay.World;
using MyGame.Infrastructure.Save;
using MyGame.Rendering.Gameplay;

namespace MyGame.Scenes.Gameplay;

public sealed class GameplayScene : IScene
{
    private readonly Action _onRestart;
    private readonly Action _onReturnToMainMenu;
    private readonly IInputService _inputService;
    private readonly IRenderer<GameplayScene> _renderer;
    private readonly IRenderContext _renderContext;
    private readonly ISaveGameService _saveGameService;
    private readonly GameRecorder _gameRecorder;
    private readonly GameplayPauseMenu _pauseMenu;

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
    {
        _onRestart = onRestart;
        _onReturnToMainMenu = onReturnToMainMenu;
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

    public string Name => "Gameplay";

    public World World { get; }

    public GameplayPauseMenu PauseMenu => _pauseMenu;

    public bool IsPlayerDead => World.Player.IsDead;

    public bool IsRecording => _gameRecorder.IsRecording;

    public bool IsReplaying => _gameRecorder.IsReplaying;

    public bool IsReplayPaused => _gameRecorder.IsReplayPaused;

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

        World.Update(frameTime);
    }

    public void Draw(FrameTime frameTime, SpriteBatch spriteBatch, IAssetCatalog assetCatalog)
    {
        var viewport = spriteBatch.GraphicsDevice.Viewport;
        var camera = GameplayCamera.Create(
            World.Player.Position,
            new Point(viewport.Width, viewport.Height),
            new Point(World.Player.Bounds.Width, World.Player.Bounds.Height));
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
            ["ReplayMenuOpen"] = _pauseMenu.IsShowingReplayMenu.ToString(),
            ["PauseMenuFooterText"] = _pauseMenu.FooterText,
            ["RecorderRecording"] = _gameRecorder.IsRecording.ToString(),
            ["RecorderReplaying"] = _gameRecorder.IsReplaying.ToString(),
            ["RecorderReplayPaused"] = _gameRecorder.IsReplayPaused.ToString()
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
}
