using MyGame.Core.Diagnostics;

namespace MyGame.Core.Input;

public sealed class InputService : IInputService
{
    private readonly IInputSnapshotSource _snapshotSource;
    private readonly GameRecorder _gameRecorder;

    public InputService(IInputSnapshotSource snapshotSource, GameRecorder gameRecorder)
    {
        _snapshotSource = snapshotSource;
        _gameRecorder = gameRecorder;
        Current = InputSnapshot.Empty;
        Previous = InputSnapshot.Empty;
    }

    public InputSnapshot Current { get; private set; }

    public InputSnapshot Previous { get; private set; }

    public void Update()
    {
        Previous = Current;
        var liveSnapshot = _snapshotSource.ReadCurrent();

        if (liveSnapshot.IsPressed(GameAction.Pause) && _gameRecorder.IsReplaying)
        {
            Current = liveSnapshot;
            return;
        }

        Current = _gameRecorder.TryDequeueReplayInput(out var replaySnapshot)
            ? replaySnapshot
            : liveSnapshot;
    }

    public bool IsPressed(GameAction action)
    {
        return Current.IsPressed(action);
    }

    public bool IsJustPressed(GameAction action)
    {
        return Current.IsPressed(action) && !Previous.IsPressed(action);
    }

    public bool IsJustReleased(GameAction action)
    {
        return !Current.IsPressed(action) && Previous.IsPressed(action);
    }
}
