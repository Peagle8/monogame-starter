using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Gameplay.Player;

namespace MyGame.Gameplay.Enemies;

internal sealed class SkeletonEnemyBehavior : IEnemyBehavior
{
    private const float EliteBackstepPauseSeconds = 0.14f;

    private readonly float _initialPauseSeconds;
    private readonly int _projectilesPerVolley;
    private readonly float _projectileSpreadDegrees;
    private readonly bool _usesBackstepLeap;
    private float _remainingPauseSeconds;
    private Vector2 _backstepVector;
    private float _remainingBackstepSeconds;

    public SkeletonEnemyBehavior(
        float initialPauseSeconds,
        int projectilesPerVolley = 1,
        float projectileSpreadDegrees = 0f,
        bool usesBackstepLeap = false)
    {
        _initialPauseSeconds = Math.Max(0f, initialPauseSeconds);
        _projectilesPerVolley = Math.Max(1, projectilesPerVolley);
        _projectileSpreadDegrees = Math.Max(0f, projectileSpreadDegrees);
        _usesBackstepLeap = usesBackstepLeap;
        _remainingPauseSeconds = _initialPauseSeconds;
    }

    public void Update(EnemyActor enemy, Vector2 playerPosition, Rectangle playerBounds, FrameTime frameTime)
    {
        var toPlayer = playerPosition - enemy.Position;
        var distanceToPlayer = toPlayer.Length();

        if (distanceToPlayer > enemy.Settings.ChaseRange || distanceToPlayer <= 0.001f)
        {
            ResetInternal(preserveInitialPause: false);
            enemy.SetState(EnemyState.Idle, isMoving: false);
            return;
        }

        enemy.DashDirection = DirectionHelper.FromDominantAxis(toPlayer, enemy.DashDirection);
        enemy.TryActivateShield();
        _remainingPauseSeconds = Math.Max(0f, _remainingPauseSeconds - frameTime.DeltaSeconds);
        if (_remainingBackstepSeconds > 0f)
        {
            ContinueBackstep(enemy, frameTime);
            return;
        }

        if (distanceToPlayer < enemy.Settings.PreferredRange)
        {
            RepositionAway(enemy, toPlayer, frameTime);
            return;
        }

        if (distanceToPlayer > enemy.Settings.ProjectileAttackRange)
        {
            MoveToward(enemy, toPlayer, frameTime);
            return;
        }

        if (_remainingPauseSeconds > 0f)
        {
            enemy.SetState(EnemyState.Aiming, isMoving: false);
            return;
        }

        if (TryFireArrowVolley(enemy, toPlayer))
        {
            _remainingPauseSeconds = enemy.Settings.DashPauseSeconds;
        }

        enemy.SetState(EnemyState.Aiming, isMoving: false);
    }

    public void Reset(EnemyActor enemy)
    {
        ResetInternal(preserveInitialPause: true);
        enemy.DashDirection = Direction.Left;
    }

    private void MoveToward(EnemyActor enemy, Vector2 toPlayer, FrameTime frameTime)
    {
        var movement = NormalizeOrZero(toPlayer) * enemy.Settings.MoveSpeed * frameTime.DeltaSeconds;
        enemy.MoveBy(movement);
        enemy.SetState(EnemyState.Chasing, isMoving: movement.LengthSquared() > 0.0001f);
    }

    private void RepositionAway(EnemyActor enemy, Vector2 toPlayer, FrameTime frameTime)
    {
        if (!_usesBackstepLeap
            || enemy.Settings.DashSpeed <= 0f
            || enemy.Settings.DashSeconds <= 0f)
        {
            MoveAway(enemy, toPlayer, frameTime);
            return;
        }

        if (_remainingPauseSeconds > 0f)
        {
            enemy.SetState(EnemyState.Aiming, isMoving: false);
            return;
        }

        StartBackstep(enemy, toPlayer);
        ContinueBackstep(enemy, frameTime);
    }

    private void MoveAway(EnemyActor enemy, Vector2 toPlayer, FrameTime frameTime)
    {
        var movement = NormalizeOrZero(-toPlayer) * enemy.Settings.MoveSpeed * frameTime.DeltaSeconds;
        enemy.MoveBy(movement);
        enemy.SetState(EnemyState.Chasing, isMoving: movement.LengthSquared() > 0.0001f);
    }

