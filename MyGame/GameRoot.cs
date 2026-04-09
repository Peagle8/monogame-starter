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
using MyGame.Gameplay.Player;
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
    private GameplayScene? _overworldScene;
    private GameplayScene? _shopInteriorScene;

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
            onStartGame: StartNewGame,
            onLoadGame: LoadGameplayFromSave,
            canLoadGame: () => _serviceProvider.GetRequiredService<ISaveGameService>().SaveExists(),
            onExitGame: Exit);
    }

    private void StartNewGame()
    {
        StartNewGame(GameplaySceneNames.Overworld);
    }

    private void StartNewGame(string initialSceneName)
    {
        CreateGameplayScenes();
        _sceneManager!.ChangeScene(GetGameplayScene(initialSceneName));
    }

    private void CreateGameplayScenes()
    {
        var builder = _serviceProvider.GetRequiredService<GameplayLevelBuilder>();
        var renderer = _serviceProvider.GetServices<IRenderer<GameplayScene>>().OfType<GameplaySceneRenderer>().Single();
        var renderContext = _serviceProvider.GetRequiredService<IRenderContext>();
        var saveGameService = _serviceProvider.GetRequiredService<ISaveGameService>();
        var gameRecorder = _serviceProvider.GetRequiredService<GameRecorder>();
        var diagnosticsSettings = _serviceProvider.GetRequiredService<DiagnosticsSettings>();

        _overworldScene = CreateGameplayScene(
            GameplaySceneNames.Overworld,
            builder.BuildOverworld(_serviceProvider.GetRequiredService<PlayerActor>()),
            renderer,
            renderContext,
            saveGameService,
            gameRecorder,
            diagnosticsSettings);
        // TODO: there is no way this should be here...  when we have lots of scenes imagine loading them all up in a nightmare giant sequence here
        _shopInteriorScene = CreateGameplayScene(
            GameplaySceneNames.ShopInterior,
            builder.BuildShopInterior(_serviceProvider.GetRequiredService<PlayerActor>()),
            renderer,
            renderContext,
            saveGameService,
            gameRecorder,
            diagnosticsSettings);
    }

    private GameplayScene CreateGameplayScene(
        string sceneName,
        World world,
        IRenderer<GameplayScene> renderer,
        IRenderContext renderContext,
        ISaveGameService saveGameService,
        GameRecorder gameRecorder,
        DiagnosticsSettings diagnosticsSettings)
    {
        return new GameplayScene(
            sceneName,
            _inputService!,
            world,
            renderer,
            renderContext,
            saveGameService,
            gameRecorder,
            diagnosticsSettings,
            onRestart: () => StartNewGame(sceneName),
            onReturnToMainMenu: () => _sceneManager!.ChangeScene(CreateMainMenuScene()),
            onSceneTransition: HandleGameplaySceneTransition);
    }

    private void HandleGameplaySceneTransition(WorldSceneTransition transition)
    {
        var sourceScene = (_sceneManager?.CurrentSceneName == GameplaySceneNames.ShopInterior)
            ? _shopInteriorScene
            : _overworldScene;
        var targetScene = GetGameplayScene(transition.TargetSceneName);
        var sourcePlayer = sourceScene!.World.Player;
        var transitionState = sourcePlayer.CreateTransitionState();

        targetScene.World.Player.ApplyTransitionState(transition.TargetPlayerPosition, transitionState);
        _sceneManager!.ChangeScene(targetScene);
    }

    private GameplayScene GetGameplayScene(string sceneName)
    {
        return sceneName switch
        {
            GameplaySceneNames.ShopInterior => _shopInteriorScene ?? throw new InvalidOperationException("Shop scene is unavailable."),
            GameplaySceneNames.Overworld => _overworldScene ?? throw new InvalidOperationException("Overworld scene is unavailable."),
            _ => throw new InvalidOperationException($"Unknown gameplay scene '{sceneName}'.")
        };
    }

    private bool LoadGameplayFromSave()
    {
        var saveGameService = _serviceProvider.GetRequiredService<ISaveGameService>();
        var saveData = saveGameService.Load();

        if (saveData is null)
        {
            return false;
        }

        if (saveData.SceneName != GameplaySceneNames.Overworld
            && saveData.SceneName != GameplaySceneNames.ShopInterior)
        {
            return false;
        }

        CreateGameplayScenes();
        var scene = GetGameplayScene(saveData.SceneName);
        scene.World.ApplySaveData(saveData);
        _sceneManager!.ChangeScene(scene);
        return true;
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
        _debugOverlay?.SetValue("Recorder", GetRecorderStatus(_gameRecorder));

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

    private static string GetRecorderStatus(GameRecorder? recorder)
    {
        return recorder switch
        {
            null => "Unavailable",
            { IsRecording: true } => "Recording",
            { IsReplayPaused: true } => "Replay Paused",
            { IsReplaying: true } => "Replay",
            _ => "Idle"
        };
    }
}
