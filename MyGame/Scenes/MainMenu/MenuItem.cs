namespace MyGame.Scenes.MainMenu;

public sealed class MenuItem
{
    public MenuItem(string text, Action onSelected)
    {
        Text = text;
        OnSelected = onSelected;
    }

    public string Text { get; }

    public Action OnSelected { get; }
}
