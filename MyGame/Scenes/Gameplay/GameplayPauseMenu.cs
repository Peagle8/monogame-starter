using MyGame.Core.Input;
using MyGame.Gameplay.Inventory;
using MyGame.Infrastructure.Save;
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
            new MenuItem("Inventory", OpenInventoryMenu),
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

    public static GameplayPauseMenu CreateGameplayMenu(
        ISaveGameService saveGameService,
        Core.Diagnostics.GameRecorder gameRecorder,
        Configuration.DiagnosticsSettings diagnosticsSettings,
        string sceneName,
        Func<SaveGameData> createSaveData,
        Action<SaveGameData> applySaveData,
        Action onRestart,
        Action onReturnToMainMenu)
    {
        GameplayPauseMenu? pauseMenu = null;
        pauseMenu = new GameplayPauseMenu(
            onResume: () =>
            {
                gameRecorder.ResumeReplay();
                pauseMenu!.Close();
            },
            onSaveGame: () =>
            {
                saveGameService.Save(createSaveData());
                pauseMenu!.SetStatus("Game saved.");
            },
            onLoadGame: () =>
            {
                var data = saveGameService.Load();
                if (data is not null && data.SceneName == sceneName)
                {
                    applySaveData(data);
                    pauseMenu!.SetStatus("Game loaded.");
                    return;
                }

                pauseMenu!.SetStatus("No gameplay save could be loaded.");
            },
            canLoadGame: () => saveGameService.SaveExists(),
            showReplayMenu: diagnosticsSettings.EnableReplayMenu,
            recordingToggleText: () => gameRecorder.IsRecording ? "Stop Recording" : "Start Recording",
            onToggleRecording: () =>
            {
                if (gameRecorder.IsRecording)
                {
                    gameRecorder.StopRecording();
                    return;
                }

                gameRecorder.StartRecording();
                pauseMenu!.Close();
            },
            onReplayLastRecording: () =>
            {
                gameRecorder.StartReplayFromBeginning();
                onRestart();
            },
            canReplayRecording: () => gameRecorder.Frames.Count > 0 && !gameRecorder.IsRecording,
            onReturnToMainMenu: () =>
            {
                gameRecorder.StopReplay();
                onReturnToMainMenu();
            });
        return pauseMenu;
    }

    public bool IsOpen { get; private set; }

    public bool IsShowingControls { get; private set; }

    public bool IsShowingInventoryMenu { get; private set; }

    public bool IsShowingReplayMenu { get; private set; }

    public string? StatusMessage { get; private set; }

    public int SelectedIndex { get; private set; }

    public string SelectedText => _items[SelectedIndex].Text;

    public IReadOnlyList<MenuItem> Items => _items;

    public int ReplaySelectedIndex { get; private set; }

    public string ReplaySelectedText => _replayItems[ReplaySelectedIndex].Text;

    public IReadOnlyList<MenuItem> ReplayItems => _replayItems;

    public PlayerInventoryTab InventoryTab { get; private set; } = PlayerInventoryTab.Weapons;

    public string FooterText => StatusMessage ?? GetFooterText();

    public void Open()
    {
        IsOpen = true;
        IsShowingControls = false;
        IsShowingInventoryMenu = false;
        IsShowingReplayMenu = false;
        InventoryTab = PlayerInventoryTab.Weapons;
        SelectedIndex = 0;
        ReplaySelectedIndex = 0;
        StatusMessage = null;
        _skipInputUntilNextUpdate = true;
    }

    public void Close()
    {
        IsOpen = false;
        IsShowingControls = false;
        IsShowingInventoryMenu = false;
        IsShowingReplayMenu = false;
        InventoryTab = PlayerInventoryTab.Weapons;
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

        if (IsShowingInventoryMenu)
        {
            UpdateInventoryMenu(inputService);
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

    private void OpenInventoryMenu()
    {
        IsShowingInventoryMenu = true;
        InventoryTab = PlayerInventoryTab.Weapons;
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

    private void CloseInventoryMenu()
    {
        IsShowingInventoryMenu = false;
        InventoryTab = PlayerInventoryTab.Weapons;
    }

    private void UpdateInventoryMenu(IInputService inputService)
    {
        if (inputService.IsJustPressed(GameAction.Cancel) || inputService.IsJustPressed(GameAction.Pause))
        {
            CloseInventoryMenu();
            return;
        }

        if (inputService.IsJustPressed(GameAction.PreviousTab))
        {
            InventoryTab = PreviousInventoryTab(InventoryTab);
        }

        if (inputService.IsJustPressed(GameAction.NextTab))
        {
            InventoryTab = NextInventoryTab(InventoryTab);
        }
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

        if (IsShowingInventoryMenu)
        {
            return InventoryTab switch
            {
                PlayerInventoryTab.Weapons => "Equip and compare your currently owned weapons.",
                PlayerInventoryTab.Armor => "Review armor pieces once defensive gear is added.",
                PlayerInventoryTab.Items => "Consumables and key items will appear here later.",
                PlayerInventoryTab.Abilities => "Track unlocked abilities and future upgrades here.",
                _ => "Browse your inventory tabs."
            };
        }

        var menuItem = _items[SelectedIndex];
        return menuItem.Text switch
        {
            "Resume" => "Return to gameplay.",
            "Inventory" => "Open your inventory and browse equipment, items, and abilities.",
            "Save Game" => "Write your current progress to the active save file.",
            "Load Game" when !menuItem.IsEnabled => "No save found yet.",
            "Load Game" => "Restore the latest saved gameplay state.",
            "Controls" => "Review the current controls.",
            "Replay" => "Open replay and recording diagnostics.",
            "Main Menu" => "Leave this run and return to the title screen.",
            _ => "Press Enter to select."
        };
    }

    private static PlayerInventoryTab PreviousInventoryTab(PlayerInventoryTab tab)
    {
        return tab switch
        {
            PlayerInventoryTab.Weapons => PlayerInventoryTab.Abilities,
            PlayerInventoryTab.Armor => PlayerInventoryTab.Weapons,
            PlayerInventoryTab.Items => PlayerInventoryTab.Armor,
            _ => PlayerInventoryTab.Items
        };
    }

    private static PlayerInventoryTab NextInventoryTab(PlayerInventoryTab tab)
    {
        return tab switch
        {
            PlayerInventoryTab.Weapons => PlayerInventoryTab.Armor,
            PlayerInventoryTab.Armor => PlayerInventoryTab.Items,
            PlayerInventoryTab.Items => PlayerInventoryTab.Abilities,
            _ => PlayerInventoryTab.Weapons
        };
    }
}
