using Microsoft.Xna.Framework.Input;
using MyGame.Core.Input;
using MyGame.Infrastructure.Input;

namespace MyGame.Tests.Infrastructure.Input;

public sealed class DefaultInputBindingsTests
{
    [Fact]
    public void Create_ReturnsExpectedDefaultBindings()
    {
        var bindings = new DefaultInputBindings().Create();

        Assert.Equal([Keys.W, Keys.Up], bindings[GameAction.MoveUp]);
        Assert.Equal([Keys.S, Keys.Down], bindings[GameAction.MoveDown]);
        Assert.Equal([Keys.A, Keys.Left], bindings[GameAction.MoveLeft]);
        Assert.Equal([Keys.D, Keys.Right], bindings[GameAction.MoveRight]);
        Assert.Equal([Keys.E], bindings[GameAction.Interact]);
        Assert.Equal([Keys.J, Keys.LeftControl], bindings[GameAction.Attack]);
        Assert.Equal([Keys.K, Keys.LeftAlt], bindings[GameAction.RangedAttack]);
        Assert.Equal([Keys.L, Keys.RightControl], bindings[GameAction.DefenseAbility]);
        Assert.Equal([Keys.LeftShift, Keys.RightShift], bindings[GameAction.Dash]);
        Assert.Equal([Keys.Enter, Keys.Space], bindings[GameAction.Confirm]);
        Assert.Equal([Keys.Escape, Keys.Back], bindings[GameAction.Cancel]);
        Assert.Equal([Keys.P, Keys.Escape], bindings[GameAction.Pause]);
        Assert.Equal([Keys.Tab, Keys.M], bindings[GameAction.Map]);
        Assert.Equal([Keys.Q], bindings[GameAction.PreviousTab]);
        Assert.Equal([Keys.R], bindings[GameAction.NextTab]);
    }
}
