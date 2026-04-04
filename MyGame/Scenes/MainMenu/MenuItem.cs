namespace MyGame.Scenes.MainMenu;

public sealed class MenuItem
{
    private readonly Func<string> _text;
    private readonly Func<bool>? _isEnabled;

    public MenuItem(string text, Action onSelected, Func<bool>? isEnabled = null)
        : this(() => text, onSelected, isEnabled)
    {
    }

    public MenuItem(Func<string> text, Action onSelected, Func<bool>? isEnabled = null)
    {
        _text = text;
        OnSelected = onSelected;
        _isEnabled = isEnabled;
    }

    public string Text => _text();

    public Action OnSelected { get; }

    public bool IsEnabled => _isEnabled?.Invoke() ?? true;
}
