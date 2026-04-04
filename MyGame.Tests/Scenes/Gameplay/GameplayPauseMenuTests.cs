using MyGame.Core.Input;
using MyGame.Scenes.Gameplay;

namespace MyGame.Tests.Scenes.Gameplay;

public sealed class GameplayPauseMenuTests
{
    [Fact]
    public void Toggle_OpensMenu_AndSelectsResume()
    {
        var pauseMenu = CreatePauseMenu();

        pauseMenu.Toggle();

        Assert.True(pauseMenu.IsOpen);
        Assert.Equal(0, pauseMenu.SelectedIndex);
        Assert.Equal("Resume", pauseMenu.SelectedText);
    }

    [Fact]
    public void Update_MoveDown_SelectsNextItem()
    {
        var pauseMenu = CreatePauseMenu();
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
        pauseMenu = CreatePauseMenu(onResume: () => pauseMenu!.Close());
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
        var pauseMenu = CreatePauseMenu(onLoadGame: () => loadInvoked = true);
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
        var pauseMenu = CreatePauseMenu();
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
        var pauseMenu = CreatePauseMenu();
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
        var pauseMenu = CreatePauseMenu();
        var inputService = new StubInputService(GameAction.Cancel);
        pauseMenu.Open();
        pauseMenu.Update(new StubInputService());

        pauseMenu.Update(inputService);

        Assert.False(pauseMenu.IsOpen);
    }

    [Fact]
    public void Update_PausePressedImmediatelyAfterOpen_KeepsMenuOpen()
    {
        var pauseMenu = CreatePauseMenu();
        var inputService = new StubInputService(GameAction.Pause);
        pauseMenu.Open();

        pauseMenu.Update(inputService);

        Assert.True(pauseMenu.IsOpen);
    }

    [Fact]
    public void Update_ConfirmOnMainMenu_InvokesCallback()
    {
        var returnedToMainMenu = false;
        var pauseMenu = CreatePauseMenu(onReturnToMainMenu: () => returnedToMainMenu = true);
        pauseMenu.Open();
        pauseMenu.Update(new StubInputService());
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));
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
        var pauseMenu = CreatePauseMenu(onLoadGame: () => loadInvoked = true, canLoadGame: () => false);
        pauseMenu.Open();
        pauseMenu.Update(new StubInputService());
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));

        pauseMenu.Update(new StubInputService(GameAction.Confirm));

        Assert.False(loadInvoked);
    }

    [Fact]
    public void Update_ConfirmOnReplay_OpensReplaySubmenu()
    {
        var pauseMenu = CreatePauseMenu();
        pauseMenu.Open();
        pauseMenu.Update(new StubInputService());
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));

        pauseMenu.Update(new StubInputService(GameAction.Confirm));

        Assert.True(pauseMenu.IsShowingReplayMenu);
        Assert.Equal("Start Recording", pauseMenu.ReplaySelectedText);
    }

    [Fact]
    public void Update_WhenReplaySubmenuOpen_ConfirmInvokesReplay()
    {
        var replayInvoked = false;
        var pauseMenu = CreatePauseMenu(onReplayLastRecording: () => replayInvoked = true);
        pauseMenu.Open();
        pauseMenu.Update(new StubInputService());
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));
        pauseMenu.Update(new StubInputService(GameAction.Confirm));
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));

        pauseMenu.Update(new StubInputService(GameAction.Confirm));

        Assert.True(replayInvoked);
    }

    [Fact]
    public void Update_WhenReplaySubmenuOpen_CancelReturnsToPauseMenu()
    {
        var pauseMenu = CreatePauseMenu();
        pauseMenu.Open();
        pauseMenu.Update(new StubInputService());
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));
        pauseMenu.Update(new StubInputService(GameAction.Confirm));

        pauseMenu.Update(new StubInputService(GameAction.Cancel));

        Assert.True(pauseMenu.IsOpen);
        Assert.False(pauseMenu.IsShowingReplayMenu);
    }

    [Fact]
    public void Constructor_WhenReplayMenuDisabled_OmitsReplayItem()
    {
        var pauseMenu = CreatePauseMenu(showReplayMenu: false);

        Assert.DoesNotContain(pauseMenu.Items, item => item.Text == "Replay");
    }

    [Fact]
    public void Update_WhenReplaySubmenuOpen_ConfirmOnRecordingToggleInvokesCallback()
    {
        var toggled = false;
        var pauseMenu = CreatePauseMenu(onToggleRecording: () => toggled = true);
        pauseMenu.Open();
        pauseMenu.Update(new StubInputService());
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));
        pauseMenu.Update(new StubInputService(GameAction.MoveDown));
        pauseMenu.Update(new StubInputService(GameAction.Confirm));

        pauseMenu.Update(new StubInputService(GameAction.Confirm));

        Assert.True(toggled);
    }

    private static GameplayPauseMenu CreatePauseMenu(
        Action? onResume = null,
        Action? onSaveGame = null,
        Action? onLoadGame = null,
        Func<bool>? canLoadGame = null,
        bool showReplayMenu = true,
        Func<string>? recordingToggleText = null,
        Action? onToggleRecording = null,
        Action? onReplayLastRecording = null,
        Func<bool>? canReplayRecording = null,
        Action? onReturnToMainMenu = null)
    {
        return new GameplayPauseMenu(
            onResume ?? (() => { }),
            onSaveGame ?? (() => { }),
            onLoadGame ?? (() => { }),
            canLoadGame ?? (() => true),
            showReplayMenu,
            recordingToggleText ?? (() => "Start Recording"),
            onToggleRecording ?? (() => { }),
            onReplayLastRecording ?? (() => { }),
            canReplayRecording ?? (() => true),
            onReturnToMainMenu ?? (() => { }));
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
