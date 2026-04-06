using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Core;
using MyGame.Core.Input;
using MyGame.Gameplay.Combat;

namespace MyGame.Gameplay.Player;

public sealed class PlayerActor
{
    private readonly IInputService _inputService;
    private readonly PlayerCombatSettings _combatSettings;
    private readonly PlayerAttackController _attackController;
    private readonly PlayerMovementController _movementController;
    private readonly PlayerDashController _dashController;
    private readonly IPlayerAbilityService _abilityService;
    private readonly KnockbackMotion _knockbackMotion = new();
    private PlayerAttackState _attackState = PlayerAttackState.Idle;
    private PlayerDashState _dashState = PlayerDashState.Idle;

    public PlayerActor(
        IInputService inputService,
        PlayerCombatSettings combatSettings,
        PlayerMovementController movementController,
        PlayerDashController dashController,
        IPlayerAbilityService abilityService,
        PlayerAttackController attackController)
    {
        _inputService = inputService;
        _combatSettings = combatSettings;
        _movementController = movementController;
        _dashController = dashController;
        _abilityService = abilityService;
        _attackController = attackController;
        Position = new Vector2(400f, 240f);
        Facing = Direction.Down;
        CurrentHealth = _combatSettings.MaxHealth;
    }

    public Vector2 Position { get; private set; }

    public Direction Facing { get; private set; }

    public int CurrentHealth { get; private set; }

    public int MaxHealth => _combatSettings.MaxHealth;

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
            UpdateRecoil(frameTime);
            return;
        }

        if (TryUpdateDash(frameTime))
        {
            return;
        }

        UpdateMovement(frameTime);
        UpdateAttack(frameTime, _inputService.IsJustPressed(GameAction.Attack) && !IsDead);
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

    private void UpdateRecoil(FrameTime frameTime)
    {
        Position += _knockbackMotion.Update(frameTime.DeltaSeconds);
        IsMoving = true;
        UpdateAttack(frameTime, attackJustPressed: false);
    }

    private bool TryUpdateDash(FrameTime frameTime)
    {
        var dashResult = _dashController.Update(
            _dashState,
            Position,
            Facing,
            _inputService.Current,
            CanStartDash(),
            frameTime);
        _dashState = dashResult.State;

        if (!_dashState.IsDashing)
        {
            return false;
        }

        Position = dashResult.Position;
        Facing = dashResult.Facing;
        IsMoving = true;
        UpdateAttack(frameTime, attackJustPressed: false);
        return true;
    }

    private bool CanStartDash()
    {
        return _inputService.IsJustPressed(GameAction.Dash)
            && _abilityService.HasAbility(PlayerAbility.Dash)
            && !IsDead
            && !IsAttacking;
    }

    private void UpdateMovement(FrameTime frameTime)
    {
        var previousPosition = Position;
        var result = _movementController.Update(Position, Facing, _inputService.Current, frameTime);
        Position = result.Position;
        Facing = result.Facing;
        IsMoving = Position != previousPosition;
    }

    private void UpdateAttack(FrameTime frameTime, bool attackJustPressed)
    {
        _attackState = _attackController.Update(
            _attackState,
            Position,
            Facing,
            attackJustPressed,
            frameTime);
    }
}
