using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Gameplay.Player;

namespace MyGame.Gameplay.Enemies;

internal sealed class HornedRabbitEnemyBehavior : IEnemyBehavior
{
    private readonly EnemyAxisPreference _axisPreference;
    private float _remainingDashSeconds;
    private float _remainingDashPauseSeconds;

    public HornedRabbitEnemyBehavior(EnemyAxisPreference axisPreference, float initialDashPauseSeconds)
    {
        _axisPreference = axisPreference;
        _remainingDashPauseSeconds = Math.Max(0f, initialDashPauseSeconds);
    }

    public void Update(EnemyActor enemy, Vector2 playerPosition, Rectangle playerBounds, FrameTime frameTime)
    {
        var toPlayer = playerPosition - enemy.Position;
        var distanceToPlayer = toPlayer.Length();

        if (distanceToPlayer > enemy.Settings.ChaseRange || distanceToPlayer <= 0.001f)
        {
            _remainingDashSeconds = 0f;
            _remainingDashPauseSeconds = 0f;
            enemy.SetState(EnemyState.Idle, isMoving: false);
            return;
        }

        if (_remainingDashSeconds > 0f)
        {
            enemy.MoveBy(DirectionHelper.ToVector(enemy.DashDirection) * enemy.Settings.DashSpeed * frameTime.DeltaSeconds);
            _remainingDashSeconds = Math.Max(0f, _remainingDashSeconds - frameTime.DeltaSeconds);
            enemy.SetState(_remainingDashSeconds > 0f ? EnemyState.Dashing : EnemyState.Aiming, isMoving: _remainingDashSeconds > 0f);

            if (_remainingDashSeconds <= 0f)
            {
                _remainingDashPauseSeconds = enemy.Settings.DashPauseSeconds;
            }

            return;
        }

        enemy.DashDirection = ResolveDashDirection(toPlayer);

        if (_remainingDashPauseSeconds > 0f)
        {
            _remainingDashPauseSeconds = Math.Max(0f, _remainingDashPauseSeconds - frameTime.DeltaSeconds);
            enemy.SetState(EnemyState.Aiming, isMoving: false);

            if (_remainingDashPauseSeconds > 0f)
            {
                return;
            }
        }

        _remainingDashSeconds = enemy.Settings.DashSeconds;
        enemy.SetState(EnemyState.Dashing, isMoving: true);
        enemy.MoveBy(DirectionHelper.ToVector(enemy.DashDirection) * enemy.Settings.DashSpeed * frameTime.DeltaSeconds);
        _remainingDashSeconds = Math.Max(0f, _remainingDashSeconds - frameTime.DeltaSeconds);

        if (_remainingDashSeconds <= 0f)
        {
            _remainingDashPauseSeconds = enemy.Settings.DashPauseSeconds;
            enemy.SetState(EnemyState.Aiming, isMoving: false);
        }
    }

    public void Reset(EnemyActor enemy)
    {
        _remainingDashSeconds = 0f;
        _remainingDashPauseSeconds = 0f;
        enemy.DashDirection = Direction.Left;
    }

    private Direction ResolveDashDirection(Vector2 directionToPlayer)
    {
        if (_axisPreference == EnemyAxisPreference.Horizontal && Math.Abs(directionToPlayer.X) > 0.001f)
        {
            return directionToPlayer.X < 0f ? Direction.Left : Direction.Right;
        }

        if (_axisPreference == EnemyAxisPreference.Vertical && Math.Abs(directionToPlayer.Y) > 0.001f)
        {
            return directionToPlayer.Y < 0f ? Direction.Up : Direction.Down;
        }

        return DirectionHelper.FromDominantAxis(directionToPlayer, Direction.Down);
    }
}
