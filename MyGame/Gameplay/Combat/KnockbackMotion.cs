using Microsoft.Xna.Framework;

namespace MyGame.Gameplay.Combat;

public sealed class KnockbackMotion
{
    private readonly float _initialKnockbackRatio;
    private float _remainingSeconds;
    private Vector2 _velocity;

    public KnockbackMotion(float initialKnockbackRatio = 0.5f)
    {
        _initialKnockbackRatio = initialKnockbackRatio;
    }

    public bool IsActive => _remainingSeconds > 0f;

    public Vector2 Begin(Vector2 direction, float distance, float durationSeconds)
    {
        if (direction.LengthSquared() <= 0.0001f || distance <= 0f)
        {
            Reset();
            return Vector2.Zero;
        }

        direction.Normalize();

        var immediateDistance = distance * _initialKnockbackRatio;
        var remainingDistance = distance - immediateDistance;

        _remainingSeconds = Math.Max(0f, durationSeconds);
        _velocity = _remainingSeconds <= 0f
            ? Vector2.Zero
            : direction * (remainingDistance / _remainingSeconds);

        return direction * immediateDistance;
    }

    public Vector2 Update(float deltaSeconds)
    {
        if (!IsActive)
        {
            return Vector2.Zero;
        }

        var stepSeconds = Math.Min(deltaSeconds, _remainingSeconds);
        _remainingSeconds = Math.Max(0f, _remainingSeconds - deltaSeconds);
        return _velocity * stepSeconds;
    }

    public void Reset()
    {
        _remainingSeconds = 0f;
        _velocity = Vector2.Zero;
    }
}
