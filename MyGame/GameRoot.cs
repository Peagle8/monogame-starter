using Microsoft.Extensions.DependencyInjection;
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
using MyGame.Infrastructure.DependencyInjection;
using MyGame.Infrastructure.Logging;
using MyGame.Infrastructure.Save;
using MyGame.Rendering.Gameplay;
using MyGame.Rendering.MainMenu;
using MyGame.Scenes.Gameplay;
using MyGame.Scenes.MainMenu;

namespace MyGame;

public sealed class GameRoot : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private readonly ServiceProvider _serviceProvider;

    private SpriteBatch? _spriteBatch;
    private FrameTime _frameTime = FrameTime.Zero;
    private SceneManager? _sceneManager;
    private IInputService? _inputService;
    private IAssetCatalog? _assetCatalog;
    private DebugOverlay? _debugOverlay;
    private GameRecorder? _gameRecorder;

    public GameRoot()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _serviceProvider = ServiceRegistration.Build(this);
    }

    protected override void Initialize()
    {
        _inputService = _serviceProvider.GetRequiredService<IInputService>();
        _sceneManager = _serviceProvider.GetRequiredService<SceneManager>();
        _debugOverlay = _serviceProvider.GetRequiredService<DebugOverlay>();
        _gameRecorder = _serviceProvider.GetRequiredService<GameRecorder>();
        _sceneManager.ChangeScene(CreateMainMenuScene());

        base.Initialize();
    }

    private MainMenuScene CreateMainMenuScene()
    {
        return new MainMenuScene(
            _inputService!,
            _serviceProvider.GetServices<IRenderer<MainMenuScene>>().OfType<MainMenuSceneRenderer>().Single(),
            _serviceProvider.GetRequiredService<IRenderContext>(),
            onStartGame: () => _sceneManager!.ChangeScene(CreateGameplayScene()),
            onLoadGame: LoadGameplayFromSave,
            canLoadGame: () => _serviceProvider.GetRequiredService<ISaveGameService>().SaveExists(),
            onExitGame: Exit);
    }

    private GameplayScene CreateGameplayScene()
    {
        return new GameplayScene(
            _inputService!,
            _serviceProvider.GetRequiredService<World>(),
            _serviceProvider.GetServices<IRenderer<GameplayScene>>().OfType<GameplaySceneRenderer>().Single(),
            _serviceProvider.GetRequiredService<IRenderContext>(),
            _serviceProvider.GetRequiredService<ISaveGameService>(),
            _serviceProvider.GetRequiredService<GameRecorder>(),
            _serviceProvider.GetRequiredService<DiagnosticsSettings>(),
            onRestart: () => _sceneManager!.ChangeScene(CreateGameplayScene()),
            onReturnToMainMenu: () => _sceneManager!.ChangeScene(CreateMainMenuScene()));
    }

    private void LoadGameplayFromSave()
    {
        var saveGameService = _serviceProvider.GetRequiredService<ISaveGameService>();
        var saveData = saveGameService.Load();

        if (saveData is null || saveData.SceneName != "Gameplay")
        {
            return;
        }

        var scene = CreateGameplayScene();
        scene.World.ApplySaveData(saveData);
        _sceneManager!.ChangeScene(scene);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _assetCatalog = new AssetCatalog(Content, GraphicsDevice);

        _debugOverlay?.SetSpriteBatch(_spriteBatch);
        _debugOverlay?.SetFont(_assetCatalog.DebugFont);
        _debugOverlay?.SetPosition(Rendering.Gameplay.GameplayHudLayout.GetDebugOverlayPosition());
    }

    protected override void Update(GameTime gameTime)
    {
        if (_inputService is null || _sceneManager is null)
        {
            base.Update(gameTime);
            return;
        }

        _frameTime = FrameTime.From(gameTime);

        _inputService.Update();
        _sceneManager.Update(_frameTime);

        if (_gameRecorder?.IsRecording == true)
        {
            _gameRecorder.Capture(new RecordedFrame(
                _frameTime.Total,
                _sceneManager.CurrentSceneName,
                _inputService.Current,
                _sceneManager.GetDebugState()));
        }

        _debugOverlay?.SetValue("Scene", _sceneManager.CurrentSceneName);
        _debugOverlay?.SetValue("Elapsed", _frameTime.TotalSeconds.ToString("0.000"));
        _debugOverlay?.SetValue("FrameInput", _inputService.Current.ToSummary());
        _debugOverlay?.SetValue("Recorder", _gameRecorder is null
            ? "Unavailable"
            : _gameRecorder.IsRecording
                ? "Recording"
                : _gameRecorder.IsReplayPaused
                    ? "Replay Paused"
                    : _gameRecorder.IsReplaying
                        ? "Replay"
                        : "Idle");

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        if (_spriteBatch is null || _sceneManager is null)
        {
            base.Draw(gameTime);
            return;
        }

        GraphicsDevice.Clear(Color.Black);

        _sceneManager.Draw(_frameTime, _spriteBatch, _assetCatalog!);
        _debugOverlay?.Draw();

        base.Draw(gameTime);
    }
}
