using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Core;
using MyGame.Gameplay.Player;

namespace MyGame.Tests.Gameplay.Player;

public sealed class PlayerAttackControllerTests
{
    [Fact]
    public void Update_WhenAttackPressed_StartsAttackWithForwardHitbox()
    {
        var controller = new PlayerAttackController(new PlayerAttackSettings());

        var state = controller.Update(
            PlayerAttackState.Idle,
            new Vector2(100f, 200f),
            Direction.Right,
            attackJustPressed: true,
            new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.True(state.IsAttacking);
        Assert.Equal(1, state.AttackSequence);
        Assert.Equal(new Rectangle(132, 200, 30, 32), state.AttackBounds);
    }

    [Fact]
    public void Update_WhenAttackStillActive_KeepsExistingAttackBounds()
    {
        var controller = new PlayerAttackController(new PlayerAttackSettings());
        var attackState = controller.Update(
            PlayerAttackState.Idle,
            new Vector2(100f, 200f),
            Direction.Up,
            attackJustPressed: true,
            new FrameTime(TimeSpan.FromSeconds(0.01), TimeSpan.FromSeconds(0.01)));

        var updatedState = controller.Update(
            attackState,
            new Vector2(140f, 200f),
            Direction.Left,
            attackJustPressed: false,
            new FrameTime(TimeSpan.FromSeconds(0.05), TimeSpan.FromSeconds(0.06)));

        Assert.True(updatedState.IsAttacking);
        Assert.Equal(attackState.AttackBounds, updatedState.AttackBounds);
    }

    [Fact]
    public void Update_WhenCooldownExpires_AllowsNextAttack()
    {
        var controller = new PlayerAttackController(new PlayerAttackSettings
        {
            ActiveSeconds = 0.1f,
            CooldownSeconds = 0.2f,
            Range = 20
        });

        var state = controller.Update(
            PlayerAttackState.Idle,
            new Vector2(100f, 100f),
            Direction.Down,
            attackJustPressed: true,
            new FrameTime(TimeSpan.FromSeconds(0.01), TimeSpan.FromSeconds(0.01)));
        state = controller.Update(
            state,
            new Vector2(100f, 100f),
            Direction.Down,
            attackJustPressed: false,
            new FrameTime(TimeSpan.FromSeconds(0.25), TimeSpan.FromSeconds(0.26)));
        state = controller.Update(
            state,
            new Vector2(100f, 100f),
            Direction.Left,
            attackJustPressed: true,
            new FrameTime(TimeSpan.FromSeconds(0.01), TimeSpan.FromSeconds(0.27)));

        Assert.True(state.IsAttacking);
        Assert.Equal(2, state.AttackSequence);
        Assert.Equal(new Rectangle(80, 100, 20, 32), state.AttackBounds);
    }
}
