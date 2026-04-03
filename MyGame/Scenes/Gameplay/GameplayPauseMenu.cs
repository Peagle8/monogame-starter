using MyGame.Core.Input;
using MyGame.Scenes.MainMenu;

namespace MyGame.Scenes.Gameplay;

public sealed class GameplayPauseMenu
{
    private readonly List<MenuItem> _items;
    private bool _skipInputUntilNextUpdate;

    public GameplayPauseMenu(
        Action onResume,
        Action onSaveGame,
        Action onLoadGame,
        Func<bool> canLoadGame,
        Action onReturnToMainMenu)
    {
        _items =
        [
            new MenuItem("Resume", onResume),
            new MenuItem("Save Game", onSaveGame),
            new MenuItem("Load Game", onLoadGame, canLoadGame),
            new MenuItem("Controls", OpenControls),
            new MenuItem("Main Menu", onReturnToMainMenu)
        ];
    }

    public bool IsOpen { get; private set; }

    public bool IsShowingControls { get; private set; }

    public int SelectedIndex { get; private set; }

    public string SelectedText => _items[SelectedIndex].Text;

    public IReadOnlyList<MenuItem> Items => _items;

    public void Open()
    {
        IsOpen = true;
        IsShowingControls = false;
        SelectedIndex = 0;
        _skipInputUntilNextUpdate = true;
    }

    public void Close()
    {
        IsOpen = false;
        IsShowingControls = false;
        SelectedIndex = 0;
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
        }
    }

    private void OpenControls()
    {
        IsShowingControls = true;
    }
}
