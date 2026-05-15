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
using MyGame.Gameplay.Narrative;
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
    private readonly Dictionary<string, GameplayScene> _gameplayScenes = [];

    public GameRoot()
    {
        _graphics = new GraphicsDeviceManager(this);
        ConfigureFullscreenPresentation();
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
        _gameplayScenes.Clear();
        var builder = _serviceProvider.GetRequiredService<GameplayLevelBuilder>();
        var renderer = _serviceProvider.GetServices<IRenderer<GameplayScene>>().OfType<GameplaySceneRenderer>().Single();
        var renderContext = _serviceProvider.GetRequiredService<IRenderContext>();
        var saveGameService = _serviceProvider.GetRequiredService<ISaveGameService>();
        var gameRecorder = _serviceProvider.GetRequiredService<GameRecorder>();
        var diagnosticsSettings = _serviceProvider.GetRequiredService<DiagnosticsSettings>();
        var npcDialogueService = _serviceProvider.GetRequiredService<NpcDialogueService>();
        var hintService = _serviceProvider.GetRequiredService<HintService>();
        var journalService = _serviceProvider.GetRequiredService<JournalService>();

        AddGameplayScene(CreateGameplayScene(
            GameplaySceneNames.Overworld,
            builder.BuildOverworld(_serviceProvider.GetRequiredService<PlayerActor>()),
            renderer,
            renderContext,
            saveGameService,
            gameRecorder,
            diagnosticsSettings,
            npcDialogueService,
            hintService,
            journalService));
        AddGameplayScene(CreateGameplayScene(
            GameplaySceneNames.WildernessNorth,
            builder.BuildWildernessNorth(_serviceProvider.GetRequiredService<PlayerActor>()),
            renderer,
            renderContext,
            saveGameService,
            gameRecorder,
            diagnosticsSettings,
            npcDialogueService,
            hintService,
            journalService));
        AddGameplayScene(CreateGameplayScene(
            GameplaySceneNames.WildernessSouth,
            builder.BuildWildernessSouth(_serviceProvider.GetRequiredService<PlayerActor>()),
            renderer,
            renderContext,
            saveGameService,
            gameRecorder,
            diagnosticsSettings,
            npcDialogueService,
            hintService,
            journalService));
        AddGameplayScene(CreateGameplayScene(
            GameplaySceneNames.WildernessEast,
            builder.BuildWildernessEast(_serviceProvider.GetRequiredService<PlayerActor>()),
            renderer,
            renderContext,
            saveGameService,
            gameRecorder,
            diagnosticsSettings,
            npcDialogueService,
            hintService,
            journalService));
        AddGameplayScene(CreateGameplayScene(
            GameplaySceneNames.WildernessWest,
            builder.BuildWildernessWest(_serviceProvider.GetRequiredService<PlayerActor>()),
            renderer,
            renderContext,
            saveGameService,
            gameRecorder,
            diagnosticsSettings,
            npcDialogueService,
            hintService,
            journalService));
        AddGameplayScene(CreateGameplayScene(
            GameplaySceneNames.ShopInterior,
            builder.BuildShopInterior(_serviceProvider.GetRequiredService<PlayerActor>()),
            renderer,
            renderContext,
            saveGameService,
            gameRecorder,
            diagnosticsSettings,
            npcDialogueService,
            hintService,
            journalService));
        AddGameplayScene(CreateGameplayScene(
            GameplaySceneNames.Arena,
            builder.BuildArena(_serviceProvider.GetRequiredService<PlayerActor>()),
            renderer,
            renderContext,
            saveGameService,
            gameRecorder,
            diagnosticsSettings,
            npcDialogueService,
            hintService,
            journalService));
    }

    private GameplayScene CreateGameplayScene(
        string sceneName,
        World world,
        IRenderer<GameplayScene> renderer,
        IRenderContext renderContext,
        ISaveGameService saveGameService,
        GameRecorder gameRecorder,
        DiagnosticsSettings diagnosticsSettings,
        NpcDialogueService npcDialogueService,
        HintService hintService,
        JournalService journalService)
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
            onSceneTransition: HandleGameplaySceneTransition,
            npcDialogueService: npcDialogueService,
            hintService: hintService,
            journalService: journalService);
    }

    private void AddGameplayScene(GameplayScene scene)
    {
        _gameplayScenes[scene.Name] = scene;
    }

    private void HandleGameplaySceneTransition(WorldSceneTransition transition)
    {
        var sourceScene = GetGameplayScene(_sceneManager?.CurrentSceneName ?? GameplaySceneNames.Overworld);
        var targetScene = GetGameplayScene(transition.TargetSceneName);
        var sourcePlayer = sourceScene!.World.Player;
        var transitionState = sourcePlayer.CreateTransitionState();
        var targetPosition = transition.ResolveTargetPlayerPosition(sourceScene.World);

        targetScene.World.Player.ApplyTransitionState(targetPosition, transitionState);
        targetScene.World.SuppressIntersectingSceneTransitions();
        _sceneManager!.ChangeScene(targetScene);
    }

    private GameplayScene GetGameplayScene(string sceneName)
    {
        return _gameplayScenes.TryGetValue(sceneName, out var scene)
            ? scene
            : throw new InvalidOperationException($"Unknown gameplay scene '{sceneName}'.");
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
            && saveData.SceneName != GameplaySceneNames.ShopInterior
            && saveData.SceneName != GameplaySceneNames.Arena
            && saveData.SceneName != GameplaySceneNames.WildernessNorth
            && saveData.SceneName != GameplaySceneNames.WildernessSouth
            && saveData.SceneName != GameplaySceneNames.WildernessEast
            && saveData.SceneName != GameplaySceneNames.WildernessWest)
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
        _debugOverlay?.SetPosition(Rendering.Gameplay.GameplayHudLayout.GetDebugOverlayPosition(
            new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height)));
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

        _debugOverlay?.SetPosition(Rendering.Gameplay.GameplayHudLayout.GetDebugOverlayPosition(
            new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height)));
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

    private void ConfigureFullscreenPresentation()
    {
        var displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
        _graphics.PreferredBackBufferWidth = displayMode.Width;
        _graphics.PreferredBackBufferHeight = displayMode.Height;
        _graphics.HardwareModeSwitch = false;
        _graphics.IsFullScreen = true;
        _graphics.ApplyChanges();
    }
}
