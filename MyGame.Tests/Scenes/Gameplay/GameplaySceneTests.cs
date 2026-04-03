using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Configuration;
using MyGame.Core;
using MyGame.Core.Assets;
using MyGame.Core.Input;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Player;
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
        scene.PauseMenu.Update(new StubInputService(GameAction.Confirm));

        Assert.NotNull(saveGameService.LastSavedData);
        Assert.Equal("Gameplay", saveGameService.LastSavedData!.SceneName);
        Assert.Equal(400f, saveGameService.LastSavedData.PlayerPositionX);
        Assert.Equal(240f, saveGameService.LastSavedData.PlayerPositionY);
        Assert.Equal(5, saveGameService.LastSavedData.PlayerHealth);
        Assert.Single(saveGameService.LastSavedData.Enemies);
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
                        PositionX = 512f,
                        PositionY = 288f,
                        CurrentHealth = 0
                    }
                ],
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
        scene.PauseMenu.Update(new StubInputService(GameAction.Confirm));

        Assert.Equal(new Vector2(128f, 256f), scene.World.Player.Position);
        Assert.Equal(2, scene.World.Player.CurrentHealth);
        Assert.Single(scene.World.Enemies);
        Assert.Equal(new Vector2(512f, 288f), scene.World.Enemies[0].Position);
        Assert.Equal(0, scene.World.Enemies[0].CurrentHealth);
    }

    private static GameplayScene CreateScene(
        IInputService inputService,
        CallbackState state,
        ISaveGameService? saveGameService = null,
        World? world = null)
    {
        world ??= CreateWorld(inputService);

        return new GameplayScene(
            inputService,
            world,
            new StubSceneRenderer(),
            new StubRenderContext(),
            saveGameService ?? new StubSaveGameService(),
            onRestart: () => state.Restarted = true,
            onReturnToMainMenu: () => state.ReturnedToMainMenu = true);
    }

    private static World CreateWorld(IInputService inputService)
    {
        var attackController = new PlayerAttackController(new PlayerAttackSettings());
        var player = new PlayerActor(
            inputService,
            new PlayerMovementController(new PlayerMovementSettings { MoveSpeed = 180f }),
            attackController);
        return new World(player, new EnemySettings());
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
