using MyGame.Core.Input;
using MyGame.Scenes.MainMenu;

namespace MyGame.Scenes.Gameplay;

public sealed class GameplayPauseMenu
{
    private readonly List<MenuItem> _items;
    private readonly List<MenuItem> _replayItems;
    private bool _skipInputUntilNextUpdate;

    public GameplayPauseMenu(
        Action onResume,
        Action onSaveGame,
        Action onLoadGame,
        Func<bool> canLoadGame,
        bool showReplayMenu,
        Func<string> recordingToggleText,
        Action onToggleRecording,
        Action onReplayLastRecording,
        Func<bool> canReplayRecording,
        Action onReturnToMainMenu)
    {
        _items =
        [
            new MenuItem("Resume", onResume),
            new MenuItem("Save Game", onSaveGame),
            new MenuItem("Load Game", onLoadGame, canLoadGame),
            new MenuItem("Controls", OpenControls),
        ];

        if (showReplayMenu)
        {
            _items.Add(new MenuItem("Replay", OpenReplayMenu));
        }

        _items.Add(new MenuItem("Main Menu", onReturnToMainMenu));

        _replayItems =
        [
            new MenuItem(recordingToggleText, onToggleRecording),
            new MenuItem("Replay Last Recording", onReplayLastRecording, canReplayRecording),
            new MenuItem("Back", CloseReplayMenu)
        ];
    }

    public bool IsOpen { get; private set; }

    public bool IsShowingControls { get; private set; }

    public bool IsShowingReplayMenu { get; private set; }

    public string? StatusMessage { get; private set; }

    public int SelectedIndex { get; private set; }

    public string SelectedText => _items[SelectedIndex].Text;

    public IReadOnlyList<MenuItem> Items => _items;

    public int ReplaySelectedIndex { get; private set; }

    public string ReplaySelectedText => _replayItems[ReplaySelectedIndex].Text;

    public IReadOnlyList<MenuItem> ReplayItems => _replayItems;

    public string FooterText => StatusMessage ?? GetFooterText();

    public void Open()
    {
        IsOpen = true;
        IsShowingControls = false;
        IsShowingReplayMenu = false;
        SelectedIndex = 0;
        ReplaySelectedIndex = 0;
        StatusMessage = null;
        _skipInputUntilNextUpdate = true;
    }

    public void Close()
    {
        IsOpen = false;
        IsShowingControls = false;
        IsShowingReplayMenu = false;
        SelectedIndex = 0;
        ReplaySelectedIndex = 0;
        StatusMessage = null;
        _skipInputUntilNextUpdate = false;
    }

    public void Toggle()
    {
        if (IsOpen)
        {
            Close();
            return;
        }

        Open();
    }

    public void Update(IInputService inputService)
    {
        if (!IsOpen)
        {
            return;
        }

        if (_skipInputUntilNextUpdate)
        {
            _skipInputUntilNextUpdate = false;
            return;
        }

        if (IsShowingControls)
        {
            if (inputService.IsJustPressed(GameAction.Confirm)
                || inputService.IsJustPressed(GameAction.Cancel)
                || inputService.IsJustPressed(GameAction.Pause))
            {
                IsShowingControls = false;
            }

            return;
        }

        if (IsShowingReplayMenu)
        {
            UpdateReplayMenu(inputService);
            return;
        }

        if (inputService.IsJustPressed(GameAction.Cancel) || inputService.IsJustPressed(GameAction.Pause))
        {
            Close();
            return;
        }

        if (inputService.IsJustPressed(GameAction.MoveDown))
        {
            SelectedIndex = (SelectedIndex + 1) % _items.Count;
        }

        if (inputService.IsJustPressed(GameAction.MoveUp))
        {
            SelectedIndex = (SelectedIndex - 1 + _items.Count) % _items.Count;
        }

        if (inputService.IsJustPressed(GameAction.Confirm))
        {
            var selectedItem = _items[SelectedIndex];
            if (selectedItem.IsEnabled)
            {
                selectedItem.OnSelected();
            }
            else if (selectedItem.Text == "Load Game")
            {
                StatusMessage = "No save found yet.";
            }
        }
    }

    public void SetStatus(string? statusMessage)
    {
        StatusMessage = statusMessage;
    }

    private void OpenControls()
    {
        IsShowingControls = true;
    }

    private void OpenReplayMenu()
    {
        IsShowingReplayMenu = true;
        ReplaySelectedIndex = 0;
    }

    private void CloseReplayMenu()
    {
        IsShowingReplayMenu = false;
        ReplaySelectedIndex = 0;
    }

    private void UpdateReplayMenu(IInputService inputService)
    {
        if (inputService.IsJustPressed(GameAction.Cancel) || inputService.IsJustPressed(GameAction.Pause))
        {
            CloseReplayMenu();
            return;
        }

        if (inputService.IsJustPressed(GameAction.MoveDown))
        {
            ReplaySelectedIndex = (ReplaySelectedIndex + 1) % _replayItems.Count;
        }

        if (inputService.IsJustPressed(GameAction.MoveUp))
        {
            ReplaySelectedIndex = (ReplaySelectedIndex - 1 + _replayItems.Count) % _replayItems.Count;
        }

        if (inputService.IsJustPressed(GameAction.Confirm))
        {
            var selectedItem = _replayItems[ReplaySelectedIndex];
            if (selectedItem.IsEnabled)
            {
                selectedItem.OnSelected();
            }
        }
    }

    private string GetFooterText()
    {
        if (IsShowingReplayMenu)
        {
            var selectedItem = _replayItems[ReplaySelectedIndex];
            return selectedItem.Text switch
            {
                "Replay Last Recording" when !selectedItem.IsEnabled => "Record a run before playback is available.",
                "Replay Last Recording" => "Restart gameplay using the last recorded input sequence.",
                "Back" => "Return to the pause menu.",
                _ => "Toggle recording diagnostics for this run."
            };
        }

        var menuItem = _items[SelectedIndex];
        return menuItem.Text switch
        {
            "Resume" => "Return to gameplay.",
            "Save Game" => "Write your current progress to the active save file.",
            "Load Game" when !menuItem.IsEnabled => "No save found yet.",
            "Load Game" => "Restore the latest saved gameplay state.",
            "Controls" => "Review the current controls.",
            "Replay" => "Open replay and recording diagnostics.",
            "Main Menu" => "Leave this run and return to the title screen.",
            _ => "Press Enter to select."
        };
    }
}
