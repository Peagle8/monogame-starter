using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Configuration;
using MyGame.Core;
using MyGame.Core.Assets;
using MyGame.Core.Diagnostics;
using MyGame.Core.Input;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Props;
using MyGame.Gameplay.World;
using MyGame.Infrastructure.Save;
using MyGame.Scenes.Gameplay;

namespace MyGame.Tests.Scenes.Gameplay;

public sealed class GameplaySceneTests
{
    [Fact]
    public void Update_WhenPlayerDead_ConfirmRestartsGameplay()
    {
        var inputService = new StubInputService(GameAction.Confirm);
        var state = new CallbackState();
        var scene = CreateScene(inputService, state);
        scene.World.Player.TakeDamage(scene.World.Player.MaxHealth);

        scene.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.True(state.Restarted);
        Assert.False(state.ReturnedToMainMenu);
    }

    [Fact]
    public void Update_WhenPlayerDead_CancelReturnsToMainMenu()
    {
        var inputService = new StubInputService(GameAction.Cancel);
        var state = new CallbackState();
        var scene = CreateScene(inputService, state);
        scene.World.Player.TakeDamage(scene.World.Player.MaxHealth);

        scene.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.False(state.Restarted);
        Assert.True(state.ReturnedToMainMenu);
    }

    [Fact]
    public void Update_WhenPlayerDead_DoesNotAdvanceWorldSimulation()
    {
        var inputService = new StubInputService(current: new InputSnapshot(new HashSet<GameAction> { GameAction.MoveRight }));
        var scene = CreateScene(inputService, new CallbackState());
        var initialPosition = scene.World.Player.Position;
        scene.World.Player.TakeDamage(scene.World.Player.MaxHealth);

        scene.Update(new FrameTime(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)));

