using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Gameplay.Player;

namespace MyGame.Gameplay.Enemies;

internal sealed class GrasshopperEnemyBehavior : IEnemyBehavior
{
    private readonly float _initialPauseSeconds;
    private Vector2 _leapVector;
    private float _remainingLeapSeconds;
    private float _remainingPauseSeconds;
    private int _leapsCompletedInCombo;
    private Direction _primaryDirection;

    public GrasshopperEnemyBehavior(float initialPauseSeconds)
    {
        _initialPauseSeconds = Math.Max(0f, initialPauseSeconds);
        _remainingPauseSeconds = _initialPauseSeconds;
        _primaryDirection = Direction.Left;
    }

    public void Update(EnemyActor enemy, Vector2 playerPosition, Rectangle playerBounds, FrameTime frameTime)
    {
        var toPlayer = playerPosition - enemy.Position;
        if (toPlayer.Length() > enemy.Settings.ChaseRange || toPlayer.LengthSquared() <= 0.001f)
        {
            ResetInternal(enemy, preserveInitialPause: false);
            enemy.SetState(EnemyState.Idle, isMoving: false);
            return;
        }

        if (_remainingLeapSeconds > 0f)
        {
            UpdateLeap(enemy, frameTime);
            return;
        }

        if (_remainingPauseSeconds > 0f)
        {
            _remainingPauseSeconds = Math.Max(0f, _remainingPauseSeconds - frameTime.DeltaSeconds);
            enemy.SetState(EnemyState.Aiming, isMoving: false);

            if (_remainingPauseSeconds > 0f)
            {
                return;
            }
        }

        StartNextLeap(enemy, playerPosition);
        UpdateLeap(enemy, frameTime);
    }

    public void Reset(EnemyActor enemy)
    {
        ResetInternal(enemy, preserveInitialPause: true);
        enemy.DashDirection = Direction.Left;
    }

    private void UpdateLeap(EnemyActor enemy, FrameTime frameTime)
    {
        enemy.MoveBy(_leapVector * enemy.Settings.DashSpeed * frameTime.DeltaSeconds);
        _remainingLeapSeconds = Math.Max(0f, _remainingLeapSeconds - frameTime.DeltaSeconds);

        if (_remainingLeapSeconds > 0f)
        {
            enemy.SetState(EnemyState.Dashing, isMoving: true);
            return;
        }

        _leapsCompletedInCombo++;

        if (_leapsCompletedInCombo >= 3)
        {
            _remainingPauseSeconds = enemy.Settings.DashPauseSeconds;
            _leapsCompletedInCombo = 0;
            enemy.SetState(EnemyState.Aiming, isMoving: false);
            return;
        }

        enemy.SetState(EnemyState.Aiming, isMoving: false);
    }

    private void StartNextLeap(EnemyActor enemy, Vector2 playerPosition)
    {
        _leapVector = ResolveLeapVector(enemy, playerPosition);
        _remainingLeapSeconds = enemy.Settings.DashSeconds;
        enemy.DashDirection = DirectionHelper.FromDominantAxis(_leapVector, enemy.DashDirection);
        enemy.SetState(EnemyState.Dashing, isMoving: true);
    }

    private Vector2 ResolveLeapVector(EnemyActor enemy, Vector2 playerPosition)
    {
        var toPlayer = playerPosition - enemy.Position;
        if (toPlayer.LengthSquared() <= 0.001f)
        {
            return DirectionHelper.ToVector(enemy.DashDirection);
        }

        toPlayer.Normalize();

        if (_leapsCompletedInCombo == 0)
        {
            _primaryDirection = DirectionHelper.FromDominantAxis(toPlayer, enemy.DashDirection);
            return toPlayer;
        }

        if (_leapsCompletedInCombo == 1)
        {
            return DirectionHelper.ToVector(_primaryDirection);
        }

        var alternate = toPlayer;
        var alternateDirection = DirectionHelper.FromDominantAxis(alternate, _primaryDirection);
        if (alternateDirection != _primaryDirection)
        {
            return alternate;
        }

        return RotateQuarterTurn(alternate);
    }

    private void ResetInternal(EnemyActor enemy, bool preserveInitialPause)
    {
        _leapVector = Vector2.Zero;
        _remainingLeapSeconds = 0f;
        _remainingPauseSeconds = preserveInitialPause ? _initialPauseSeconds : 0f;
        _leapsCompletedInCombo = 0;
        _primaryDirection = enemy.DashDirection;
    }

    private static Vector2 RotateQuarterTurn(Vector2 vector)
    {
        var rotated = new Vector2(-vector.Y, vector.X);
        if (rotated.LengthSquared() > 0.001f)
        {
            rotated.Normalize();
        }

        return rotated;
    }
}
