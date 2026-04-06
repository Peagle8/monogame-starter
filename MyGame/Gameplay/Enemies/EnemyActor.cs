using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Core;
using MyGame.Gameplay.Combat;
using MyGame.Gameplay.Player;
using MyGame.Infrastructure.Save;

namespace MyGame.Gameplay.Enemies;

public sealed class EnemyActor
{
    private readonly EnemySettings _settings;
    private readonly EnemyAxisPreference _axisPreference;
    private readonly KnockbackMotion _knockbackMotion = new();
    private Direction _dashDirection = Direction.Left;
    private float _remainingDashSeconds;
    private float _remainingDashPauseSeconds;
    private float _remainingDefeatedVisibleSeconds;
    private float _remainingHitFlashSeconds;
    private float _remainingRecoverySeconds;
    private const float HitFlashSeconds = 0.12f;

    public EnemyActor(
        EnemySettings settings,
        Vector2 position,
        float initialDashPauseSeconds = 0f,
        EnemyAxisPreference axisPreference = EnemyAxisPreference.None)
    {
        _settings = settings;
        _axisPreference = axisPreference;
        Position = position;
        CurrentHealth = settings.MaxHealth;
        _remainingDashPauseSeconds = Math.Max(0f, initialDashPauseSeconds);
        State = EnemyState.Idle;
    }

    public Vector2 Position { get; private set; }

    public int CurrentHealth { get; private set; }

    public int MaxHealth => _settings.MaxHealth;

    public EnemyKind Kind => _settings.Kind;

    public bool IsMoving { get; private set; }

    public EnemyState State { get; private set; }

    public bool CanDealContactDamage =>
        State is not EnemyState.Dead and not EnemyState.Recovering
        && (Kind != EnemyKind.HornedRabbit || State == EnemyState.Dashing);

    public Direction DashDirection => _dashDirection;

    public EnemyAxisPreference AxisPreference => _axisPreference;

    public bool IsRenderable => State != EnemyState.Dead || _remainingDefeatedVisibleSeconds > 0f;

    public bool IsFlashingFromHit => _remainingHitFlashSeconds > 0f;

    public float HitFlashAlpha => MathHelper.Clamp(_remainingHitFlashSeconds / HitFlashSeconds, 0f, 1f);

