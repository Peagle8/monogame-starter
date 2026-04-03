using MyGame.Core.Input;

namespace MyGame.Tests.Core.Input;

public sealed class InputSnapshotTests
{
    [Fact]
    public void ToSummary_ReturnsNone_WhenNoActionsArePressed()
    {
        var snapshot = InputSnapshot.Empty;

        var result = snapshot.ToSummary();

        Assert.Equal("<none>", result);
    }

    [Fact]
    public void IsPressed_ReturnsTrue_WhenActionIsPresent()
    {
        var snapshot = new InputSnapshot(new HashSet<GameAction> { GameAction.Confirm });

        var result = snapshot.IsPressed(GameAction.Confirm);

        Assert.True(result);
    }
}
