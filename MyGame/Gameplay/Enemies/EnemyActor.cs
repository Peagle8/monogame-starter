using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Core;
using MyGame.Gameplay.Combat;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.World;
using MyGame.Infrastructure.Save;

namespace MyGame.Gameplay.Enemies;

public sealed class EnemyActor
{
    internal const float BossStageTransitionSeconds = 1.4f;

    private readonly EnemySettings _settings;
    private readonly EnemyAxisPreference _axisPreference;
    private readonly IEnemyBehavior _behavior;
    private readonly KnockbackMotion _knockbackMotion = new();
    private readonly List<EnemySpawnDefinition> _pendingEnemySpawns = [];
    private float _remainingDefeatedVisibleSeconds;
    private float _remainingHitFlashSeconds;
    private float _remainingRecoverySeconds;
    private float _remainingSpecialAttackVisibleSeconds;
    private readonly List<EnemyBomb> _bombs = [];
    private EnemyAttack? _pendingAttack;
    private EnemyAttack? _activeSpecialAttack;
    private bool _hasConsumedActiveSpecialAttack;
    private bool _hasPendingBossStageTransition;
    private float _remainingBossStageTransitionSeconds;
    private const float HitFlashSeconds = 0.12f;
    private const float SpecialAttackVisibleSeconds = 0.18f;

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
        if (settings.Kind == EnemyKind.HornedRabbitBoss)
        {
            BossStageCount = 3;
        }
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
        && !IsBossStageTransitioning
        && (Kind switch
        {
            EnemyKind.HornedRabbit => State == EnemyState.Dashing,
            EnemyKind.HornedRabbitBoss => State == EnemyState.Dashing,
            EnemyKind.HornedRabbitElite => State == EnemyState.Dashing,
            EnemyKind.Bat => State == EnemyState.Dashing,
            EnemyKind.BatMiniBoss => State == EnemyState.Dashing,
            EnemyKind.Grasshopper => State == EnemyState.Dashing,
            _ => true
        });

    public Direction DashDirection { get; internal set; }

    public EnemyAxisPreference AxisPreference => _axisPreference;

    public int BossStage { get; private set; } = 1;

    public int BossStageCount { get; private set; } = 1;

    public bool IsBossStageTransitioning => _remainingBossStageTransitionSeconds > 0f;

    public float RemainingBossStageTransitionSeconds => _remainingBossStageTransitionSeconds;

    public bool IsRenderable => State != EnemyState.Dead || _remainingDefeatedVisibleSeconds > 0f;

    public bool IsFlashingFromHit => _remainingHitFlashSeconds > 0f;

    public float HitFlashAlpha => MathHelper.Clamp(_remainingHitFlashSeconds / HitFlashSeconds, 0f, 1f);

    public float HealthBarAlpha => CurrentHealth > 0 ? 1f : 0f;

