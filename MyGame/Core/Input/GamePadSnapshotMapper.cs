namespace MyGame.Core.Input;

public sealed class GamePadSnapshotMapper
{
    private readonly IReadOnlyDictionary<GameAction, GamePadControl[]> _bindings;

    public GamePadSnapshotMapper(IReadOnlyDictionary<GameAction, GamePadControl[]> bindings)
    {
        _bindings = bindings;
    }

    public InputSnapshot Map(GamePadSnapshot snapshot)
    {
        var pressedActions = new HashSet<GameAction>();

        foreach (var binding in _bindings)
        {
            if (binding.Value.Any(snapshot.IsPressed))
            {
                pressedActions.Add(binding.Key);
            }
        }

        return new InputSnapshot(pressedActions);
    }
}
