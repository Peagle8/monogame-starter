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
    private readonly IEnemyBehavior _behavior;
    private readonly KnockbackMotion _knockbackMotion = new();
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
        _behavior = EnemyBehaviorFactory.Create(settings.Kind, axisPreference, initialDashPauseSeconds);
        Position = position;
        PreviousPosition = position;
        CurrentHealth = settings.MaxHealth;
        DashDirection = Direction.Left;
        State = EnemyState.Idle;
    }

    public Vector2 Position { get; private set; }

    public Vector2 PreviousPosition { get; private set; }

    public int CurrentHealth { get; private set; }

    public int MaxHealth => _settings.MaxHealth;

    public EnemyKind Kind => _settings.Kind;

    public bool IsMoving { get; private set; }

    public EnemyState State { get; private set; }

    public bool CanDealContactDamage =>
        State is not EnemyState.Dead and not EnemyState.Recovering
        && (Kind switch
        {
            EnemyKind.HornedRabbit => State == EnemyState.Dashing,
            EnemyKind.Bat => State == EnemyState.Dashing,
            EnemyKind.Grasshopper => State == EnemyState.Dashing,
            _ => true
        });

    public Direction DashDirection { get; internal set; }

    public EnemyAxisPreference AxisPreference => _axisPreference;

    public bool IsRenderable => State != EnemyState.Dead || _remainingDefeatedVisibleSeconds > 0f;

    public bool IsFlashingFromHit => _remainingHitFlashSeconds > 0f;

    public float HitFlashAlpha => MathHelper.Clamp(_remainingHitFlashSeconds / HitFlashSeconds, 0f, 1f);

    public float DefeatedVisibilityAlpha =>
        State != EnemyState.Dead || _settings.DefeatedVisibleSeconds <= 0f
            ? 1f
            : MathHelper.Clamp(_remainingDefeatedVisibleSeconds / _settings.DefeatedVisibleSeconds, 0f, 1f);

    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, 28, 28);

    public Rectangle ContactBounds => GetContactBounds();

    public Rectangle PreviousBounds => new((int)PreviousPosition.X, (int)PreviousPosition.Y, 28, 28);

    public void Update(Vector2 playerPosition, FrameTime frameTime)
    {
        PreviousPosition = Position;
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

        _behavior.Update(this, playerPosition, frameTime);
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
        PreviousPosition = position;
        CurrentHealth = Math.Clamp(currentHealth, 0, MaxHealth);
        IsMoving = false;
        State = CurrentHealth == 0 ? EnemyState.Dead : EnemyState.Idle;
        _remainingDefeatedVisibleSeconds = 0f;
        _remainingHitFlashSeconds = 0f;
        _remainingRecoverySeconds = 0f;
        _knockbackMotion.Reset();
        _behavior.Reset(this);
    }

    internal EnemySettings Settings => _settings;

    internal void MoveBy(Vector2 delta)
    {
        Position += delta;
    }

    internal void SetState(EnemyState state, bool isMoving)
    {
        State = state;
        IsMoving = isMoving;
    }

    private Rectangle GetContactBounds()
    {
        if (_settings.AttackHitboxPadding <= 0 || !CanDealContactDamage)
        {
            return Bounds;
        }

        var bounds = Bounds;
        bounds.Inflate(_settings.AttackHitboxPadding, _settings.AttackHitboxPadding);
        return bounds;
    }
}
