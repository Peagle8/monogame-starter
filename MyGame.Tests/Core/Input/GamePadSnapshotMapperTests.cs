using MyGame.Core.Input;

namespace MyGame.Tests.Core.Input;

public sealed class GamePadSnapshotMapperTests
{
    [Fact]
    public void Map_UsesDefaultBoundControlsToProduceActions()
    {
        var mapper = new GamePadSnapshotMapper(new Dictionary<GameAction, GamePadControl[]>
        {
            [GameAction.MoveRight] = [GamePadControl.DPadRight, GamePadControl.LeftStickRight],
            [GameAction.Attack] = [GamePadControl.FaceLeft],
            [GameAction.Confirm] = [GamePadControl.FaceBottom]
        });

        var snapshot = new GamePadSnapshot(new HashSet<GamePadControl>
        {
            GamePadControl.LeftStickRight,
            GamePadControl.FaceLeft
        });

        var inputSnapshot = mapper.Map(snapshot);

        Assert.True(inputSnapshot.IsPressed(GameAction.MoveRight));
        Assert.True(inputSnapshot.IsPressed(GameAction.Attack));
        Assert.False(inputSnapshot.IsPressed(GameAction.Confirm));
    }
}
