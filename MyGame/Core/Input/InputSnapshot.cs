namespace MyGame.Core.Input;

public sealed record InputSnapshot(IReadOnlySet<GameAction> PressedActions)
{
    public static readonly InputSnapshot Empty = new(new HashSet<GameAction>());

    public bool IsPressed(GameAction action)
    {
        return PressedActions.Contains(action);
    }

    public string ToSummary()
    {
        return PressedActions.Count == 0
            ? "<none>"
            : string.Join(", ", PressedActions.OrderBy(static action => action));
    }
}