    private void StartBackstep(EnemyActor enemy, Vector2 toPlayer)
    {
        if (_remainingBackstepSeconds > 0f)
        {
            return;
        }

        _backstepVector = NormalizeOrZero(-toPlayer);
        if (_backstepVector.LengthSquared() <= 0.0001f)
        {
            _backstepVector = -DirectionHelper.ToVector(enemy.DashDirection);
        }

        _remainingBackstepSeconds = enemy.Settings.DashSeconds;
    }

    private void ContinueBackstep(EnemyActor enemy, FrameTime frameTime)
    {
        var movement = _backstepVector * enemy.Settings.DashSpeed * frameTime.DeltaSeconds;
        enemy.MoveBy(movement);
        _remainingBackstepSeconds = Math.Max(0f, _remainingBackstepSeconds - frameTime.DeltaSeconds);
        enemy.SetState(EnemyState.Chasing, isMoving: movement.LengthSquared() > 0.0001f);

        if (_remainingBackstepSeconds <= 0f)
        {
            _remainingPauseSeconds = Math.Max(_remainingPauseSeconds, EliteBackstepPauseSeconds);
        }
    }

    private bool TryFireArrowVolley(EnemyActor enemy, Vector2 toPlayer)
    {
        var direction = NormalizeOrZero(toPlayer);
        if (direction.LengthSquared() <= 0.0001f
            || enemy.Settings.ProjectileDamage <= 0
            || enemy.Settings.ProjectileSpeed <= 0f
            || enemy.Settings.ProjectileLifetimeSeconds <= 0f
            || enemy.Settings.ProjectileSize <= 0)
        {
            return false;
        }

        return _projectilesPerVolley switch
        {
            1 => QueueArrow(enemy, direction),
            2 => QueueTwinShot(enemy, direction),
            _ => QueueArrow(enemy, direction)
        };
    }

    private bool QueueTwinShot(EnemyActor enemy, Vector2 direction)
    {
        var halfSpreadDegrees = _projectileSpreadDegrees * 0.5f;
        var leftDirection = Rotate(direction, -halfSpreadDegrees);
        var rightDirection = Rotate(direction, halfSpreadDegrees);

        return QueueArrow(enemy, leftDirection) | QueueArrow(enemy, rightDirection);
    }

    private bool QueueArrow(EnemyActor enemy, Vector2 direction)
    {
        var projectileRadius = enemy.Settings.ProjectileSize / 2f;
        var enemyCenter = new Vector2(enemy.Bounds.Center.X, enemy.Bounds.Center.Y);
        var spawnOffset = direction * ((Math.Max(enemy.Bounds.Width, enemy.Bounds.Height) / 2f) + projectileRadius + 2f);
        var spawnPosition = enemyCenter + spawnOffset - new Vector2(projectileRadius, projectileRadius);

        enemy.QueueProjectile(new EnemyProjectile(
            spawnPosition,
            direction * enemy.Settings.ProjectileSpeed,
            enemy.Settings.ProjectileLifetimeSeconds,
            enemy.Settings.ProjectileSize,
            enemy.Settings.ProjectileDamage));
        return true;
    }

    private void ResetInternal(bool preserveInitialPause)
    {
        _remainingPauseSeconds = preserveInitialPause ? _initialPauseSeconds : 0f;
        _backstepVector = Vector2.Zero;
        _remainingBackstepSeconds = 0f;
    }

    private static Vector2 NormalizeOrZero(Vector2 value)
    {
        if (value.LengthSquared() <= 0.0001f)
        {
            return Vector2.Zero;
        }

        value.Normalize();
        return value;
    }

    private static Vector2 Rotate(Vector2 vector, float degrees)
    {
        var radians = MathHelper.ToRadians(degrees);
        var cosine = MathF.Cos(radians);
        var sine = MathF.Sin(radians);

        return NormalizeOrZero(new Vector2(
            (vector.X * cosine) - (vector.Y * sine),
            (vector.X * sine) + (vector.Y * cosine)));
    }
}
