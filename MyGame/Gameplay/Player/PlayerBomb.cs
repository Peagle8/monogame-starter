using Microsoft.Xna.Framework;

namespace MyGame.Gameplay.Player;

public sealed class PlayerBomb
{
    private readonly int _damage;
    private readonly float _explosionDurationSeconds;
    private readonly int _explosionPadding;
    private float _remainingFuseSeconds;
    private float _remainingExplosionSeconds;
    private bool _hasResolvedExplosion;

    public PlayerBomb(
        Rectangle bounds,
        int damage,
        float fuseSeconds,
        float explosionDurationSeconds,
        int explosionPadding)
    {
        Bounds = bounds;
        _damage = damage;
        _remainingFuseSeconds = Math.Max(0f, fuseSeconds);
        _explosionDurationSeconds = Math.Max(0.01f, explosionDurationSeconds);
        _explosionPadding = Math.Max(0, explosionPadding);
    }

    public Rectangle Bounds { get; }

    public Rectangle ExplosionBounds
    {
        get
        {
            var bounds = Bounds;
            bounds.Inflate(_explosionPadding, _explosionPadding);
            return bounds;
        }
    }

    public bool IsExploding => _remainingFuseSeconds <= 0f && _remainingExplosionSeconds > 0f;

    public bool IsActive => _remainingFuseSeconds > 0f || _remainingExplosionSeconds > 0f;

    public float FuseAlpha => _remainingFuseSeconds > 0f ? 1f : 0f;

    public float ExplosionAlpha => _remainingExplosionSeconds > 0f
        ? MathHelper.Clamp(_remainingExplosionSeconds / _explosionDurationSeconds, 0f, 1f)
        : 0f;

    public void Update(float deltaSeconds)
    {
        if (!IsActive)
        {
            return;
        }

        if (_remainingFuseSeconds > 0f)
        {
            _remainingFuseSeconds = Math.Max(0f, _remainingFuseSeconds - deltaSeconds);
            if (_remainingFuseSeconds <= 0f)
            {
                _remainingExplosionSeconds = _explosionDurationSeconds;
            }

            return;
        }

        _remainingExplosionSeconds = Math.Max(0f, _remainingExplosionSeconds - deltaSeconds);
    }

    public bool TryConsumeExplosion(out Rectangle explosionBounds, out int damage)
    {
        if (!IsExploding || _hasResolvedExplosion)
        {
            explosionBounds = Rectangle.Empty;
            damage = 0;
            return false;
        }

        _hasResolvedExplosion = true;
        explosionBounds = ExplosionBounds;
        damage = _damage;
        return true;
    }
}
