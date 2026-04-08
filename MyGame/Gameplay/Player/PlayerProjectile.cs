using Microsoft.Xna.Framework;
using MyGame.Core;

namespace MyGame.Gameplay.Player;

public sealed class PlayerProjectile
{
    private float _remainingLifetimeSeconds;

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
    }

    public PlayerRangedAttackKind Kind { get; }

    public Vector2 Position { get; private set; }

    public Direction Direction { get; }

    public float Speed { get; }

    public int Size { get; }

    public int Damage { get; }

    public bool IsActive => _remainingLifetimeSeconds > 0f;

    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, Size, Size);

    public void Update(FrameTime frameTime)
    {
        if (!IsActive)
        {
            return;
        }

        Position += DirectionHelper.ToVector(Direction) * Speed * frameTime.DeltaSeconds;
        _remainingLifetimeSeconds = Math.Max(0f, _remainingLifetimeSeconds - frameTime.DeltaSeconds);
    }

    public void Deactivate()
    {
        _remainingLifetimeSeconds = 0f;
    }
}
