using MyGame.Core.Diagnostics;
using MyGame.Core.Input;

namespace MyGame.Tests.Core.Input;

public sealed class InputServiceTests
{
    [Fact]
    public void Update_UsesSnapshotSource_WhenReplayIsNotActive()
    {
        var recorder = new GameRecorder();
        var source = new StubInputSnapshotSource(
            new InputSnapshot(new HashSet<GameAction> { GameAction.MoveRight }));
        var service = new InputService(source, recorder);

        service.Update();

        Assert.True(service.IsPressed(GameAction.MoveRight));
        Assert.True(service.IsJustPressed(GameAction.MoveRight));
        Assert.Equal(InputSnapshot.Empty, service.Previous);
    }

    [Fact]
    public void Update_UsesReplayFramesBeforeLiveInput()
    {
        var recorder = new GameRecorder();
        recorder.StartReplay(new[]
        {
            CreateFrame(GameAction.MoveLeft),
            CreateFrame(GameAction.Attack)
        });

        var source = new StubInputSnapshotSource(
            new InputSnapshot(new HashSet<GameAction> { GameAction.MoveRight }));
        var service = new InputService(source, recorder);

        service.Update();
        var first = service.Current;

        service.Update();
        var second = service.Current;

        service.Update();
        var third = service.Current;

        Assert.True(first.IsPressed(GameAction.MoveLeft));
        Assert.True(second.IsPressed(GameAction.Attack));
        Assert.True(third.IsPressed(GameAction.MoveRight));
        Assert.False(recorder.IsReplaying);
    }

    [Fact]
    public void Update_UsesLivePauseInput_ToInterruptReplay()
    {
        var recorder = new GameRecorder();
        recorder.StartReplay(new[]
        {
            CreateFrame(GameAction.MoveLeft)
        });

        var source = new StubInputSnapshotSource(
            new InputSnapshot(new HashSet<GameAction> { GameAction.Pause }));
        var service = new InputService(source, recorder);

        service.Update();

        Assert.True(service.IsPressed(GameAction.Pause));
        Assert.False(service.IsPressed(GameAction.MoveLeft));
        Assert.True(recorder.IsReplaying);
    }

    private static RecordedFrame CreateFrame(params GameAction[] actions)
    {
        return new RecordedFrame(
            TimeSpan.Zero,
            "Gameplay",
            new InputSnapshot(actions.ToHashSet()),
            new Dictionary<string, string>());
    }

    private sealed class StubInputSnapshotSource : IInputSnapshotSource
    {
        private readonly Queue<InputSnapshot> _snapshots;

        public StubInputSnapshotSource(params InputSnapshot[] snapshots)
        {
            _snapshots = new Queue<InputSnapshot>(snapshots);
        }

        public InputSnapshot ReadCurrent()
        {
            return _snapshots.Count > 1
                ? _snapshots.Dequeue()
                : _snapshots.Peek();
        }
    }
}
