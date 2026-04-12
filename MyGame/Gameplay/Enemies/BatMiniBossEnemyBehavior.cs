using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Gameplay.Player;

namespace MyGame.Gameplay.Enemies;

internal sealed class BatMiniBossEnemyBehavior : IEnemyBehavior
{
    private readonly BatEnemyBehavior _followUpBehavior = new(0f);
    private bool _isChargingConeAttack;
    private bool _isPerformingFollowUpSwoop;
    private bool _followUpSwoopStarted;
    private float _remainingConeChargeSeconds;

    public void Update(EnemyActor enemy, Vector2 playerPosition, Rectangle playerBounds, FrameTime frameTime)
    {
        var targetPoint = ResolveTargetPoint(playerPosition, playerBounds);
        var toPlayerFromCenter = targetPoint - new Vector2(enemy.Bounds.Center.X, enemy.Bounds.Center.Y);
        enemy.DashDirection = ResolveFacing(toPlayerFromCenter, enemy.DashDirection);

        var attackOrigin = enemy.AttackOrigin;
        var toPlayer = targetPoint - attackOrigin;
        var distanceToPlayer = toPlayer.Length();

        if (distanceToPlayer > enemy.Settings.ChaseRange || distanceToPlayer <= 0.001f)
        {
            Reset(enemy);
            enemy.SetState(EnemyState.Idle, isMoving: false);
            return;
        }

        if (_isPerformingFollowUpSwoop)
        {
            UpdateFollowUpSwoop(enemy, playerPosition, playerBounds, frameTime);
            return;
        }

        if (distanceToPlayer > enemy.Settings.SpecialAttackRange || !CanLineUpCone(enemy, playerBounds))
        {
            MoveTowardConeRange(enemy, toPlayer, frameTime);
            return;
        }

        ChargeAndReleaseConeAttack(enemy, playerPosition, playerBounds, frameTime);
    }

    public void Reset(EnemyActor enemy)
    {
        _isChargingConeAttack = false;
        _isPerformingFollowUpSwoop = false;
        _followUpSwoopStarted = false;
        _remainingConeChargeSeconds = 0f;
        enemy.ClearSpecialAttackTelegraph();
        enemy.DashDirection = Direction.Left;
        _followUpBehavior.Reset(enemy);
    }

    private void MoveTowardConeRange(EnemyActor enemy, Vector2 toPlayer, FrameTime frameTime)
    {
        _isChargingConeAttack = false;
        _remainingConeChargeSeconds = enemy.Settings.SpecialAttackPauseSeconds;
        enemy.ClearSpecialAttackTelegraph();

        if (toPlayer.LengthSquared() > 0.0001f)
        {
            toPlayer.Normalize();
            enemy.MoveBy(toPlayer * enemy.Settings.MoveSpeed * frameTime.DeltaSeconds);
        }

        enemy.SetState(EnemyState.Chasing, isMoving: true);
    }

    private void ChargeAndReleaseConeAttack(EnemyActor enemy, Vector2 playerPosition, Rectangle playerBounds, FrameTime frameTime)
    {
        if (!_isChargingConeAttack)
        {
            _isChargingConeAttack = true;
            _remainingConeChargeSeconds = enemy.Settings.SpecialAttackPauseSeconds;
        }

        if (!CanLineUpCone(enemy, playerBounds))
        {
            MoveTowardConeRange(enemy, ResolveTargetPoint(playerPosition, playerBounds) - enemy.AttackOrigin, frameTime);
            return;
        }

        enemy.ShowSpecialAttackTelegraph(enemy.DashDirection);
        enemy.SetState(EnemyState.Aiming, isMoving: false);
        _remainingConeChargeSeconds = Math.Max(0f, _remainingConeChargeSeconds - frameTime.DeltaSeconds);

        if (_remainingConeChargeSeconds > 0f)
        {
            return;
        }

        _isChargingConeAttack = false;
        enemy.ClearSpecialAttackTelegraph();
        var coneAttack = new EnemyAttack(enemy.Settings.SpecialAttackDamage, enemy.Settings.SpecialAttackStunSeconds);
        enemy.TriggerSpecialAttack(enemy.DashDirection, coneAttack);

        var didHit = ConeAttackHitTester.Intersects(enemy, playerBounds);

        if (!didHit)
        {
            enemy.SetState(EnemyState.Chasing, isMoving: false);
            return;
        }

        enemy.QueueAttack(coneAttack);
        _isPerformingFollowUpSwoop = true;
        _followUpSwoopStarted = false;
        _followUpBehavior.Reset(enemy);
        UpdateFollowUpSwoop(enemy, playerPosition, playerBounds, frameTime);
    }

    private void UpdateFollowUpSwoop(EnemyActor enemy, Vector2 playerPosition, Rectangle playerBounds, FrameTime frameTime)
    {
        _followUpBehavior.Update(enemy, playerPosition, playerBounds, frameTime);

        if (enemy.State == EnemyState.Dashing)
        {
            _followUpSwoopStarted = true;
            return;
        }

        if (!_followUpSwoopStarted || enemy.State != EnemyState.Aiming)
        {
            return;
        }

        _isPerformingFollowUpSwoop = false;
        _followUpSwoopStarted = false;
        enemy.ClearSpecialAttackTelegraph();
    }

    private static Direction ResolveFacing(Vector2 movement, Direction fallback)
    {
        return movement.LengthSquared() <= 0.0001f
            ? fallback
            : DirectionHelper.FromDominantAxis(movement, fallback);
    }

    private static bool CanLineUpCone(EnemyActor enemy, Rectangle playerBounds)
    {
        return playerBounds.Width > 0
            && playerBounds.Height > 0
            && ConeAttackHitTester.Intersects(enemy, playerBounds);
    }

    private static Vector2 ResolveTargetPoint(Vector2 playerPosition, Rectangle playerBounds)
    {
        return playerBounds.Width > 0 && playerBounds.Height > 0
            ? new Vector2(playerBounds.Center.X, playerBounds.Center.Y)
            : playerPosition;
    }
}
