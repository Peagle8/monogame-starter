using MyGame.Core.Input;
using MyGame.Scenes.Gameplay;

namespace MyGame.Tests.Scenes.Gameplay;

public sealed class GameplayPauseMenuTests
{
    [Fact]
    public void Toggle_OpensMenu_AndSelectsResume()
    {
        var pauseMenu = new GameplayPauseMenu(() => { }, () => { }, () => { }, () => true, () => { });

        pauseMenu.Toggle();

        Assert.True(pauseMenu.IsOpen);
        Assert.Equal(0, pauseMenu.SelectedIndex);
        Assert.Equal("Resume", pauseMenu.SelectedText);
    }

    [Fact]
    public void Update_MoveDown_SelectsNextItem()
    {
        var pauseMenu = new GameplayPauseMenu(() => { }, () => { }, () => { }, () => true, () => { });
        var inputService = new StubInputService(GameAction.MoveDown);
        pauseMenu.Open();
        pauseMenu.Update(new StubInputService());

        pauseMenu.Update(inputService);

        Assert.Equal(1, pauseMenu.SelectedIndex);
        Assert.Equal("Save Game", pauseMenu.SelectedText);
    }

    [Fact]
    public void Update_ConfirmOnResume_ClosesMenu()
    {
        GameplayPauseMenu? pauseMenu = null;
        pauseMenu = new GameplayPauseMenu(() => pauseMenu!.Close(), () => { }, () => { }, () => true, () => { });
        var inputService = new StubInputService(GameAction.Confirm);
        pauseMenu.Open();
        pauseMenu.Update(new StubInputService());

        pauseMenu.Update(inputService);

        Assert.False(pauseMenu.IsOpen);
    }

    [Fact]
    public void Update_ConfirmOnLoadGame_InvokesCallback()
    {
        var loadInvoked = false;
        var pauseMenu = new GameplayPauseMenu(() => { }, () => { }, () => loadInvoked = true, () => true, () => { });
        pauseMenu.Open();
        pauseMenu.Update(new StubInputService());
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));

        pauseMenu.Update(new StubInputService(GameAction.Confirm));

        Assert.True(loadInvoked);
    }

    [Fact]
    public void Update_ConfirmOnControls_OpensControlsPanel()
    {
        var pauseMenu = new GameplayPauseMenu(() => { }, () => { }, () => { }, () => true, () => { });
        pauseMenu.Open();
        pauseMenu.Update(new StubInputService());
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));

        pauseMenu.Update(new StubInputService(GameAction.Confirm));

        Assert.True(pauseMenu.IsShowingControls);
    }

    [Fact]
    public void Update_WhenControlsPanelOpen_CancelClosesControlsOnly()
    {
        var pauseMenu = new GameplayPauseMenu(() => { }, () => { }, () => { }, () => true, () => { });
        pauseMenu.Open();
        pauseMenu.Update(new StubInputService());
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));
        pauseMenu.Update(new StubInputService(GameAction.Confirm));

        pauseMenu.Update(new StubInputService(GameAction.Cancel));

        Assert.True(pauseMenu.IsOpen);
        Assert.False(pauseMenu.IsShowingControls);
    }

    [Fact]
    public void Update_Cancel_ClosesMenu()
    {
        var pauseMenu = new GameplayPauseMenu(() => { }, () => { }, () => { }, () => true, () => { });
        var inputService = new StubInputService(GameAction.Cancel);
        pauseMenu.Open();
        pauseMenu.Update(new StubInputService());

        pauseMenu.Update(inputService);

        Assert.False(pauseMenu.IsOpen);
    }

    [Fact]
    public void Update_PausePressedImmediatelyAfterOpen_KeepsMenuOpen()
    {
        var pauseMenu = new GameplayPauseMenu(() => { }, () => { }, () => { }, () => true, () => { });
        var inputService = new StubInputService(GameAction.Pause);
        pauseMenu.Open();

        pauseMenu.Update(inputService);

        Assert.True(pauseMenu.IsOpen);
    }

    [Fact]
    public void Update_ConfirmOnMainMenu_InvokesCallback()
    {
        var returnedToMainMenu = false;
        var pauseMenu = new GameplayPauseMenu(() => { }, () => { }, () => { }, () => true, () => returnedToMainMenu = true);
        pauseMenu.Open();
        pauseMenu.Update(new StubInputService());
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));

        pauseMenu.Update(new StubInputService(GameAction.Confirm));

        Assert.True(returnedToMainMenu);
    }

    [Fact]
    public void Update_ConfirmOnDisabledLoadGame_DoesNotInvokeCallback()
    {
        var loadInvoked = false;
        var pauseMenu = new GameplayPauseMenu(() => { }, () => { }, () => loadInvoked = true, () => false, () => { });
        pauseMenu.Open();
        pauseMenu.Update(new StubInputService());
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));

        pauseMenu.Update(new StubInputService(GameAction.Confirm));

        Assert.False(loadInvoked);
    }

    private sealed class StubInputService : IInputService
    {
        private readonly HashSet<GameAction> _justPressedActions;

        public StubInputService(params GameAction[] justPressedActions)
        {
            _justPressedActions = justPressedActions.ToHashSet();
        }

        public InputSnapshot Current => InputSnapshot.Empty;

        public InputSnapshot Previous => InputSnapshot.Empty;

        public void Update()
        {
        }

        public bool IsPressed(GameAction action)
        {
            return _justPressedActions.Contains(action);
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
}
