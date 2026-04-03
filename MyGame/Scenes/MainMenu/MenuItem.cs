namespace MyGame.Scenes.MainMenu;

public sealed class MenuItem
{
    private readonly Func<bool>? _isEnabled;

    public MenuItem(string text, Action onSelected, Func<bool>? isEnabled = null)
    {
        Text = text;
        OnSelected = onSelected;
        _isEnabled = isEnabled;
    }

    public string Text { get; }

    public Action OnSelected { get; }

    public bool IsEnabled => _isEnabled?.Invoke() ?? true;
}
