using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Input;
using MyGame.Gameplay.Combat;

namespace MyGame.Gameplay.Player;

public sealed class PlayerActor
{
    // TODO: move this into config so it's not a magic number
    private const int DefaultMaxHealth = 5;

    private readonly IInputService _inputService;
    private readonly PlayerAttackController _attackController;
    private readonly PlayerMovementController _movementController;
    private readonly PlayerDashController _dashController;
    private readonly IPlayerAbilityService _abilityService;
    private readonly KnockbackMotion _knockbackMotion = new();
    private PlayerAttackState _attackState = PlayerAttackState.Idle;
    private PlayerDashState _dashState = PlayerDashState.Idle;

    public PlayerActor(
        IInputService inputService,
        PlayerMovementController movementController,
        PlayerDashController dashController,
        IPlayerAbilityService abilityService,
        PlayerAttackController attackController)
    {
        _inputService = inputService;
        _movementController = movementController;
        _dashController = dashController;
        _abilityService = abilityService;
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

    public bool IsRecoiling => _knockbackMotion.IsActive;

    public bool IsDashing => _dashState.IsDashing;

    public bool IsDead => CurrentHealth <= 0;

    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, 32, 32);

    public void Update(FrameTime frameTime)
    {
        if (_knockbackMotion.IsActive)
        {
            Position += _knockbackMotion.Update(frameTime.DeltaSeconds);
            IsMoving = true;
            _attackState = _attackController.Update(
                _attackState,
                Position,
                Facing,
                attackJustPressed: false,
                frameTime);
            return;
        }

        // TODO: these at least belong in their own method... one method per action maybe, however might be better as a player action service?
        var dashResult = _dashController.Update(
            _dashState,
            Position,
            Facing,
            _inputService.Current,
            // TODO: When we have a lot of abilities repeating this pattern will be gross... lets make this common once we have say four abilities if we see this same pattern in multiple
            _inputService.IsJustPressed(GameAction.Dash)
                && _abilityService.HasAbility(PlayerAbility.Dash)
                && !IsDead
                && !IsAttacking,
            frameTime);
        _dashState = dashResult.State;

        if (_dashState.IsDashing)
        {
            Position = dashResult.Position;
            Facing = dashResult.Facing;
            IsMoving = true;
            _attackState = _attackController.Update(
                _attackState,
                Position,
                Facing,
                attackJustPressed: false,
                frameTime);
            return;
        }

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

    public void ApplyKnockback(Vector2 direction)
    {
        if (IsDead)
        {
            return;
        }

        Position += _knockbackMotion.Begin(
            direction,
            _movementController.ContactKnockbackDistance,
            _movementController.ContactKnockbackSeconds);
        _dashState = PlayerDashState.Idle;
        IsMoving = _knockbackMotion.IsActive;
    }

    public void RestoreState(Vector2 position, int currentHealth)
    {
        Position = position;
        CurrentHealth = Math.Clamp(currentHealth, 0, MaxHealth);
        IsMoving = false;
        _attackState = PlayerAttackState.Idle;
        _dashState = PlayerDashState.Idle;
        _knockbackMotion.Reset();
    }
}
