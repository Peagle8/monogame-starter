namespace MyGame.Rendering.Menus;

public static class ControlsOverlayText
{
    public static IReadOnlyList<string> Lines { get; } =
    [
        "Move: WASD / arrow keys",
        "Attack: J / Left Ctrl",
        "Confirm: Enter",
        "Back: Esc",
        "Pause: Esc / P"
    ];

    public static string HintLineOne => "Press Enter or Esc";

    public static string HintLineTwo => "to return.";
}
