using MyGame.Core.Input;
using MyGame.Gameplay.Inventory;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.World;
using MyGame.Infrastructure.Save;
using MyGame.Scenes.MainMenu;

namespace MyGame.Scenes.Gameplay;

public sealed class GameplayPauseMenu
{
    private const int UpgradeNodesPerPage = 4;
    private readonly List<MenuItem> _items;
    private readonly List<MenuItem> _replayItems;
    private readonly Func<OverworldMapSnapshot> _createMapSnapshot;
    private readonly PlayerActor _player;
    private bool _skipInputUntilNextUpdate;

    public GameplayPauseMenu(
        PlayerActor player,
        Action onResume,
        Action onSaveGame,
        Action onLoadGame,
        Func<bool> canLoadGame,
        bool showReplayMenu,
        Func<string> recordingToggleText,
        Action onToggleRecording,
        Action onReplayLastRecording,
        Func<bool> canReplayRecording,
        bool canShowMap,
        Func<OverworldMapSnapshot> createMapSnapshot,
        Action onReturnToMainMenu)
    {
        _player = player;
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

        CanShowMap = canShowMap;
        _createMapSnapshot = createMapSnapshot;
    }

    public static GameplayPauseMenu CreateGameplayMenu(
        PlayerActor player,
        ISaveGameService saveGameService,
        Core.Diagnostics.GameRecorder gameRecorder,
        Configuration.DiagnosticsSettings diagnosticsSettings,
        string sceneName,
        Func<SaveGameData> createSaveData,
        Action<SaveGameData> applySaveData,
        Func<OverworldMapSnapshot> createMapSnapshot,
        Action onRestart,
        Action onReturnToMainMenu)
    {
        GameplayPauseMenu? pauseMenu = null;
        pauseMenu = new GameplayPauseMenu(
            player,
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
            canShowMap: OverworldLayoutMetrics.IsOverworldScene(sceneName),
            createMapSnapshot: createMapSnapshot,
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

    public bool IsShowingMap { get; private set; }

    public bool CanShowMap { get; }

    public string? StatusMessage { get; private set; }

    public int SelectedIndex { get; private set; }

    public string SelectedText => _items[SelectedIndex].Text;

    public IReadOnlyList<MenuItem> Items => _items;

    public int ReplaySelectedIndex { get; private set; }

    public string ReplaySelectedText => _replayItems[ReplaySelectedIndex].Text;

    public IReadOnlyList<MenuItem> ReplayItems => _replayItems;

    public PlayerInventoryTab InventoryTab { get; private set; } = PlayerInventoryTab.Weapons;

    public string FooterText => StatusMessage ?? GetFooterText();

    public OverworldMapSnapshot MapSnapshot => _createMapSnapshot();

    public PlayerActor Player => _player;

    public AbilityMenuView AbilityMenuView { get; private set; } = AbilityMenuView.SlotList;

    public int SelectedAbilitySlotIndex { get; private set; }

    public int SelectedAbilityActionIndex { get; private set; }

    public int SelectedAbilityOptionIndex { get; private set; }

    public int UpgradePageIndex { get; private set; }

    public AbilityLoadoutSlot SelectedAbilitySlot => AbilityLoadoutCatalog.OrderedSlots[SelectedAbilitySlotIndex];

    public AbilityMenuAction SelectedAbilityAction => AbilityLoadoutCatalog.MenuActions[SelectedAbilityActionIndex];

    public IReadOnlyList<AbilitySummaryEntry> AbilitySummary => AbilityLoadoutCatalog.CreateSummary(_player);

    public IReadOnlyList<AbilityMenuOptionViewModel> AbilityOptions => AbilityLoadoutCatalog.CreateOptionViewModels(_player, SelectedAbilitySlot);

    public int UpgradePageCount => Math.Max(1, (int)Math.Ceiling(AbilityLoadoutCatalog.GetEntries(SelectedAbilitySlot).Count / (double)UpgradeNodesPerPage));

    public void Open()
    {
        IsOpen = true;
        IsShowingControls = false;
        IsShowingInventoryMenu = false;
        IsShowingReplayMenu = false;
        IsShowingMap = false;
        InventoryTab = PlayerInventoryTab.Weapons;
        SelectedIndex = 0;
        ReplaySelectedIndex = 0;
        StatusMessage = null;
        ResetAbilityMenuState();
        _skipInputUntilNextUpdate = true;
    }

    public void Close()
    {
        IsOpen = false;
        IsShowingControls = false;
        IsShowingInventoryMenu = false;
        IsShowingReplayMenu = false;
        IsShowingMap = false;
        InventoryTab = PlayerInventoryTab.Weapons;
        SelectedIndex = 0;
        ReplaySelectedIndex = 0;
        StatusMessage = null;
        ResetAbilityMenuState();
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

    public void ToggleMap()
    {
        if (!CanShowMap)
        {
            return;
        }

        if (IsOpen && IsShowingMap)
        {
            Close();
            return;
        }

        Open();
        IsShowingMap = true;
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

        if (IsShowingMap)
        {
            if (inputService.IsJustPressed(GameAction.Cancel)
                || inputService.IsJustPressed(GameAction.Pause)
                || inputService.IsJustPressed(GameAction.Map))
            {
                Close();
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
        ResetAbilityMenuState();
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
        ResetAbilityMenuState();
    }

    private void UpdateInventoryMenu(IInputService inputService)
    {
        if (inputService.IsJustPressed(GameAction.Pause))
        {
            CloseInventoryMenu();
            return;
        }

        if (inputService.IsJustPressed(GameAction.Cancel) && HandleAbilityCancel())
        {
            return;
        }

        if (CanSwitchInventoryTabs() && inputService.IsJustPressed(GameAction.PreviousTab))
        {
            InventoryTab = PreviousInventoryTab(InventoryTab);
            ResetAbilityMenuState();
        }

        if (CanSwitchInventoryTabs() && inputService.IsJustPressed(GameAction.NextTab))
        {
            InventoryTab = NextInventoryTab(InventoryTab);
            ResetAbilityMenuState();
        }

        if (InventoryTab == PlayerInventoryTab.Abilities)
        {
            UpdateAbilitiesMenu(inputService);
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

        if (IsShowingMap)
        {
            return "Survey the overworld ring and your current position.";
        }

        if (IsShowingInventoryMenu)
        {
            if (InventoryTab == PlayerInventoryTab.Abilities)
            {
                return GetAbilitiesFooterText();
            }

            return InventoryTab switch
            {
                PlayerInventoryTab.Weapons => "Equip and compare your currently owned weapons.",
                PlayerInventoryTab.Armor => "Review armor pieces once defensive gear is added.",
                PlayerInventoryTab.Items => "Consumables and key items will appear here later.",
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

    private void UpdateAbilitiesMenu(IInputService inputService)
    {
        switch (AbilityMenuView)
        {
            case AbilityMenuView.SlotList:
                UpdateAbilitySlotList(inputService);
                return;
            case AbilityMenuView.ActionList:
                UpdateAbilityActionList(inputService);
                return;
            case AbilityMenuView.EquipList:
                UpdateAbilityEquipList(inputService);
                return;
            case AbilityMenuView.UpgradeView:
                UpdateAbilityUpgradeView(inputService);
                return;
        }
    }

    private void UpdateAbilitySlotList(IInputService inputService)
    {
        if (inputService.IsJustPressed(GameAction.MoveDown))
        {
            SelectedAbilitySlotIndex = (SelectedAbilitySlotIndex + 1) % AbilityLoadoutCatalog.OrderedSlots.Count;
        }

        if (inputService.IsJustPressed(GameAction.MoveUp))
        {
            SelectedAbilitySlotIndex = (SelectedAbilitySlotIndex - 1 + AbilityLoadoutCatalog.OrderedSlots.Count) % AbilityLoadoutCatalog.OrderedSlots.Count;
        }

        if (inputService.IsJustPressed(GameAction.Confirm))
        {
            AbilityMenuView = AbilityMenuView.ActionList;
            SelectedAbilityActionIndex = 0;
        }
    }

    private void UpdateAbilityActionList(IInputService inputService)
    {
        if (inputService.IsJustPressed(GameAction.MoveDown))
        {
            SelectedAbilityActionIndex = (SelectedAbilityActionIndex + 1) % AbilityLoadoutCatalog.MenuActions.Count;
        }

        if (inputService.IsJustPressed(GameAction.MoveUp))
        {
            SelectedAbilityActionIndex = (SelectedAbilityActionIndex - 1 + AbilityLoadoutCatalog.MenuActions.Count) % AbilityLoadoutCatalog.MenuActions.Count;
        }

        if (!inputService.IsJustPressed(GameAction.Confirm))
        {
            return;
        }

        switch (SelectedAbilityAction)
        {
            case AbilityMenuAction.Equip:
                AbilityMenuView = AbilityMenuView.EquipList;
                SelectedAbilityOptionIndex = 0;
                return;
            case AbilityMenuAction.ViewUpgrades:
                AbilityMenuView = AbilityMenuView.UpgradeView;
                UpgradePageIndex = 0;
                return;
        }
    }

    private void UpdateAbilityEquipList(IInputService inputService)
    {
        var optionCount = AbilityOptions.Count;
        if (optionCount == 0)
        {
            return;
        }

        if (inputService.IsJustPressed(GameAction.MoveDown))
        {
            SelectedAbilityOptionIndex = (SelectedAbilityOptionIndex + 1) % optionCount;
        }

        if (inputService.IsJustPressed(GameAction.MoveUp))
        {
            SelectedAbilityOptionIndex = (SelectedAbilityOptionIndex - 1 + optionCount) % optionCount;
        }

        if (!inputService.IsJustPressed(GameAction.Confirm))
        {
            return;
        }

        var entry = AbilityLoadoutCatalog.GetEntries(SelectedAbilitySlot)[SelectedAbilityOptionIndex];
        if (!AbilityOptions[SelectedAbilityOptionIndex].IsEnabled)
        {
            return;
        }

        AbilityLoadoutCatalog.Equip(_player, entry);
        StatusMessage = $"{AbilityLoadoutCatalog.GetSlotLabel(SelectedAbilitySlot)} equipped: {entry.DisplayName}";
    }

    private void UpdateAbilityUpgradeView(IInputService inputService)
    {
        if (inputService.IsJustPressed(GameAction.PreviousTab))
        {
            UpgradePageIndex = (UpgradePageIndex - 1 + UpgradePageCount) % UpgradePageCount;
            return;
        }

        if (inputService.IsJustPressed(GameAction.NextTab))
        {
            UpgradePageIndex = (UpgradePageIndex + 1) % UpgradePageCount;
            return;
        }

        if (inputService.IsJustPressed(GameAction.Confirm))
        {
            AbilityMenuView = AbilityMenuView.ActionList;
        }
    }

    private bool HandleAbilityCancel()
    {
        if (InventoryTab != PlayerInventoryTab.Abilities)
        {
            CloseInventoryMenu();
            return true;
        }

        switch (AbilityMenuView)
        {
            case AbilityMenuView.SlotList:
                CloseInventoryMenu();
                return true;
            case AbilityMenuView.ActionList:
                AbilityMenuView = AbilityMenuView.SlotList;
                return true;
            case AbilityMenuView.EquipList:
            case AbilityMenuView.UpgradeView:
                AbilityMenuView = AbilityMenuView.ActionList;
                return true;
            default:
                return false;
        }
    }

    private bool CanSwitchInventoryTabs()
    {
        return InventoryTab != PlayerInventoryTab.Abilities || AbilityMenuView == AbilityMenuView.SlotList;
    }

    private string GetAbilitiesFooterText()
    {
        return AbilityMenuView switch
        {
            AbilityMenuView.SlotList => "Choose a loadout slot to equip abilities or inspect its placeholder upgrade tree.",
            AbilityMenuView.ActionList => $"Choose whether to equip a {AbilityLoadoutCatalog.GetSlotLabel(SelectedAbilitySlot).ToLowerInvariant()} ability or inspect its upgrade path.",
            AbilityMenuView.EquipList => GetAbilityEquipFooterText(),
            AbilityMenuView.UpgradeView => $"Preview the placeholder branch layout for this slot. Page {UpgradePageIndex + 1}/{UpgradePageCount}. LB/RB changes pages.",
            _ => "Browse your loadout."
        };
    }

    private string GetAbilityEquipFooterText()
    {
        var option = AbilityOptions[SelectedAbilityOptionIndex];
        if (option.IsEnabled)
        {
            return option.IsEquipped
                ? $"{option.DisplayName} is currently equipped."
                : $"Equip {option.DisplayName} to the {AbilityLoadoutCatalog.GetSlotLabel(SelectedAbilitySlot).ToLowerInvariant()} slot.";
        }

        if (option.IsUnlocked)
        {
            return $"{option.DisplayName} is visible, but its gameplay behavior is still a placeholder.";
        }

        return $"{option.DisplayName} is planned for a later unlock and is not available yet.";
    }

    private void ResetAbilityMenuState()
    {
        AbilityMenuView = AbilityMenuView.SlotList;
        SelectedAbilitySlotIndex = 0;
        SelectedAbilityActionIndex = 0;
        SelectedAbilityOptionIndex = 0;
        UpgradePageIndex = 0;
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