        Assert.Equal(initialPosition, scene.World.Player.Position);
    }

    [Fact]
    public void GetDebugState_WhenPlayerDead_ReportsDeadState()
    {
        var scene = CreateScene(new StubInputService(), new CallbackState());
        scene.World.Player.TakeDamage(scene.World.Player.MaxHealth);

        var debugState = scene.GetDebugState();

        Assert.Equal("True", debugState["PlayerDead"]);
    }

    [Fact]
    public void Update_WhenPauseMenuSaveSelected_SavesCurrentGameplayState()
    {
        var saveGameService = new StubSaveGameService();
        var scene = CreateScene(
            new StubInputService(GameAction.Pause),
            new CallbackState(),
            saveGameService);

        scene.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        scene = CreateScene(new StubInputService(), new CallbackState(), saveGameService, scene.World);
        scene.PauseMenu.Open();
        scene.PauseMenu.Update(new StubInputService());
        scene.PauseMenu.Update(new StubInputService(GameAction.MoveDown));
        scene.PauseMenu.Update(new StubInputService(GameAction.MoveDown));
        scene.PauseMenu.Update(new StubInputService(GameAction.Confirm));

        Assert.NotNull(saveGameService.LastSavedData);
        Assert.Equal("Gameplay", saveGameService.LastSavedData!.SceneName);
        Assert.Equal(3f, saveGameService.LastSavedData.PlayerAbilityPoints);
        Assert.Equal(400f, saveGameService.LastSavedData.PlayerPositionX);
        Assert.Equal(240f, saveGameService.LastSavedData.PlayerPositionY);
        Assert.Equal(20, saveGameService.LastSavedData.PlayerHealth);
        Assert.Single(saveGameService.LastSavedData.Enemies);
        Assert.Equal("Game saved.", scene.PauseMenu.StatusMessage);
        Assert.True(scene.PauseMenu.IsOpen);
    }

    [Fact]
    public void Update_WhenPauseMenuLoadSelected_RestoresGameplayState()
    {
        var saveGameService = new StubSaveGameService
        {
            NextLoadData = new SaveGameData
            {
                SceneName = "Gameplay",
                DefeatedEnemyCount = 1,
                Enemies =
                [
                    new EnemySaveData
                    {
                        Kind = EnemyKind.Crab,
                        AxisPreference = EnemyAxisPreference.None,
                        PositionX = 512f,
                        PositionY = 288f,
                        CurrentHealth = 0
                    }
                ],
                PlayerAbilityPoints = 1.25f,
                PlayerHealth = 2,
                PlayerPositionX = 128f,
                PlayerPositionY = 256f
            }
        };
        var scene = CreateScene(
            new StubInputService(),
            new CallbackState(),
            saveGameService);

        scene.PauseMenu.Open();
        scene.PauseMenu.Update(new StubInputService());
        scene.PauseMenu.Update(new StubInputService(GameAction.MoveDown));
        scene.PauseMenu.Update(new StubInputService(GameAction.MoveDown));
        scene.PauseMenu.Update(new StubInputService(GameAction.MoveDown));
        scene.PauseMenu.Update(new StubInputService(GameAction.Confirm));

        Assert.Equal(new Vector2(128f, 256f), scene.World.Player.Position);
        Assert.Equal(2, scene.World.Player.CurrentHealth);
        Assert.Equal(1.25f, scene.World.Player.CurrentAbilityPoints);
        Assert.Single(scene.World.Enemies);
        Assert.Equal(new Vector2(512f, 288f), scene.World.Enemies[0].Position);
        Assert.Equal(0, scene.World.Enemies[0].CurrentHealth);
        Assert.Equal("Game loaded.", scene.PauseMenu.StatusMessage);
        Assert.True(scene.PauseMenu.IsOpen);
    }

    [Fact]
    public void Update_WhenPauseMenuLoadSelectedWithoutSave_ShowsFeedback()
    {
        var saveGameService = new StubSaveGameService { Exists = false };
        var scene = CreateScene(
            new StubInputService(),
            new CallbackState(),
            saveGameService);

        scene.PauseMenu.Open();
        scene.PauseMenu.Update(new StubInputService());
        scene.PauseMenu.Update(new StubInputService(GameAction.MoveDown));
        scene.PauseMenu.Update(new StubInputService(GameAction.MoveDown));
        scene.PauseMenu.Update(new StubInputService(GameAction.MoveDown));
        scene.PauseMenu.Update(new StubInputService(GameAction.Confirm));

        Assert.Equal("No save found yet.", scene.PauseMenu.StatusMessage);
        Assert.True(scene.PauseMenu.IsOpen);
    }

    [Fact]
    public void Update_WhenReplaySelected_StartsReplayAndRestartsGameplay()
    {
        var recorder = new GameRecorder();
        recorder.StartRecording();
        recorder.Capture(new RecordedFrame(
            TimeSpan.Zero,
            "Gameplay",
            new InputSnapshot(new HashSet<GameAction> { GameAction.MoveRight }),
            new Dictionary<string, string>()));
        recorder.StopRecording();
        var state = new CallbackState();
        var scene = CreateScene(new StubInputService(), state, gameRecorder: recorder);

        scene.PauseMenu.Open();
        scene.PauseMenu.Update(new StubInputService());
        scene.PauseMenu.Update(new StubInputService(GameAction.MoveDown));
        scene.PauseMenu.Update(new StubInputService(GameAction.MoveDown));
        scene.PauseMenu.Update(new StubInputService(GameAction.MoveDown));
        scene.PauseMenu.Update(new StubInputService(GameAction.MoveDown));
        scene.PauseMenu.Update(new StubInputService(GameAction.MoveDown));
        scene.PauseMenu.Update(new StubInputService(GameAction.Confirm));
        scene.PauseMenu.Update(new StubInputService(GameAction.MoveDown));

        scene.PauseMenu.Update(new StubInputService(GameAction.Confirm));

        Assert.True(state.Restarted);
        Assert.True(recorder.IsReplaying);
    }

    [Fact]
    public void Update_WhenPausePressedDuringReplay_PausesReplayAndOpensPauseMenu()
    {
        var recorder = new GameRecorder();
        recorder.StartReplay(new[]
        {
            new RecordedFrame(
                TimeSpan.Zero,
                "Gameplay",
                new InputSnapshot(new HashSet<GameAction> { GameAction.MoveRight }),
                new Dictionary<string, string>())
        });
        var scene = CreateScene(new StubInputService(GameAction.Pause), new CallbackState(), gameRecorder: recorder);

        scene.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.True(scene.PauseMenu.IsOpen);
        Assert.True(recorder.IsReplayPaused);
    }

    [Fact]
    public void Update_WhenReplayPaused_ResumeContinuesReplay()
    {
        var recorder = new GameRecorder();
        recorder.StartReplay(new[]
        {
            new RecordedFrame(
                TimeSpan.Zero,
                "Gameplay",
                new InputSnapshot(new HashSet<GameAction> { GameAction.MoveRight }),
                new Dictionary<string, string>())
        });
        recorder.PauseReplay();
        var scene = CreateScene(new StubInputService(), new CallbackState(), gameRecorder: recorder);

        scene.PauseMenu.Open();
        scene.PauseMenu.Update(new StubInputService());
        scene.PauseMenu.Update(new StubInputService(GameAction.Confirm));

        Assert.False(scene.PauseMenu.IsOpen);
        Assert.False(recorder.IsReplayPaused);
        Assert.True(recorder.IsReplaying);
    }

    [Fact]
    public void Update_WhenRecordingToggleSelected_StartsRecordingAndClosesPauseMenu()
    {
        var recorder = new GameRecorder();
        var scene = CreateScene(new StubInputService(), new CallbackState(), gameRecorder: recorder);

        scene.PauseMenu.Open();
        scene.PauseMenu.Update(new StubInputService());
        scene.PauseMenu.Update(new StubInputService(GameAction.MoveDown));
        scene.PauseMenu.Update(new StubInputService(GameAction.MoveDown));
        scene.PauseMenu.Update(new StubInputService(GameAction.MoveDown));
        scene.PauseMenu.Update(new StubInputService(GameAction.MoveDown));
        scene.PauseMenu.Update(new StubInputService(GameAction.MoveDown));
        scene.PauseMenu.Update(new StubInputService(GameAction.Confirm));
        scene.PauseMenu.Update(new StubInputService(GameAction.Confirm));

        Assert.True(recorder.IsRecording);
        Assert.False(scene.PauseMenu.IsOpen);
    }

    [Fact]
    public void Update_WhenRecordingToggleSelectedAgain_StopsRecordingAndKeepsPauseMenuOpen()
    {
        var recorder = new GameRecorder();
        recorder.StartRecording();
        var scene = CreateScene(new StubInputService(), new CallbackState(), gameRecorder: recorder);

        scene.PauseMenu.Open();
        scene.PauseMenu.Update(new StubInputService());
        scene.PauseMenu.Update(new StubInputService(GameAction.MoveDown));
        scene.PauseMenu.Update(new StubInputService(GameAction.MoveDown));
        scene.PauseMenu.Update(new StubInputService(GameAction.MoveDown));
        scene.PauseMenu.Update(new StubInputService(GameAction.MoveDown));
        scene.PauseMenu.Update(new StubInputService(GameAction.MoveDown));
        scene.PauseMenu.Update(new StubInputService(GameAction.Confirm));
        scene.PauseMenu.Update(new StubInputService(GameAction.Confirm));

        Assert.False(recorder.IsRecording);
        Assert.True(scene.PauseMenu.IsOpen);
        Assert.True(scene.PauseMenu.IsShowingReplayMenu);
    }

    [Fact]
    public void Update_WhenWorldRequestsSceneTransition_InvokesTransitionCallback()
    {
        var requestedTransition = default(WorldSceneTransition);
        var world = CreateTransitionWorld(new Rectangle(400, 240, 24, 24));
        var scene = CreateScene(
            new StubInputService(),
            new CallbackState(),
            world: world,
            onSceneTransition: transition => requestedTransition = transition);

        scene.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.NotNull(requestedTransition);
        Assert.Equal(GameplaySceneNames.ShopInterior, requestedTransition!.TargetSceneName);
        Assert.Equal(new Vector2(384f, 304f), requestedTransition.TargetPlayerPosition);
    }

    [Fact]
    public void Update_WhenWorldTransitionIsBlocked_DoesNotInvokeTransitionCallback()
    {
        var requestedTransition = default(WorldSceneTransition);
        var world = CreateBlockedTransitionWorld(new Rectangle(400, 240, 24, 24));
        var scene = CreateScene(
            new StubInputService(),
            new CallbackState(),
            world: world,
            onSceneTransition: transition => requestedTransition = transition);

        scene.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.Null(requestedTransition);
    }

    [Fact]
    public void Update_WhenShopDialogueIsOpen_DoesNotAdvanceWorldSimulation()
    {
        var inputService = new StubInputService(GameAction.Interact);
        var world = CreateShopWorld(inputService);
        var scene = CreateScene(inputService, new CallbackState(), world: world);
        var initialPosition = scene.World.Player.Position;

        scene.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        Assert.True(scene.ShopDialogue.IsOpen);

        scene.Update(new FrameTime(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1.1)));

        Assert.Equal(initialPosition, scene.World.Player.Position);
    }

    [Fact]
    public void Constructor_WhenReplayMenuDisabled_HidesReplayEntry()
    {
        var scene = CreateScene(
            new StubInputService(),
            new CallbackState(),
            diagnosticsSettings: new DiagnosticsSettings { EnableReplayMenu = false });

        Assert.DoesNotContain(scene.PauseMenu.Items, item => item.Text == "Replay");
    }

    private static GameplayScene CreateScene(
        IInputService inputService,
        CallbackState state,
        ISaveGameService? saveGameService = null,
        World? world = null,
        GameRecorder? gameRecorder = null,
        DiagnosticsSettings? diagnosticsSettings = null,
        Action<WorldSceneTransition>? onSceneTransition = null)
    {
        world ??= CreateWorld(inputService);

        return new GameplayScene(
            GameplaySceneNames.Overworld,
            inputService,
            world,
            new StubSceneRenderer(),
            new StubRenderContext(),
            saveGameService ?? new StubSaveGameService(),
            gameRecorder ?? new GameRecorder(),
            diagnosticsSettings ?? new DiagnosticsSettings(),
            onRestart: () => state.Restarted = true,
            onReturnToMainMenu: () => state.ReturnedToMainMenu = true,
            onSceneTransition: onSceneTransition ?? (_ => { }));
    }

    private static World CreateWorld(IInputService inputService)
    {
        var movementSettings = new PlayerMovementSettings { MoveSpeed = 180f };
        var attackController = new PlayerAttackController(new PlayerAttackSettings());
        var player = new PlayerActor(
            inputService,
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash]),
            attackController);
        return new World(player, new EnemySettings());
    }

    private static World CreateTransitionWorld(Rectangle triggerBounds)
    {
        var inputService = new StubInputService();
        var movementSettings = new PlayerMovementSettings { MoveSpeed = 180f };
        var player = new PlayerActor(
            inputService,
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));

        return new World(
            player,
            [],
            [],
            sceneTransitions:
            [
                new WorldSceneTransition(
                    triggerBounds,
                    GameplaySceneNames.ShopInterior,
                    new Vector2(384f, 304f))
            ]);
    }

    private static World CreateBlockedTransitionWorld(Rectangle triggerBounds)
    {
        var inputService = new StubInputService();
        var movementSettings = new PlayerMovementSettings { MoveSpeed = 180f };
        var player = new PlayerActor(
            inputService,
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));
        var miniboss = new EnemyActor(
            EnemySettingsCatalog.CreateDefault(EnemyKind.BatMiniBoss),
            new Vector2(560f, 240f));

        return new World(
            player,
            [],
            [miniboss],
            sceneTransitions:
            [
                new WorldSceneTransition(
                    triggerBounds,
                    GameplaySceneNames.ShopInterior,
                    new Vector2(384f, 304f),
                    canTrigger: world => !world.HasLivingEnemy(EnemyKind.BatMiniBoss))
            ]);
    }

    private static World CreateShopWorld(IInputService inputService)
    {
        var movementSettings = new PlayerMovementSettings { MoveSpeed = 180f };
        var player = new PlayerActor(
            inputService,
            new PlayerCombatSettings(),
            new PlayerMovementController(movementSettings),
            new PlayerDashController(movementSettings),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));
        player.RestoreState(new Vector2(384f, 264f), player.MaxHealth);

        return new World(
            player,
            [new CounterProp(new Vector2(352f, 232f), new Point(96, 24))],
            []);
    }

    private sealed class CallbackState
    {
        public bool Restarted { get; set; }

        public bool ReturnedToMainMenu { get; set; }
    }

    private sealed class StubSceneRenderer : IRenderer<GameplayScene>
    {
        public void Draw(GameplayScene model, FrameTime frameTime)
        {
        }
    }

    private sealed class StubRenderContext : IRenderContext
    {
        public SpriteBatch SpriteBatch => throw new NotSupportedException();

        public IAssetCatalog Assets => throw new NotSupportedException();

        public RenderCamera Camera => throw new NotSupportedException();

        public void Bind(SpriteBatch spriteBatch, IAssetCatalog assetCatalog, RenderCamera camera)
        {
        }
    }

    private sealed class StubInputService : IInputService
    {
        private readonly HashSet<GameAction> _justPressedActions;

        public StubInputService(params GameAction[] justPressedActions)
            : this(InputSnapshot.Empty, justPressedActions)
        {
        }

        public StubInputService(InputSnapshot? current = null, params GameAction[] justPressedActions)
        {
            Current = current ?? InputSnapshot.Empty;
            _justPressedActions = justPressedActions.ToHashSet();
        }

        public InputSnapshot Current { get; }

        public InputSnapshot Previous => InputSnapshot.Empty;

        public void Update()
        {
        }

        public bool IsPressed(GameAction action)
        {
            return Current.IsPressed(action) || _justPressedActions.Contains(action);
        }

        public bool IsJustPressed(GameAction action)
        {
            return _justPressedActions.Contains(action);
        }

        public bool IsJustReleased(GameAction action)
        {
            return false;
        }
    }

    private sealed class StubSaveGameService : ISaveGameService
    {
        public SaveGameData? LastSavedData { get; private set; }

        public SaveGameData? NextLoadData { get; set; }

        public bool Exists { get; set; } = true;

        public bool SaveExists()
        {
            return Exists;
        }

        public SaveGameData? Load()
        {
            return NextLoadData;
        }

        public void Save(SaveGameData data)
        {
            LastSavedData = data;
        }
    }
}
