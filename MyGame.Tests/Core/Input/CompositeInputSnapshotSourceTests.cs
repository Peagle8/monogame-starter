using MyGame.Core.Input;

namespace MyGame.Tests.Core.Input;

public sealed class CompositeInputSnapshotSourceTests
{
    [Fact]
    public void ReadCurrent_CombinesActionsFromAllSources()
    {
        var source = new CompositeInputSnapshotSource(
        [
            new StubInputSnapshotSource(new InputSnapshot(new HashSet<GameAction> { GameAction.MoveLeft })),
            new StubInputSnapshotSource(new InputSnapshot(new HashSet<GameAction> { GameAction.Attack }))
        ]);

        var snapshot = source.ReadCurrent();

        Assert.True(snapshot.IsPressed(GameAction.MoveLeft));
        Assert.True(snapshot.IsPressed(GameAction.Attack));
    }

    private sealed class StubInputSnapshotSource : IInputSnapshotSource
    {
        private readonly InputSnapshot _snapshot;

        public StubInputSnapshotSource(InputSnapshot snapshot)
        {
            _snapshot = snapshot;
        }

        public InputSnapshot ReadCurrent()
        {
            return _snapshot;
        }
    }
}
