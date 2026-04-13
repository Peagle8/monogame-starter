using MyGame.Core.Input;
using MyGame.Infrastructure.Input;

namespace MyGame.Tests.Infrastructure.Input;

public sealed class DefaultGamePadBindingsTests
{
    [Fact]
    public void Create_ReturnsExpectedDefaultBindings()
    {
        var bindings = new DefaultGamePadBindings().Create();

        Assert.Equal([GamePadControl.DPadUp, GamePadControl.LeftStickUp], bindings[GameAction.MoveUp]);
        Assert.Equal([GamePadControl.DPadDown, GamePadControl.LeftStickDown], bindings[GameAction.MoveDown]);
        Assert.Equal([GamePadControl.DPadLeft, GamePadControl.LeftStickLeft], bindings[GameAction.MoveLeft]);
        Assert.Equal([GamePadControl.DPadRight, GamePadControl.LeftStickRight], bindings[GameAction.MoveRight]);
        Assert.Equal([GamePadControl.FaceRight], bindings[GameAction.Interact]);
        Assert.Equal([GamePadControl.FaceLeft], bindings[GameAction.Attack]);
        Assert.Equal([GamePadControl.RightTrigger], bindings[GameAction.RangedAttack]);
        Assert.Equal([GamePadControl.FaceTop], bindings[GameAction.DefenseAbility]);
        Assert.Equal([GamePadControl.RightShoulder], bindings[GameAction.Dash]);
        Assert.Equal([GamePadControl.FaceBottom], bindings[GameAction.Confirm]);
        Assert.Equal([GamePadControl.FaceRight, GamePadControl.Back], bindings[GameAction.Cancel]);
        Assert.Equal([GamePadControl.Start], bindings[GameAction.Pause]);
        Assert.Equal([GamePadControl.Back], bindings[GameAction.Map]);
        Assert.Equal([GamePadControl.LeftShoulder], bindings[GameAction.PreviousTab]);
        Assert.Equal([GamePadControl.RightShoulder], bindings[GameAction.NextTab]);
    }
}
