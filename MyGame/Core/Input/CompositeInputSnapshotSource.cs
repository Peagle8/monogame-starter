namespace MyGame.Core.Input;

public sealed class CompositeInputSnapshotSource : IInputSnapshotSource
{
    private readonly IReadOnlyList<IInputSnapshotSource> _sources;

    public CompositeInputSnapshotSource(IEnumerable<IInputSnapshotSource> sources)
    {
        _sources = sources.ToArray();
    }

    public InputSnapshot ReadCurrent()
    {
        var pressedActions = new HashSet<GameAction>();

        foreach (var source in _sources)
        {
            foreach (var action in source.ReadCurrent().PressedActions)
            {
                pressedActions.Add(action);
            }
        }

        return new InputSnapshot(pressedActions);
    }
}
