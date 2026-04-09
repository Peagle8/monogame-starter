namespace MyGame.Rendering.Menus;

public static class ControlsOverlayText
{
    public static IReadOnlyList<string> Lines { get; } =
    [
        "Move: WASD / arrows / left stick / D-pad",
        "Attack: J / Left Ctrl / X",
        "Interact: E / B",
        "Ranged: K / Left Alt / Right Trigger",
        "Dash: Shift / Right Shoulder",
        "Tabs: Q / R / Left Shoulder / Right Shoulder",
        "Confirm: Enter / Space / A",
        "Back: Esc / B / Back",
        "Pause: Esc / P / Start"
    ];

    public static string HintLineOne => "Press Enter or Esc";

    public static string HintLineTwo => "to return.";
}