    public float DefeatedVisibilityAlpha =>
        State != EnemyState.Dead || _settings.DefeatedVisibleSeconds <= 0f
            ? 1f
            : MathHelper.Clamp(_remainingDefeatedVisibleSeconds / _settings.DefeatedVisibleSeconds, 0f, 1f);

    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, _settings.BoundsWidth, _settings.BoundsHeight);

    public Rectangle ContactBounds => GetContactBounds();

    public Rectangle PreviousBounds => new((int)PreviousPosition.X, (int)PreviousPosition.Y, _settings.BoundsWidth, _settings.BoundsHeight);

    public Vector2 AttackOrigin => GetAttackOrigin();

    public bool IsSpecialAttackTelegraphVisible { get; private set; }

    public bool IsSpecialAttackActive => _remainingSpecialAttackVisibleSeconds > 0f;

    public float SpecialAttackRange => _settings.SpecialAttackRange;

    public float SpecialAttackConeHalfAngleDegrees => _settings.SpecialAttackConeHalfAngleDegrees;

    internal IReadOnlyList<EnemyBomb> Bombs => _bombs;

    public void Update(Vector2 playerPosition, FrameTime frameTime)
    {
        PreviousPosition = Position;
        _remainingHitFlashSeconds = Math.Max(0f, _remainingHitFlashSeconds - frameTime.DeltaSeconds);
        _remainingSpecialAttackVisibleSeconds = Math.Max(0f, _remainingSpecialAttackVisibleSeconds - frameTime.DeltaSeconds);
        _remainingBossStageTransitionSeconds = Math.Max(0f, _remainingBossStageTransitionSeconds - frameTime.DeltaSeconds);
        UpdateBombs(frameTime.DeltaSeconds);
        if (_remainingSpecialAttackVisibleSeconds <= 0f)
        {
            _activeSpecialAttack = null;
            _hasConsumedActiveSpecialAttack = false;
        }

        if (CurrentHealth <= 0)
        {
            State = EnemyState.Dead;
            IsMoving = false;
            IsSpecialAttackTelegraphVisible = false;
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

        _behavior.Update(this, playerPosition, new Rectangle((int)playerPosition.X, (int)playerPosition.Y, 0, 0), frameTime);
    }

    public void Update(Vector2 playerPosition, Rectangle playerBounds, FrameTime frameTime)
    {
        PreviousPosition = Position;
        _remainingHitFlashSeconds = Math.Max(0f, _remainingHitFlashSeconds - frameTime.DeltaSeconds);
        _remainingSpecialAttackVisibleSeconds = Math.Max(0f, _remainingSpecialAttackVisibleSeconds - frameTime.DeltaSeconds);
        _remainingBossStageTransitionSeconds = Math.Max(0f, _remainingBossStageTransitionSeconds - frameTime.DeltaSeconds);
        UpdateBombs(frameTime.DeltaSeconds);
        if (_remainingSpecialAttackVisibleSeconds <= 0f)
        {
            _activeSpecialAttack = null;
            _hasConsumedActiveSpecialAttack = false;
        }

        if (CurrentHealth <= 0)
        {
            State = EnemyState.Dead;
            IsMoving = false;
            IsSpecialAttackTelegraphVisible = false;
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
                IsSpecialAttackTelegraphVisible = false;
                return;
            }
        }

        _behavior.Update(this, playerPosition, playerBounds, frameTime);
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
        IsSpecialAttackTelegraphVisible = false;
        _remainingSpecialAttackVisibleSeconds = 0f;

        if (TryAdvanceBossStage())
        {
            return;
        }

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

        if (Kind == EnemyKind.HornedRabbitBoss)
        {
            return;
        }

        if (IsBossStageTransitioning)
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
        _remainingSpecialAttackVisibleSeconds = 0f;
        IsSpecialAttackTelegraphVisible = false;
        _pendingAttack = null;
        _activeSpecialAttack = null;
        _hasConsumedActiveSpecialAttack = false;
        _bombs.Clear();
        _pendingEnemySpawns.Clear();
        _hasPendingBossStageTransition = false;
        _remainingBossStageTransitionSeconds = 0f;
        _knockbackMotion.Reset();
        _behavior.Reset(this);
    }

    internal EnemySettings Settings => _settings;

    internal void QueueAttack(EnemyAttack attack)
    {
        _pendingAttack = attack;
    }

    internal bool TryConsumePendingAttack(out EnemyAttack attack)
    {
        if (_pendingAttack is null)
        {
            attack = null!;
            return false;
        }

        attack = _pendingAttack;
        _pendingAttack = null;
        _hasConsumedActiveSpecialAttack = true;
        return true;
    }

    internal bool TryConsumeActiveSpecialAttack(Rectangle targetBounds, out EnemyAttack attack)
    {
        if (_activeSpecialAttack is null
            || _hasConsumedActiveSpecialAttack
            || !IsSpecialAttackActive
            || !ConeAttackHitTester.Intersects(this, targetBounds))
        {
            attack = null!;
            return false;
        }

        attack = _activeSpecialAttack;
        _hasConsumedActiveSpecialAttack = true;
        return true;
    }

    internal void ShowSpecialAttackTelegraph(Direction direction)
    {
        DashDirection = direction;
        IsSpecialAttackTelegraphVisible = true;
    }

    internal void ClearSpecialAttackTelegraph()
    {
        IsSpecialAttackTelegraphVisible = false;
    }

    internal void TriggerSpecialAttack(Direction direction, EnemyAttack attack)
    {
        DashDirection = direction;
        _remainingSpecialAttackVisibleSeconds = SpecialAttackVisibleSeconds;
        _activeSpecialAttack = attack;
        _hasConsumedActiveSpecialAttack = false;
    }

    internal void MoveBy(Vector2 delta)
    {
        Position += delta;
    }

    internal void DropBomb(Rectangle bounds, EnemyAttack attack, float fuseSeconds, float explosionDurationSeconds, int explosionPadding)
    {
        _bombs.Add(new EnemyBomb(bounds, attack, fuseSeconds, explosionDurationSeconds, explosionPadding));
    }

    internal void QueueSpawnEnemy(EnemySpawnDefinition spawn)
    {
        _pendingEnemySpawns.Add(spawn);
    }

    internal IReadOnlyList<EnemySpawnDefinition> ConsumePendingEnemySpawns()
    {
        if (_pendingEnemySpawns.Count == 0)
        {
            return [];
        }

        var pendingSpawns = _pendingEnemySpawns.ToArray();
        _pendingEnemySpawns.Clear();
        return pendingSpawns;
    }

    internal bool TryConsumeActiveBombExplosion(Rectangle targetBounds, out EnemyAttack attack, out Rectangle explosionBounds)
    {
        foreach (var bomb in _bombs)
        {
            if (bomb.TryConsumeExplosion(targetBounds, out attack))
            {
                explosionBounds = bomb.ExplosionBounds;
                return true;
            }
        }

        attack = null!;
        explosionBounds = Rectangle.Empty;
        return false;
    }

    internal void SetState(EnemyState state, bool isMoving)
    {
        State = state;
        IsMoving = isMoving;
    }

    internal void SetBossStage(int stage, int stageCount)
    {
        BossStageCount = Math.Max(1, stageCount);
        BossStage = Math.Clamp(stage, 1, BossStageCount);
    }

    internal bool TryConsumePendingBossStageTransition()
    {
        if (!_hasPendingBossStageTransition)
        {
            return false;
        }

        _hasPendingBossStageTransition = false;
        return true;
    }

    private void UpdateBombs(float deltaSeconds)
    {
        foreach (var bomb in _bombs)
        {
            bomb.Update(deltaSeconds);
        }

        _bombs.RemoveAll(static bomb => !bomb.IsActive);
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

    private Vector2 GetAttackOrigin()
    {
        return DashDirection switch
        {
            Direction.Up => new Vector2(Bounds.Center.X, Bounds.Top),
            Direction.Down => new Vector2(Bounds.Center.X, Bounds.Bottom),
            Direction.Left => new Vector2(Bounds.Left, Bounds.Center.Y),
            Direction.Right => new Vector2(Bounds.Right, Bounds.Center.Y),
            _ => new Vector2(Bounds.Center.X, Bounds.Center.Y)
        };
    }

    private bool TryAdvanceBossStage()
    {
        if (Kind != EnemyKind.HornedRabbitBoss
            || CurrentHealth > 0
            || BossStage >= BossStageCount)
        {
            return false;
        }

        BossStage++;
        CurrentHealth = MaxHealth;
        State = EnemyState.Aiming;
        IsMoving = false;
        _remainingRecoverySeconds = 0f;
        _hasPendingBossStageTransition = true;
        _remainingBossStageTransitionSeconds = BossStageTransitionSeconds;
        return true;
    }
}
