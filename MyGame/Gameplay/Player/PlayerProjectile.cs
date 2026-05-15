using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Gameplay.Enemies;

namespace MyGame.Gameplay.Player;

public sealed class PlayerProjectile
{
    private float _remainingLifetimeSeconds;
    private Vector2 _velocity;
    private EnemyActor? _target;

    public PlayerProjectile(
        PlayerRangedAttackKind kind,
        Vector2 position,
        Direction direction,
        float speed,
        float lifetimeSeconds,
        int size,
        int damage)
    {
        Kind = kind;
        Position = position;
        Direction = direction;
        Speed = speed;
        _remainingLifetimeSeconds = lifetimeSeconds;
        Size = size;
        Damage = damage;
        _velocity = DirectionHelper.ToVector(direction) * speed;
    }

    public PlayerRangedAttackKind Kind { get; }

    public Vector2 Position { get; private set; }

    public Direction Direction { get; private set; }

    public float Speed { get; }

    public int Size { get; }

    public int Damage { get; }

    public bool IsActive => _remainingLifetimeSeconds > 0f;

    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, Size, Size);

    public void AssignTarget(EnemyActor? target)
    {
        if (Kind != PlayerRangedAttackKind.Missile)
        {
            return;
        }

        _target = target;
    }

    public void Update(FrameTime frameTime)
    {
        if (!IsActive)
        {
            return;
        }

        UpdateVelocity();
        Position += _velocity * frameTime.DeltaSeconds;
        _remainingLifetimeSeconds = Math.Max(0f, _remainingLifetimeSeconds - frameTime.DeltaSeconds);
    }

    public void Deactivate()
    {
        _remainingLifetimeSeconds = 0f;
    }

    private void UpdateVelocity()
    {
        if (Kind != PlayerRangedAttackKind.Missile || !HasLiveTarget())
        {
            return;
        }

        var targetCenter = GetCenter(_target!.Bounds);
        var projectileCenter = GetCenter(Bounds);
        var targetDirection = targetCenter - projectileCenter;
        if (targetDirection.LengthSquared() <= 0.0001f)
        {
            return;
        }

        targetDirection.Normalize();
        _velocity = targetDirection * Speed;
        Direction = DirectionHelper.FromDominantAxis(targetDirection, Direction);
    }

    private bool HasLiveTarget()
    {
        return _target is not null
            && _target.State != EnemyState.Dead
            && !_target.IsBossStageTransitioning;
    }

    private static Vector2 GetCenter(Rectangle bounds)
    {
        return new Vector2(bounds.Center.X, bounds.Center.Y);
    }
}
