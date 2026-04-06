namespace MyGame.Core.Input;

public sealed record GamePadSnapshot(IReadOnlySet<GamePadControl> PressedControls)
{
    public static readonly GamePadSnapshot Empty = new(new HashSet<GamePadControl>());

    public bool IsPressed(GamePadControl control)
    {
        return PressedControls.Contains(control);
    }
}
