using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Input;

namespace MyGame.Gameplay.Player;

public sealed class PlayerActor
{
    // TODO: move this into config so it's not a magic number
    private const int DefaultMaxHealth = 5;

    private readonly IInputService _inputService;
    private readonly PlayerAttackController _attackController;
    private readonly PlayerMovementController _movementController;
    private PlayerAttackState _attackState = PlayerAttackState.Idle;

    public PlayerActor(
        IInputService inputService,
        PlayerMovementController movementController,
        PlayerAttackController attackController)
    {
        _inputService = inputService;
        _movementController = movementController;
        _attackController = attackController;
        Position = new Vector2(400f, 240f);
        Facing = Direction.Down;
        CurrentHealth = DefaultMaxHealth;
    }

    public Vector2 Position { get; private set; }

    public Direction Facing { get; private set; }

    public int CurrentHealth { get; private set; }

    public int MaxHealth => DefaultMaxHealth;

    public int AttackDamage => _attackController.Damage;

    public Rectangle? AttackBounds => _attackState.AttackBounds;

    public int AttackSequence => _attackState.AttackSequence;

    public bool IsAttacking => _attackState.IsAttacking;

    public bool IsMoving { get; private set; }

    public bool IsDead => CurrentHealth <= 0;

    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, 32, 32);

    public void Update(FrameTime frameTime)
    {
        var previousPosition = Position;
        var result = _movementController.Update(Position, Facing, _inputService.Current, frameTime);
        Position = result.Position;
        Facing = result.Facing;
        IsMoving = Position != previousPosition;
        _attackState = _attackController.Update(
            _attackState,
            Position,
            Facing,
            _inputService.IsJustPressed(GameAction.Attack) && !IsDead,
            frameTime);
    }

    public void TakeDamage(int amount)
    {
        CurrentHealth = Math.Max(0, CurrentHealth - amount);
    }

    public void RestoreState(Vector2 position, int currentHealth)
    {
        Position = position;
        CurrentHealth = Math.Clamp(currentHealth, 0, MaxHealth);
        IsMoving = false;
        _attackState = PlayerAttackState.Idle;
    }
}
