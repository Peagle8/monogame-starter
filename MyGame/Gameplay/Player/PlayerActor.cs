using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Input;

namespace MyGame.Gameplay.Player;

public sealed class PlayerActor
{
    private readonly IInputService _inputService;
    private readonly PlayerMovementController _movementController;

    public PlayerActor(IInputService inputService, PlayerMovementController movementController)
    {
        _inputService = inputService;
        _movementController = movementController;
        Position = new Vector2(400f, 240f);
        Facing = Direction.Down;
    }

    public Vector2 Position { get; private set; }

    public Direction Facing { get; private set; }

    public bool IsMoving { get; private set; }

    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, 32, 32);

    public void Update(FrameTime frameTime)
    {
        var previousPosition = Position;
        var result = _movementController.Update(Position, Facing, _inputService.Current, frameTime);
        Position = result.Position;
        Facing = result.Facing;
        IsMoving = Position != previousPosition;
    }
}
