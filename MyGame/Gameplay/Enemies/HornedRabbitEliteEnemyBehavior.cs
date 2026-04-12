using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Gameplay.Player;

namespace MyGame.Gameplay.Enemies;

internal sealed class HornedRabbitEliteEnemyBehavior : IEnemyBehavior
{
    private const float BombDropIntervalSeconds = 0.08f;
    private const float BombFuseSeconds = 0.42f;
    private const float BombExplosionDurationSeconds = 0.18f;
    private const int BombSize = 12;
    private const int BombExplosionPadding = 10;
    private const float HorizontalAlignmentThreshold = 20f;

    private Vector2 _dashVector;
    private float _remainingDashSeconds;
    private float _remainingDashPauseSeconds;
    private float _remainingBombDropSeconds;

    public HornedRabbitEliteEnemyBehavior(EnemyAxisPreference axisPreference, float initialDashPauseSeconds)
    {
        _remainingDashPauseSeconds = Math.Max(0f, initialDashPauseSeconds);
    }

    public void Update(EnemyActor enemy, Vector2 playerPosition, Rectangle playerBounds, FrameTime frameTime)
    {
        var toPlayer = playerPosition - enemy.Position;
        var distanceToPlayer = toPlayer.Length();

        if (distanceToPlayer > enemy.Settings.ChaseRange || distanceToPlayer <= 0.001f)
        {
            Stop(enemy);
            return;
        }

        if (_remainingDashSeconds > 0f)
        {
            ContinueDash(enemy, frameTime);
            return;
        }

        _dashVector = ResolveDashVector(enemy, toPlayer);
        enemy.DashDirection = DirectionHelper.FromDominantAxis(_dashVector, enemy.DashDirection);

        if (_remainingDashPauseSeconds > 0f)
        {
            _remainingDashPauseSeconds = Math.Max(0f, _remainingDashPauseSeconds - frameTime.DeltaSeconds);
            enemy.SetState(EnemyState.Aiming, isMoving: false);

            if (_remainingDashPauseSeconds > 0f)
            {
                return;
            }
        }

        StartDash(enemy, frameTime);
    }

    public void Reset(EnemyActor enemy)
    {
        Stop(enemy);
        enemy.DashDirection = Direction.Left;
    }

    private void ContinueDash(EnemyActor enemy, FrameTime frameTime)
    {
        enemy.MoveBy(_dashVector * enemy.Settings.DashSpeed * frameTime.DeltaSeconds);
        UpdateBombTrail(enemy, frameTime.DeltaSeconds);
        _remainingDashSeconds = Math.Max(0f, _remainingDashSeconds - frameTime.DeltaSeconds);
        enemy.SetState(_remainingDashSeconds > 0f ? EnemyState.Dashing : EnemyState.Aiming, isMoving: _remainingDashSeconds > 0f);

        if (_remainingDashSeconds <= 0f)
        {
            _remainingDashPauseSeconds = enemy.Settings.DashPauseSeconds;
        }
    }

    private void StartDash(EnemyActor enemy, FrameTime frameTime)
    {
        _remainingDashSeconds = enemy.Settings.DashSeconds;
        _remainingBombDropSeconds = 0f;
        enemy.SetState(EnemyState.Dashing, isMoving: true);
        enemy.MoveBy(_dashVector * enemy.Settings.DashSpeed * frameTime.DeltaSeconds);
        DropBomb(enemy);
        _remainingDashSeconds = Math.Max(0f, _remainingDashSeconds - frameTime.DeltaSeconds);

        if (_remainingDashSeconds <= 0f)
        {
            _remainingDashPauseSeconds = enemy.Settings.DashPauseSeconds;
            enemy.SetState(EnemyState.Aiming, isMoving: false);
        }
    }

    private void UpdateBombTrail(EnemyActor enemy, float deltaSeconds)
    {
        _remainingBombDropSeconds = Math.Max(0f, _remainingBombDropSeconds - deltaSeconds);
        if (_remainingBombDropSeconds > 0f)
        {
            return;
        }

        DropBomb(enemy);
    }

    private void DropBomb(EnemyActor enemy)
    {
        var bounds = enemy.Bounds;
        var bombBounds = new Rectangle(
            bounds.Center.X - (BombSize / 2),
            bounds.Center.Y - (BombSize / 2),
            BombSize,
            BombSize);
        enemy.DropBomb(
            bombBounds,
            new EnemyAttack(1, 0f),
            BombFuseSeconds,
            BombExplosionDurationSeconds,
            BombExplosionPadding);
        _remainingBombDropSeconds = BombDropIntervalSeconds;
    }

    private void Stop(EnemyActor enemy)
    {
        _dashVector = Vector2.Zero;
        _remainingDashSeconds = 0f;
        _remainingDashPauseSeconds = 0f;
        _remainingBombDropSeconds = 0f;
        enemy.SetState(EnemyState.Idle, isMoving: false);
    }

    private Vector2 ResolveDashVector(EnemyActor enemy, Vector2 directionToPlayer)
    {
        if (Math.Abs(directionToPlayer.X) > 0.001f && Math.Abs(directionToPlayer.Y) <= HorizontalAlignmentThreshold)
        {
            return directionToPlayer.X < 0f ? Vector2.UnitX * -1f : Vector2.UnitX;
        }

        if (directionToPlayer.LengthSquared() <= 0.0001f)
        {
            return DirectionHelper.ToVector(enemy.DashDirection);
        }

        var diagonal = new Vector2(
            Math.Sign(directionToPlayer.X == 0f ? DirectionHelper.ToVector(enemy.DashDirection).X : directionToPlayer.X),
            Math.Sign(directionToPlayer.Y == 0f ? 1f : directionToPlayer.Y));
        if (diagonal.LengthSquared() <= 0.0001f)
        {
            diagonal = DirectionHelper.ToVector(enemy.DashDirection);
        }

        diagonal.Normalize();
        return diagonal;
    }
}