    public float DefeatedVisibilityAlpha =>
        State != EnemyState.Dead || _settings.DefeatedVisibleSeconds <= 0f
            ? 1f
            : MathHelper.Clamp(_remainingDefeatedVisibleSeconds / _settings.DefeatedVisibleSeconds, 0f, 1f);

    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, 28, 28);

    public void Update(Vector2 playerPosition, FrameTime frameTime)
    {
        _remainingHitFlashSeconds = Math.Max(0f, _remainingHitFlashSeconds - frameTime.DeltaSeconds);

        if (CurrentHealth <= 0)
        {
            State = EnemyState.Dead;
            IsMoving = false;
            _remainingDefeatedVisibleSeconds = Math.Max(0f, _remainingDefeatedVisibleSeconds - frameTime.DeltaSeconds);
            return;
        }

        if (_remainingRecoverySeconds > 0f)
        {
            if (_knockbackMotion.IsActive)
            {
                Position += _knockbackMotion.Update(frameTime.DeltaSeconds);
                IsMoving = true;
            }
            else
            {
                IsMoving = false;
            }

            _remainingRecoverySeconds = Math.Max(0f, _remainingRecoverySeconds - frameTime.DeltaSeconds);

            if (_remainingRecoverySeconds > 0f)
            {
                State = EnemyState.Recovering;
                return;
            }
        }

        switch (Kind)
        {
            case EnemyKind.Crab:
                UpdateCrab(playerPosition, frameTime);
                return;
            case EnemyKind.HornedRabbit:
                UpdateHornedRabbit(playerPosition, frameTime);
                return;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void BeginRecovery()
    {
        if (CurrentHealth <= 0)
        {
            return;
        }

        _remainingRecoverySeconds = _settings.RecoverySeconds;
        State = EnemyState.Recovering;
        IsMoving = false;
    }

    public void TakeDamage(int amount)
    {
        CurrentHealth = Math.Max(0, CurrentHealth - amount);
        _remainingHitFlashSeconds = HitFlashSeconds;

        if (CurrentHealth == 0)
        {
            State = EnemyState.Dead;
            IsMoving = false;
            _remainingDefeatedVisibleSeconds = _settings.DefeatedVisibleSeconds;
        }
    }

    public void ApplyKnockback(Vector2 direction)
    {
        if (CurrentHealth <= 0 || direction.LengthSquared() <= 0.0001f)
        {
            return;
        }

        direction.Normalize();
        Position += _knockbackMotion.Begin(
            direction,
            _settings.PlayerHitKnockbackDistance,
            _settings.PlayerHitKnockbackSeconds);
        _remainingRecoverySeconds = Math.Max(_remainingRecoverySeconds, _settings.PlayerHitKnockbackSeconds);
        
        State = EnemyState.Recovering;
        IsMoving = true;
    }

    public EnemySaveData CreateSaveData()
    {
        return new EnemySaveData
        {
            Kind = Kind,
            AxisPreference = AxisPreference,
            PositionX = Position.X,
            PositionY = Position.Y,
            CurrentHealth = CurrentHealth
        };
    }

    public void RestoreState(Vector2 position, int currentHealth)
    {
        Position = position;
        CurrentHealth = Math.Clamp(currentHealth, 0, MaxHealth);
        IsMoving = false;
        State = CurrentHealth == 0 ? EnemyState.Dead : EnemyState.Idle;
        _remainingDefeatedVisibleSeconds = 0f;
        _remainingHitFlashSeconds = 0f;
        _remainingRecoverySeconds = 0f;
        _remainingDashSeconds = 0f;
        _remainingDashPauseSeconds = 0f;
        _dashDirection = Direction.Left;
        _knockbackMotion.Reset();
    }

    // TODO: we have to get these per enemy update methods out of here and refactor this before it gets out of control... let's do this by the third enemy.
    // TODO: when refactoring these pause and ask for my opinion on options
    private void UpdateCrab(Vector2 playerPosition, FrameTime frameTime)
    {
        var toPlayer = playerPosition - Position;
        var distanceToPlayer = toPlayer.Length();

        if (distanceToPlayer > _settings.ChaseRange || distanceToPlayer <= 0.001f)
        {
            State = EnemyState.Idle;
            IsMoving = false;
            return;
        }

        toPlayer.Normalize();
        Position += toPlayer * _settings.MoveSpeed * frameTime.DeltaSeconds;
        State = EnemyState.Chasing;
        IsMoving = true;
    }

    private void UpdateHornedRabbit(Vector2 playerPosition, FrameTime frameTime)
    {
        var toPlayer = playerPosition - Position;
        var distanceToPlayer = toPlayer.Length();

        if (distanceToPlayer > _settings.ChaseRange || distanceToPlayer <= 0.001f)
        {
            _remainingDashSeconds = 0f;
            _remainingDashPauseSeconds = 0f;
            State = EnemyState.Idle;
            IsMoving = false;
            return;
        }

        if (_remainingDashSeconds > 0f)
        {
            Position += GetDirectionVector(_dashDirection) * _settings.DashSpeed * frameTime.DeltaSeconds;
            _remainingDashSeconds = Math.Max(0f, _remainingDashSeconds - frameTime.DeltaSeconds);
            IsMoving = true;
            State = _remainingDashSeconds > 0f ? EnemyState.Dashing : EnemyState.Aiming;

            if (_remainingDashSeconds <= 0f)
            {
                _remainingDashPauseSeconds = _settings.DashPauseSeconds;
                IsMoving = false;
            }

            return;
        }

        _dashDirection = ResolveDashDirection(toPlayer, _axisPreference);

        if (_remainingDashPauseSeconds > 0f)
        {
            _remainingDashPauseSeconds = Math.Max(0f, _remainingDashPauseSeconds - frameTime.DeltaSeconds);
            State = EnemyState.Aiming;
            IsMoving = false;

            if (_remainingDashPauseSeconds > 0f)
            {
                return;
            }
        }

        _remainingDashSeconds = _settings.DashSeconds;
        State = EnemyState.Dashing;
        IsMoving = true;
        Position += GetDirectionVector(_dashDirection) * _settings.DashSpeed * frameTime.DeltaSeconds;
        _remainingDashSeconds = Math.Max(0f, _remainingDashSeconds - frameTime.DeltaSeconds);

        if (_remainingDashSeconds <= 0f)
        {
            _remainingDashPauseSeconds = _settings.DashPauseSeconds;
            State = EnemyState.Aiming;
            IsMoving = false;
        }
    }

    // TODO: refactor, add this to a common util method/type with other related direction methods.
    private static Direction ResolveDashDirection(Vector2 directionToPlayer, EnemyAxisPreference axisPreference)
    {
        if (axisPreference == EnemyAxisPreference.Horizontal && Math.Abs(directionToPlayer.X) > 0.001f)
        {
            return directionToPlayer.X < 0f ? Direction.Left : Direction.Right;
        }

        if (axisPreference == EnemyAxisPreference.Vertical && Math.Abs(directionToPlayer.Y) > 0.001f)
        {
            return directionToPlayer.Y < 0f ? Direction.Up : Direction.Down;
        }

        if (Math.Abs(directionToPlayer.X) > Math.Abs(directionToPlayer.Y))
        {
            return directionToPlayer.X < 0f ? Direction.Left : Direction.Right;
        }

        return directionToPlayer.Y < 0f ? Direction.Up : Direction.Down;
    }

    // TODO: I know this method is identical to one elsewhere with a TODO so refactor in a utility type and method with like common direction methods
    private static Vector2 GetDirectionVector(Direction direction)
    {
        return direction switch
        {
            Direction.Up => new Vector2(0f, -1f),
            Direction.Down => new Vector2(0f, 1f),
            Direction.Left => new Vector2(-1f, 0f),
            Direction.Right => new Vector2(1f, 0f),
            _ => Vector2.Zero
        };
    }
}
