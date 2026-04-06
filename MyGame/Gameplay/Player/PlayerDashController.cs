using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Core;
using MyGame.Core.Input;

namespace MyGame.Gameplay.Player;

public sealed class PlayerDashController
{
    private readonly PlayerMovementSettings _settings;

    public PlayerDashController(PlayerMovementSettings settings)
    {
        _settings = settings;
    }

    public PlayerDashUpdateResult Update(
        PlayerDashState currentState,
        Vector2 position,
        Direction facing,
        InputSnapshot input,
        bool dashJustPressed,
        FrameTime frameTime)
    {
        var remainingActiveSeconds = Math.Max(0f, currentState.RemainingActiveSeconds - frameTime.DeltaSeconds);
        var remainingCooldownSeconds = Math.Max(0f, currentState.RemainingCooldownSeconds - frameTime.DeltaSeconds);
        var isDashing = remainingActiveSeconds > 0f;
        var dashDirection = currentState.Direction;
        var nextPosition = position;
        var nextFacing = facing;
        var dashSequence = currentState.DashSequence;

        if (!isDashing && dashJustPressed && remainingCooldownSeconds <= 0f)
        {
            dashDirection = ResolveDashDirection(facing, input);
            nextFacing = dashDirection;
            dashSequence++;
            remainingActiveSeconds = _settings.DashSeconds;
            remainingCooldownSeconds = _settings.DashCooldownSeconds;
            isDashing = remainingActiveSeconds > 0f;
        }

        if (isDashing)
        {
            nextFacing = dashDirection;
            var dashVelocity = DirectionHelper.ToVector(dashDirection) * (_settings.DashDistance / _settings.DashSeconds);
            nextPosition += dashVelocity * frameTime.DeltaSeconds;
        }

        return new PlayerDashUpdateResult(
            nextPosition,
            nextFacing,
            new PlayerDashState(
                isDashing,
                dashDirection,
                dashSequence,
                remainingActiveSeconds,
                remainingCooldownSeconds));
    }

    private static Direction ResolveDashDirection(Direction facing, InputSnapshot input)
    {
        var movement = Vector2.Zero;

        if (input.IsPressed(GameAction.MoveUp))
        {
            movement.Y -= 1f;
        }

        if (input.IsPressed(GameAction.MoveDown))
        {
            movement.Y += 1f;
        }

        if (input.IsPressed(GameAction.MoveLeft))
        {
            movement.X -= 1f;
        }

        if (input.IsPressed(GameAction.MoveRight))
        {
            movement.X += 1f;
        }

        if (movement == Vector2.Zero)
        {
            return facing;
        }

        return DirectionHelper.FromDominantAxis(movement, facing);
    }
}
