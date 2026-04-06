using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Core;
using MyGame.Core.Input;

namespace MyGame.Gameplay.Player;

public sealed class PlayerMovementController
{
    private readonly PlayerMovementSettings _settings;

    public PlayerMovementController(PlayerMovementSettings settings)
    {
        _settings = settings;
    }

    public float ContactKnockbackDistance => _settings.ContactKnockbackDistance;

    public float ContactKnockbackSeconds => _settings.ContactKnockbackSeconds;

    public PlayerMovementResult Update(Vector2 position, Direction facing, InputSnapshot input, FrameTime frameTime)
    {
        var movement = Vector2.Zero;
        var nextFacing = facing;

        if (input.IsPressed(GameAction.MoveUp))
        {
            movement.Y -= 1f;
            nextFacing = Direction.Up;
        }

        if (input.IsPressed(GameAction.MoveDown))
        {
            movement.Y += 1f;
            nextFacing = Direction.Down;
        }

        if (input.IsPressed(GameAction.MoveLeft))
        {
            movement.X -= 1f;
            nextFacing = Direction.Left;
        }

        if (input.IsPressed(GameAction.MoveRight))
        {
            movement.X += 1f;
            nextFacing = Direction.Right;
        }

        if (movement != Vector2.Zero)
        {
            movement.Normalize();
        }

        var nextPosition = position + (movement * _settings.MoveSpeed * frameTime.DeltaSeconds);
        return new PlayerMovementResult(nextPosition, nextFacing);
    }
}
