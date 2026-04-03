using Microsoft.Xna.Framework.Graphics;
using MyGame.Core;
using MyGame.Core.Assets;
using MyGame.Core.Input;
using MyGame.Core.Rendering;
using MyGame.Scenes.MainMenu;

namespace MyGame.Tests.Scenes.MainMenu;

public sealed class MainMenuSceneTests
{
    [Fact]
    public void Constructor_IncludesControlsMenuItem()
    {
        var scene = CreateScene(new StubInputService(), new CallbackState());

        Assert.Collection(
            scene.Items,
            item => Assert.Equal("Start Game", item.Text),
            item => Assert.Equal("Load Game", item.Text),
            item => Assert.Equal("Controls", item.Text),
            item => Assert.Equal("Exit", item.Text));
    }

    [Fact]
    public void Update_ConfirmOnLoadGame_InvokesLoadCallback()
    {
        var inputService = new MutableInputService();
        var state = new CallbackState();
        var scene = CreateScene(inputService, state);
        scene.Enter();
        inputService.SetPressed(GameAction.MoveDown);
        scene.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        inputService.SetPressed(GameAction.Confirm);
        scene.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.2)));

        Assert.False(state.Started);
        Assert.True(state.Loaded);
        Assert.False(state.Exited);
    }

    [Fact]
    public void Update_ConfirmOnDisabledLoadGame_DoesNotInvokeCallback()
    {
        var inputService = new MutableInputService();
        var state = new CallbackState();
        var scene = new MainMenuScene(
            inputService,
            new StubRenderer(),
            new StubRenderContext(),
            onStartGame: () => state.Started = true,
            onLoadGame: () => state.Loaded = true,
            canLoadGame: () => false,
            onExitGame: () => state.Exited = true);
        scene.Enter();

        inputService.SetPressed(GameAction.MoveDown);
        scene.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        inputService.SetPressed(GameAction.Confirm);
        scene.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.2)));

        Assert.False(state.Loaded);
    }

    [Fact]
    public void Update_ConfirmOnControls_OpensControlsPanel()
    {
        var inputService = new MutableInputService();
        var scene = CreateScene(inputService, new CallbackState());
        scene.Enter();

        inputService.SetPressed(GameAction.MoveDown);
        scene.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        inputService.SetPressed(GameAction.MoveDown);
        scene.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.2)));
        inputService.SetPressed(GameAction.Confirm);
        scene.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.3)));

        Assert.True(scene.IsShowingControls);
    }

    [Fact]
    public void Update_WhenControlsPanelOpen_CancelClosesIt()
    {
        var inputService = new MutableInputService();
        var scene = CreateScene(inputService, new CallbackState());
        scene.Enter();
        scene.Items[2].OnSelected();

        inputService.SetPressed(GameAction.Cancel);
        scene.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.False(scene.IsShowingControls);
    }

    private static MainMenuScene CreateScene(IInputService inputService, CallbackState state)
    {
        return new MainMenuScene(
            inputService,
            new StubRenderer(),
            new StubRenderContext(),
            onStartGame: () => state.Started = true,
            onLoadGame: () => state.Loaded = true,
            canLoadGame: () => true,
            onExitGame: () => state.Exited = true);
    }

    private sealed class CallbackState
    {
        public bool Started { get; set; }

        public bool Loaded { get; set; }

        public bool Exited { get; set; }
    }

    private sealed class StubRenderer : IRenderer<MainMenuScene>
    {
        public void Draw(MainMenuScene model, FrameTime frameTime)
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
        public StubInputService(params GameAction[] justPressedActions) { }

        public InputSnapshot Current => InputSnapshot.Empty;

        public InputSnapshot Previous => InputSnapshot.Empty;

        public void Update()
        {
        }

        public bool IsPressed(GameAction action)
        {
            return false;
        }

        public bool IsJustPressed(GameAction action)
        {
            return false;
        }

        public bool IsJustReleased(GameAction action)
        {
            return false;
        }
    }

    private sealed class MutableInputService : IInputService
    {
        private HashSet<GameAction> _pressedActions = [];

        public InputSnapshot Current => InputSnapshot.Empty;

        public InputSnapshot Previous => InputSnapshot.Empty;

        public void SetPressed(params GameAction[] actions)
        {
            _pressedActions = actions.ToHashSet();
        }

        public void Update()
        {
        }

        public bool IsPressed(GameAction action)
        {
            return _pressedActions.Contains(action);
        }

        public bool IsJustPressed(GameAction action)
        {
            return _pressedActions.Contains(action);
        }

        public bool IsJustReleased(GameAction action)
        {
            return false;
        }
    }
}
