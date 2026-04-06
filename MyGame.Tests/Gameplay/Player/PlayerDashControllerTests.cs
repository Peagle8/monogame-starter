using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Core;
using MyGame.Core.Input;
using MyGame.Gameplay.Player;

namespace MyGame.Tests.Gameplay.Player;

public sealed class PlayerDashControllerTests
{
    [Fact]
    public void Update_WhenDashPressedWithMovementDirection_StartsDashInPressedDirection()
    {
        var controller = new PlayerDashController(new PlayerMovementSettings
        {
            DashDistance = 72f,
            DashSeconds = 0.18f,
            DashCooldownSeconds = 0.35f
        });

        var result = controller.Update(
            PlayerDashState.Idle,
            new Vector2(100f, 100f),
            Direction.Down,
            new InputSnapshot(new HashSet<GameAction> { GameAction.MoveLeft }),
            dashJustPressed: true,
            new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.True(result.State.IsDashing);
        Assert.Equal(Direction.Left, result.Facing);
        Assert.True(result.Position.X < 100f);
    }

    [Fact]
    public void Update_WhenDashPressedWithoutMovement_UsesFacingDirection()
    {
        var controller = new PlayerDashController(new PlayerMovementSettings
        {
            DashDistance = 72f,
            DashSeconds = 0.18f,
            DashCooldownSeconds = 0.35f
        });

        var result = controller.Update(
            PlayerDashState.Idle,
            new Vector2(100f, 100f),
            Direction.Up,
            InputSnapshot.Empty,
            dashJustPressed: true,
            new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.True(result.State.IsDashing);
        Assert.Equal(Direction.Up, result.Facing);
        Assert.True(result.Position.Y < 100f);
    }
}
