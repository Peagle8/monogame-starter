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
    private readonly PlayerDefenseAbilityController _defenseAbilityController;
    private readonly PlayerRangedAttackController _rangedAttackController;
    private readonly PlayerMovementController _movementController;
    private readonly PlayerDashController _dashController;
    private readonly PlayerBombDashController _bombDashController;
    private readonly IPlayerAbilityService _abilityService;
    private readonly KnockbackMotion _knockbackMotion = new();
    private readonly List<PlayerProjectile> _spawnedProjectiles = [];
    private readonly List<PlayerBomb> _spawnedBombs = [];
    private PlayerAttackState _attackState = PlayerAttackState.Idle;
    private PlayerDashAbilityKind _equippedDashAbility = PlayerDashAbilityKind.BaseDash;
    private PlayerDefenseAbilityState _defenseAbilityState = PlayerDefenseAbilityState.Default;
    private PlayerRangedAttackState _rangedAttackState = PlayerRangedAttackState.Default;
    private PlayerDashState _dashState = PlayerDashState.Idle;
    private PlayerBombTrailState _bombTrailState = PlayerBombTrailState.Default;
    private PlayerMeleeAbilityKind _equippedMeleeAbility = PlayerMeleeAbilityKind.BaseAttack;
    private float _remainingStunSeconds;

    public PlayerActor(
        IInputService inputService,
        PlayerCombatSettings combatSettings,
        PlayerMovementController movementController,
        PlayerDashController dashController,
        IPlayerAbilityService abilityService,
        PlayerAttackController attackController)
        : this(
            inputService,
            combatSettings,
            movementController,
            dashController,
            abilityService,
            attackController,
            new PlayerDefenseAbilityController(new PlayerDefenseAbilitySettings()),
            new PlayerRangedAttackController(new PlayerRangedAttackSettings()),
            new PlayerBombDashController())
    {
    }

    public PlayerActor(
        IInputService inputService,
        PlayerCombatSettings combatSettings,
        PlayerMovementController movementController,
        PlayerDashController dashController,
        IPlayerAbilityService abilityService,
        PlayerAttackController attackController,
        PlayerRangedAttackController rangedAttackController)
        : this(
            inputService,
            combatSettings,
            movementController,
            dashController,
            abilityService,
            attackController,
            new PlayerDefenseAbilityController(new PlayerDefenseAbilitySettings()),
            rangedAttackController,
            new PlayerBombDashController())
    {
    }

    public PlayerActor(
        IInputService inputService,
        PlayerCombatSettings combatSettings,
        PlayerMovementController movementController,
        PlayerDashController dashController,
        IPlayerAbilityService abilityService,
        PlayerAttackController attackController,
        PlayerDefenseAbilityController defenseAbilityController,
        PlayerRangedAttackController rangedAttackController)
        : this(
            inputService,
            combatSettings,
            movementController,
            dashController,
            abilityService,
            attackController,
            defenseAbilityController,
            rangedAttackController,
            new PlayerBombDashController())
    {
    }

    public PlayerActor(
        IInputService inputService,
        PlayerCombatSettings combatSettings,
        PlayerMovementController movementController,
        PlayerDashController dashController,
        IPlayerAbilityService abilityService,
        PlayerAttackController attackController,
        PlayerDefenseAbilityController defenseAbilityController,
        PlayerRangedAttackController rangedAttackController,
        PlayerBombDashController bombDashController)
    {
        _inputService = inputService;
        _combatSettings = combatSettings;
        _movementController = movementController;
        _dashController = dashController;
        _abilityService = abilityService;
        _attackController = attackController;
        _defenseAbilityController = defenseAbilityController;
        _rangedAttackController = rangedAttackController;
        _bombDashController = bombDashController;
        Position = new Vector2(400f, 240f);
        PreviousPosition = Position;
        Facing = Direction.Down;
        CurrentHealth = _combatSettings.MaxHealth;
        CurrentAbilityPoints = _combatSettings.MaxAbilityPoints;
    }

    public Vector2 Position { get; private set; }

    public Vector2 PreviousPosition { get; private set; }

    public Direction Facing { get; private set; }

    public int CurrentHealth { get; private set; }

    public int MaxHealth => _combatSettings.MaxHealth;

    public float CurrentAbilityPoints { get; private set; }

    public float MaxAbilityPoints => _combatSettings.MaxAbilityPoints;

    public int AttackDamage => _attackController.Damage;

    public Rectangle? AttackBounds => _attackState.AttackBounds;

    public int AttackSequence => _attackState.AttackSequence;

    public PlayerDefenseAbilityKind EquippedDefenseAbility => _defenseAbilityState.EquippedAbility;

    public PlayerRangedAttackKind EquippedRangedAttack => _rangedAttackState.EquippedAttack;

    public PlayerDashAbilityKind EquippedDashAbility => _equippedDashAbility;

    public PlayerMeleeAbilityKind EquippedMeleeAbility => _equippedMeleeAbility;

    public bool IsAttacking => _attackState.IsAttacking;

    public bool IsShieldActive =>
        _defenseAbilityState.EquippedAbility == PlayerDefenseAbilityKind.Shield
        && _defenseAbilityState.IsActive;

    public int ShieldCharges => _defenseAbilityState.RemainingCharges;

    public bool IsMoving { get; private set; }

    public bool IsRecoiling => _knockbackMotion.IsActive;

    public bool IsDashing => _dashState.IsDashing;

    public bool IsDead => CurrentHealth <= 0;

    public bool IsStunned => _remainingStunSeconds > 0f;

    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, 32, 32);

    public Rectangle PreviousBounds => new((int)PreviousPosition.X, (int)PreviousPosition.Y, 32, 32);

    public void Update(FrameTime frameTime)
    {
        PreviousPosition = Position;
        RegenerateAbilityPoints(frameTime);

        if (_knockbackMotion.IsActive)
        {
            UpdateRecoil(frameTime);
            return;
        }

        if (IsStunned)
        {
            UpdateStun(frameTime);
            return;
        }

        if (TryUpdateDash(frameTime))
        {
            return;
        }

        UpdateMovement(frameTime);
        UpdateAttack(frameTime, _inputService.IsJustPressed(GameAction.Attack) && !IsDead);
        UpdateDefenseAbility(_inputService.IsJustPressed(GameAction.DefenseAbility) && !IsDead);
        UpdateRangedAttack(frameTime, _inputService.IsJustPressed(GameAction.RangedAttack) && !IsDead);
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
        _bombTrailState = PlayerBombTrailState.Default;
        IsMoving = _knockbackMotion.IsActive;
    }

    public void ApplyStun(float seconds)
    {
        if (IsDead || seconds <= 0f)
        {
            return;
        }

        _remainingStunSeconds = Math.Max(_remainingStunSeconds, seconds);
        _dashState = PlayerDashState.Idle;
        _bombTrailState = PlayerBombTrailState.Default;
        IsMoving = false;
    }

    public void RestoreState(Vector2 position, int currentHealth)
    {
        RestoreState(position, currentHealth, MaxAbilityPoints);
    }

    public PlayerTransitionState CreateTransitionState()
    {
        return new PlayerTransitionState(
            CurrentHealth,
            CurrentAbilityPoints,
            Facing,
            _equippedDashAbility,
            _defenseAbilityState,
            _rangedAttackState,
            _equippedMeleeAbility);
    }

    public void ApplyTransitionState(Vector2 position, PlayerTransitionState state)
    {
        Position = position;
        PreviousPosition = position;
        Facing = state.Facing;
        CurrentHealth = Math.Clamp(state.CurrentHealth, 0, MaxHealth);
        CurrentAbilityPoints = MathHelper.Clamp(state.CurrentAbilityPoints, 0f, MaxAbilityPoints);
        _equippedDashAbility = state.EquippedDashAbility;
        IsMoving = false;
        _attackState = PlayerAttackState.Idle;
        _defenseAbilityState = state.DefenseAbilityState;
        _rangedAttackState = state.RangedAttackState;
        _equippedMeleeAbility = state.EquippedMeleeAbility;
        _dashState = PlayerDashState.Idle;
        _bombTrailState = PlayerBombTrailState.Default;
        _remainingStunSeconds = 0f;
        _knockbackMotion.Reset();
        _spawnedBombs.Clear();
        _spawnedProjectiles.Clear();
    }

    public void RestoreState(Vector2 position, int currentHealth, float currentAbilityPoints)
    {
        ApplyTransitionState(
            position,
            new PlayerTransitionState(
                currentHealth,
                currentAbilityPoints,
                Facing,
                PlayerDashAbilityKind.BaseDash,
                PlayerDefenseAbilityState.Default,
                PlayerRangedAttackState.Default,
                PlayerMeleeAbilityKind.BaseAttack));
    }

    public void AddAbilityPoints(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        CurrentAbilityPoints = MathHelper.Clamp(CurrentAbilityPoints + amount, 0f, MaxAbilityPoints);
    }

    public bool TrySpendAbilityPoints(float amount)
    {
        if (amount <= 0f)
        {
            return true;
        }

        if (CurrentAbilityPoints < amount)
        {
            return false;
        }

        CurrentAbilityPoints -= amount;
        return true;
    }

    public bool TryAbsorbShieldHit()
    {
        if (!IsShieldActive)
        {
            return false;
        }

        _defenseAbilityState = _defenseAbilityController.ConsumeShieldCharge(_defenseAbilityState);
        return true;
    }

    private void UpdateRecoil(FrameTime frameTime)
    {
        Position += _knockbackMotion.Update(frameTime.DeltaSeconds);
        IsMoving = true;
        UpdateAttack(frameTime, attackJustPressed: false);
        UpdateDefenseAbility(defenseAbilityJustPressed: false);
        UpdateRangedAttack(frameTime, rangedAttackJustPressed: false);
    }

    private void UpdateStun(FrameTime frameTime)
    {
        _remainingStunSeconds = Math.Max(0f, _remainingStunSeconds - frameTime.DeltaSeconds);
        IsMoving = false;
        UpdateAttack(frameTime, attackJustPressed: false);
        UpdateDefenseAbility(defenseAbilityJustPressed: false);
        UpdateRangedAttack(frameTime, rangedAttackJustPressed: false);
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
            UpdateBombDash(frameTime);
            return false;
        }

        Position = dashResult.Position;
        Facing = dashResult.Facing;
        IsMoving = true;
        UpdateBombDash(frameTime);
        UpdateAttack(frameTime, attackJustPressed: false);
        UpdateDefenseAbility(defenseAbilityJustPressed: false);
        UpdateRangedAttack(frameTime, rangedAttackJustPressed: false);
        return true;
    }

    private bool CanStartDash()
    {
        return _inputService.IsJustPressed(GameAction.Dash)
            && CanUseEquippedDashAbility()
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
        var attackFacing = ResolveCombatFacing(_inputService.Current, Facing);
        _attackState = _attackController.Update(
            _attackState,
            Position,
            attackFacing,
            attackJustPressed,
            frameTime);
    }

    private void UpdateDefenseAbility(bool defenseAbilityJustPressed)
    {
        var result = _defenseAbilityController.Update(
            _defenseAbilityState,
            defenseAbilityJustPressed,
            CanActivateDefenseAbility());

        if (result.Activated && !TrySpendAbilityPoints(_defenseAbilityController.ShieldActivationCost))
        {
            return;
        }

        _defenseAbilityState = result.State;
    }

    private void UpdateRangedAttack(FrameTime frameTime, bool rangedAttackJustPressed)
    {
        var attackFacing = ResolveCombatFacing(_inputService.Current, Facing);
        var result = _rangedAttackController.Update(
            _rangedAttackState,
            Position,
            attackFacing,
            rangedAttackJustPressed,
            CanUseRangedAttack(),
            frameTime);
        _rangedAttackState = result.State;

        if (result.Projectile is not null)
        {
            _spawnedProjectiles.Add(result.Projectile);
        }
    }

    private bool CanUseRangedAttack()
    {
        return _rangedAttackState.EquippedAttack switch
        {
            PlayerRangedAttackKind.Fireball => _abilityService.HasAbility(PlayerAbility.Fireball),
            _ => false
        };
    }

    private bool CanUseEquippedDashAbility()
    {
        return _equippedDashAbility switch
        {
            PlayerDashAbilityKind.BaseDash => _abilityService.HasAbility(PlayerAbility.Dash),
            PlayerDashAbilityKind.BombDash => _abilityService.HasAbility(PlayerAbility.BombDash),
            _ => false
        };
    }

    private void UpdateBombDash(FrameTime frameTime)
    {
        var result = _bombDashController.Update(
            _bombTrailState,
            _dashState,
            Bounds,
            _equippedDashAbility == PlayerDashAbilityKind.BombDash && _abilityService.HasAbility(PlayerAbility.BombDash),
            frameTime);
        _bombTrailState = result.State;

        if (result.SpawnedBombs.Count > 0)
        {
            _spawnedBombs.AddRange(result.SpawnedBombs);
        }
    }

    private static Direction ResolveCombatFacing(InputSnapshot input, Direction fallbackFacing)
    {
        var horizontal = 0;
        var vertical = 0;

        if (input.IsPressed(GameAction.MoveLeft))
        {
            horizontal--;
        }

        if (input.IsPressed(GameAction.MoveRight))
        {
            horizontal++;
        }

        if (input.IsPressed(GameAction.MoveUp))
        {
            vertical--;
        }

        if (input.IsPressed(GameAction.MoveDown))
        {
            vertical++;
        }

        if (horizontal == 0 && vertical == 0)
        {
            return fallbackFacing;
        }

        if (Math.Abs(vertical) >= Math.Abs(horizontal) && vertical != 0)
        {
            return vertical < 0 ? Direction.Up : Direction.Down;
        }

        return horizontal < 0 ? Direction.Left : Direction.Right;
    }

    private bool CanActivateDefenseAbility()
    {
        return _defenseAbilityState.EquippedAbility switch
        {
            PlayerDefenseAbilityKind.Shield => CurrentAbilityPoints >= _defenseAbilityController.ShieldActivationCost,
            _ => false
        };
    }

    public IReadOnlyList<PlayerProjectile> ConsumeSpawnedProjectiles()
    {
        if (_spawnedProjectiles.Count == 0)
        {
            return [];
        }

        var projectiles = _spawnedProjectiles.ToArray();
        _spawnedProjectiles.Clear();
        return projectiles;
    }

    public IReadOnlyList<PlayerBomb> ConsumeSpawnedBombs()
    {
        if (_spawnedBombs.Count == 0)
        {
            return [];
        }

        var bombs = _spawnedBombs.ToArray();
        _spawnedBombs.Clear();
        return bombs;
    }

    public void EquipRangedAttack(PlayerRangedAttackKind rangedAttackKind)
    {
        _rangedAttackState = _rangedAttackState with { EquippedAttack = rangedAttackKind };
    }

    public void EquipDashAbility(PlayerDashAbilityKind dashAbilityKind)
    {
        _equippedDashAbility = dashAbilityKind;
    }

    public void EquipDefenseAbility(PlayerDefenseAbilityKind defenseAbilityKind)
    {
        _defenseAbilityState = PlayerDefenseAbilityState.Default with { EquippedAbility = defenseAbilityKind };
    }

    public void EquipMeleeAbility(PlayerMeleeAbilityKind meleeAbilityKind)
    {
        _equippedMeleeAbility = meleeAbilityKind;
    }

    public bool HasAbility(PlayerAbility ability)
    {
        return _abilityService.HasAbility(ability);
    }

    public IReadOnlyCollection<PlayerAbility> GetUnlockedAbilities()
    {
        return _abilityService.UnlockedAbilities;
    }

    public void UnlockAbility(PlayerAbility ability)
    {
        _abilityService.Unlock(ability);
    }

    public void SetUnlockedAbilities(IEnumerable<PlayerAbility> abilities)
    {
        _abilityService.SetUnlockedAbilities(abilities);
    }

    internal void MoveBy(Vector2 delta)
    {
        Position += delta;
    }

    private void RegenerateAbilityPoints(FrameTime frameTime)
    {
        if (IsDead || _combatSettings.AbilityPointRegenPerSecond <= 0f)
        {
            return;
        }

        AddAbilityPoints(_combatSettings.AbilityPointRegenPerSecond * frameTime.DeltaSeconds);
    }
}
