using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Core;
using MyGame.Infrastructure.Save;

namespace MyGame.Gameplay.Enemies;

public sealed class EnemyActor
{
    private readonly EnemySettings _settings;
    private float _remainingDefeatedVisibleSeconds;
    private float _remainingHitFlashSeconds;
    private float _remainingRecoverySeconds;
    private const float HitFlashSeconds = 0.12f;

    public EnemyActor(EnemySettings settings, Vector2 position)
    {
        _settings = settings;
        Position = position;
        CurrentHealth = settings.MaxHealth;
        State = EnemyState.Idle;
    }

    public Vector2 Position { get; private set; }

    public int CurrentHealth { get; private set; }

    public int MaxHealth => _settings.MaxHealth;

    public bool IsMoving { get; private set; }

    public EnemyState State { get; private set; }

    public bool CanDealContactDamage => State is not EnemyState.Dead and not EnemyState.Recovering;

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
            _remainingRecoverySeconds = Math.Max(0f, _remainingRecoverySeconds - frameTime.DeltaSeconds);

            if (_remainingRecoverySeconds > 0f)
            {
                State = EnemyState.Recovering;
                IsMoving = false;
                return;
            }
        }

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

    public EnemySaveData CreateSaveData()
    {
        return new EnemySaveData
        {
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
    }
}
