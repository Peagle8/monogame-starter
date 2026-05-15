using Microsoft.Xna.Framework;
using MyGame.Core;

namespace MyGame.Gameplay.Enemies;

public sealed class EnemyProjectile
{
    private float _remainingLifetimeSeconds;

    public EnemyProjectile(
        Vector2 position,
        Vector2 velocity,
        float lifetimeSeconds,
        int size,
        int damage)
    {
        Position = position;
        Velocity = velocity;
        _remainingLifetimeSeconds = lifetimeSeconds;
        Size = size;
        Damage = damage;
    }

    public Vector2 Position { get; private set; }

    public Vector2 Velocity { get; }

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

        Position += Velocity * frameTime.DeltaSeconds;
        _remainingLifetimeSeconds = Math.Max(0f, _remainingLifetimeSeconds - frameTime.DeltaSeconds);
    }

    public void Deactivate()
    {
        _remainingLifetimeSeconds = 0f;
    }
}
