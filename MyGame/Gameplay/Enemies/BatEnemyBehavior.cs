using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Gameplay.Player;

namespace MyGame.Gameplay.Enemies;

internal sealed class BatEnemyBehavior : IEnemyBehavior
{
    private const float MinimumOrbitRadius = 42f;
    private const float MaximumOrbitRadius = 74f;

    private readonly float _initialDashPauseSeconds;
    private bool _isClockwise = true;
    private float _orbitAngle;
    private float _orbitRadius;
    private float _remainingSwoopSeconds;
    private float _remainingPauseSeconds;
    private Vector2 _orbitCenter;

    public BatEnemyBehavior(float initialDashPauseSeconds)
    {
        _initialDashPauseSeconds = Math.Max(0f, initialDashPauseSeconds);
        _remainingPauseSeconds = _initialDashPauseSeconds;
    }

    public void Update(EnemyActor enemy, Vector2 playerPosition, FrameTime frameTime)
    {
        var toPlayer = playerPosition - enemy.Position;
        var distanceToPlayer = toPlayer.Length();

        if (distanceToPlayer > enemy.Settings.ChaseRange || distanceToPlayer <= 0.001f)
        {
            ResetInternal(enemy, preserveInitialPause: false);
            enemy.SetState(EnemyState.Idle, isMoving: false);
            return;
        }

        if (_remainingSwoopSeconds > 0f)
        {
            UpdateSwoop(enemy, frameTime);
            return;
        }

        enemy.DashDirection = ResolveFacing(playerPosition - enemy.Position, enemy.DashDirection);

        if (_remainingPauseSeconds > 0f)
        {
            _remainingPauseSeconds = Math.Max(0f, _remainingPauseSeconds - frameTime.DeltaSeconds);
            enemy.SetState(EnemyState.Aiming, isMoving: false);

            if (_remainingPauseSeconds > 0f)
            {
                return;
            }
        }

        StartSwoop(enemy, playerPosition);
        UpdateSwoop(enemy, frameTime);
    }

    public void Reset(EnemyActor enemy)
    {
        ResetInternal(enemy, preserveInitialPause: true);
        enemy.DashDirection = Direction.Left;
    }

    private void UpdateSwoop(EnemyActor enemy, FrameTime frameTime)
    {
        var radiansPerSecond = enemy.Settings.DashSpeed / Math.Max(_orbitRadius, 1f);
        var angleDelta = radiansPerSecond * frameTime.DeltaSeconds * (_isClockwise ? 1f : -1f);
        _orbitAngle += angleDelta;

        var orbitOffset = new Vector2(MathF.Cos(_orbitAngle), MathF.Sin(_orbitAngle)) * _orbitRadius;
        var desiredPosition = _orbitCenter + orbitOffset;
        var maxStep = enemy.Settings.DashSpeed * frameTime.DeltaSeconds;
        var movement = desiredPosition - enemy.Position;

        if (movement.LengthSquared() > maxStep * maxStep && movement.LengthSquared() > 0.0001f)
        {
            movement.Normalize();
            movement *= maxStep;
        }

        enemy.MoveBy(movement);
        enemy.DashDirection = ResolveFacing(movement, enemy.DashDirection);
        _remainingSwoopSeconds = Math.Max(0f, _remainingSwoopSeconds - frameTime.DeltaSeconds);

        if (_remainingSwoopSeconds > 0f)
        {
            enemy.SetState(EnemyState.Dashing, isMoving: movement.LengthSquared() > 0.0001f);
            return;
        }

        _remainingPauseSeconds = enemy.Settings.DashPauseSeconds;
        _isClockwise = !_isClockwise;
        enemy.SetState(EnemyState.Aiming, isMoving: false);
    }

    private void StartSwoop(EnemyActor enemy, Vector2 playerPosition)
    {
        _orbitCenter = playerPosition;
        var offset = enemy.Position - _orbitCenter;
        if (offset.LengthSquared() <= 0.0001f)
        {
            offset = new Vector2(0f, -MinimumOrbitRadius);
        }

        _orbitRadius = MathHelper.Clamp(offset.Length(), MinimumOrbitRadius, MaximumOrbitRadius);
        _orbitAngle = MathF.Atan2(offset.Y, offset.X);
        _remainingSwoopSeconds = enemy.Settings.DashSeconds;
        enemy.DashDirection = ResolveFacing(playerPosition - enemy.Position, enemy.DashDirection);
        enemy.SetState(EnemyState.Dashing, isMoving: true);
    }

    private void ResetInternal(EnemyActor enemy, bool preserveInitialPause)
    {
        _remainingSwoopSeconds = 0f;
        _remainingPauseSeconds = preserveInitialPause ? _initialDashPauseSeconds : 0f;
        _orbitAngle = 0f;
        _orbitRadius = 0f;
        _orbitCenter = enemy.Position;
        _isClockwise = true;
    }

    private static Direction ResolveFacing(Vector2 movement, Direction fallback)
    {
        return movement.LengthSquared() <= 0.0001f
            ? fallback
            : DirectionHelper.FromDominantAxis(movement, fallback);
    }
}
