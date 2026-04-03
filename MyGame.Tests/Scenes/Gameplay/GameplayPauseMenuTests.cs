using MyGame.Core.Input;
using MyGame.Scenes.Gameplay;

namespace MyGame.Tests.Scenes.Gameplay;

public sealed class GameplayPauseMenuTests
{
    [Fact]
    public void Toggle_OpensMenu_AndSelectsResume()
    {
        var pauseMenu = new GameplayPauseMenu(() => { }, () => { });

        pauseMenu.Toggle();

        Assert.True(pauseMenu.IsOpen);
        Assert.Equal(0, pauseMenu.SelectedIndex);
        Assert.Equal("Resume", pauseMenu.SelectedText);
    }

    [Fact]
    public void Update_MoveDown_SelectsNextItem()
    {
        var pauseMenu = new GameplayPauseMenu(() => { }, () => { });
        var inputService = new StubInputService(GameAction.MoveDown);
        pauseMenu.Open();
        pauseMenu.Update(new StubInputService());

        pauseMenu.Update(inputService);

        Assert.Equal(1, pauseMenu.SelectedIndex);
        Assert.Equal("Main Menu", pauseMenu.SelectedText);
    }

    [Fact]
    public void Update_ConfirmOnResume_ClosesMenu()
    {
        GameplayPauseMenu? pauseMenu = null;
        pauseMenu = new GameplayPauseMenu(() => pauseMenu!.Close(), () => { });
        var inputService = new StubInputService(GameAction.Confirm);
        pauseMenu.Open();
        pauseMenu.Update(new StubInputService());

        pauseMenu.Update(inputService);

        Assert.False(pauseMenu.IsOpen);
    }

    [Fact]
    public void Update_ConfirmOnMainMenu_InvokesCallback()
    {
        var returnedToMainMenu = false;
        var pauseMenu = new GameplayPauseMenu(() => { }, () => returnedToMainMenu = true);
        var inputService = new StubInputService(GameAction.MoveDown, GameAction.Confirm);
        pauseMenu.Open();
        pauseMenu.Update(new StubInputService());

        pauseMenu.Update(inputService);

        Assert.True(returnedToMainMenu);
    }

    [Fact]
    public void Update_Cancel_ClosesMenu()
    {
        var pauseMenu = new GameplayPauseMenu(() => { }, () => { });
        var inputService = new StubInputService(GameAction.Cancel);
        pauseMenu.Open();
        pauseMenu.Update(new StubInputService());

        pauseMenu.Update(inputService);

        Assert.False(pauseMenu.IsOpen);
    }

    [Fact]
    public void Update_PausePressedImmediatelyAfterOpen_KeepsMenuOpen()
    {
        var pauseMenu = new GameplayPauseMenu(() => { }, () => { });
        var inputService = new StubInputService(GameAction.Pause);
        pauseMenu.Open();

        pauseMenu.Update(inputService);

        Assert.True(pauseMenu.IsOpen);
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
