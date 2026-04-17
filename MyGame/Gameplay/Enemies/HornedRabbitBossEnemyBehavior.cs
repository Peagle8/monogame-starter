using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.World;

namespace MyGame.Gameplay.Enemies;

internal sealed class HornedRabbitBossEnemyBehavior : IEnemyBehavior
{
    private const int StageCount = 3;
    private const float AttackPauseSeconds = 1.1f;
    private const float StageThreeAttackPauseSeconds = 0.62f;
    private const int BombColumns = 3;
    private const int BombRows = 2;
    private const int BombSize = 14;
    private const int BombExplosionPadding = 14;
    private const float BombFuseStartSeconds = 0.28f;
    private const float BombFuseStepSeconds = 0.08f;
    private const float BombExplosionDurationSeconds = 0.22f;
    private const float StageTwoDashSpeedMultiplier = 1.15f;
    private const float StageThreeLeapSpeedMultiplier = 1.25f;
    private const float StageThreeDashSpeedMultiplier = 1.45f;
    private static readonly Rectangle ArenaPlayArea = new(115, 106, 730, 365);

    private Vector2 _movementTarget;
    private Rectangle _targetQuadrant;
    private float _remainingPauseSeconds = AttackPauseSeconds;
    private float _remainingStagePowerUpSeconds;
    private bool _isLeaping;
    private bool _isStageTwoArenaDash;
    private bool _hasSpawnedStageTwoMinions;
    private bool _hasSpawnedStageThreeElites;
    private bool _shouldUseStageTwoDashNext;

    public void Update(EnemyActor enemy, Vector2 playerPosition, Rectangle playerBounds, FrameTime frameTime)
    {
        SyncStage(enemy);

        if (enemy.IsBossStageTransitioning)
        {
            enemy.SetState(EnemyState.Aiming, isMoving: false);
            return;
        }

        if (_remainingStagePowerUpSeconds > 0f)
        {
            _remainingStagePowerUpSeconds = Math.Max(0f, _remainingStagePowerUpSeconds - frameTime.DeltaSeconds);
            enemy.SetState(EnemyState.Aiming, isMoving: false);
            return;
        }

        if (_isLeaping)
        {
            ContinueMovement(enemy, frameTime, GetLeapSpeed(enemy), OnLeapLanded);
            return;
        }

        if (_isStageTwoArenaDash)
        {
            ContinueMovement(enemy, frameTime, GetArenaDashSpeed(enemy), OnStageTwoDashFinished);
            return;
        }

        if (_remainingPauseSeconds > 0f)
        {
            _remainingPauseSeconds = Math.Max(0f, _remainingPauseSeconds - frameTime.DeltaSeconds);
            enemy.SetState(EnemyState.Aiming, isMoving: false);
            return;
        }

        if (enemy.BossStage >= 2 && _shouldUseStageTwoDashNext)
        {
            BeginStageTwoArenaDash(enemy, playerPosition);
            return;
        }

        BeginLeap(enemy, playerBounds);
    }

    public void Reset(EnemyActor enemy)
    {
        _movementTarget = enemy.Position;
        _targetQuadrant = Rectangle.Empty;
        _remainingPauseSeconds = AttackPauseSeconds;
        _remainingStagePowerUpSeconds = 0f;
        _isLeaping = false;
        _isStageTwoArenaDash = false;
        _hasSpawnedStageTwoMinions = false;
        _hasSpawnedStageThreeElites = false;
        _shouldUseStageTwoDashNext = false;
        enemy.SetBossStage(1, StageCount);
        enemy.SetState(EnemyState.Idle, isMoving: false);
    }

    private void SyncStage(EnemyActor enemy)
    {
        if (!enemy.TryConsumePendingBossStageTransition())
        {
            return;
        }

        _isLeaping = false;
        _isStageTwoArenaDash = false;
        _remainingPauseSeconds = GetAttackPauseSeconds(enemy.BossStage);
        _remainingStagePowerUpSeconds = EnemyActor.BossStageTransitionSeconds;
        _shouldUseStageTwoDashNext = false;
        enemy.SetState(EnemyState.Aiming, isMoving: false);
    }

    private void BeginLeap(EnemyActor enemy, Rectangle playerBounds)
    {
        _targetQuadrant = GetQuadrant(playerBounds);
        _movementTarget = GetLeapTarget(enemy, playerBounds, _targetQuadrant);
        enemy.DashDirection = DirectionHelper.FromDominantAxis(_movementTarget - enemy.Position, enemy.DashDirection);
        _isLeaping = true;
        enemy.SetState(EnemyState.Dashing, isMoving: true);
    }

    private void BeginStageTwoArenaDash(EnemyActor enemy, Vector2 playerPosition)
    {
        var dashDirection = GetStageTwoDashDirection(enemy, playerPosition);
        enemy.DashDirection = dashDirection;
        _movementTarget = GetArenaDashTarget(enemy, dashDirection);
        _isStageTwoArenaDash = true;
        enemy.SetState(EnemyState.Dashing, isMoving: true);
    }

    private void ContinueMovement(EnemyActor enemy, FrameTime frameTime, float speed, Action<EnemyActor> onArrive)
    {
        var toTarget = _movementTarget - enemy.Position;
        var distanceToTarget = toTarget.Length();
        var maxStep = speed * frameTime.DeltaSeconds;
        if (distanceToTarget <= Math.Max(maxStep, 0.001f))
        {
            enemy.MoveBy(toTarget);
            onArrive(enemy);
            return;
        }

        toTarget.Normalize();
        enemy.MoveBy(toTarget * maxStep);
        enemy.SetState(EnemyState.Dashing, isMoving: true);
    }

    private void OnLeapLanded(EnemyActor enemy)
    {
        _isLeaping = false;
        DropBombSpread(enemy, _targetQuadrant);
        if (enemy.BossStage >= 2)
        {
            SpawnStageTwoMinionsIfNeeded(enemy);
            SpawnStageThreeElitesIfNeeded(enemy);
            _shouldUseStageTwoDashNext = true;
        }

        _remainingPauseSeconds = GetAttackPauseSeconds(enemy.BossStage);
        enemy.SetState(EnemyState.Aiming, isMoving: false);
    }

    private void OnStageTwoDashFinished(EnemyActor enemy)
    {
        _isStageTwoArenaDash = false;
        _shouldUseStageTwoDashNext = false;
        _remainingPauseSeconds = GetAttackPauseSeconds(enemy.BossStage);
        enemy.SetState(EnemyState.Aiming, isMoving: false);
    }

    private void DropBombSpread(EnemyActor enemy, Rectangle quadrant)
    {
        var bombArea = GetBombArea(enemy, quadrant);
        var bombBounds = CreateBombBounds(bombArea).ToArray();
        var enemyCenter = new Vector2(enemy.Bounds.Center.X, enemy.Bounds.Center.Y);
        Array.Sort(
            bombBounds,
            (left, right) => Vector2.DistanceSquared(GetCenter(left), enemyCenter)
                .CompareTo(Vector2.DistanceSquared(GetCenter(right), enemyCenter)));

        for (var index = 0; index < bombBounds.Length; index++)
        {
            enemy.DropBomb(
                bombBounds[index],
                new EnemyAttack(1, 0f),
                BombFuseStartSeconds + (BombFuseStepSeconds * index),
                BombExplosionDurationSeconds,
                BombExplosionPadding);
        }
    }

    private void SpawnStageTwoMinionsIfNeeded(EnemyActor enemy)
    {
        if (_hasSpawnedStageTwoMinions || enemy.BossStage < 2)
        {
            return;
        }

        _hasSpawnedStageTwoMinions = true;
        foreach (var spawn in CreateStageTwoMinionSpawns(enemy))
        {
            enemy.QueueSpawnEnemy(spawn);
        }
    }

    private void SpawnStageThreeElitesIfNeeded(EnemyActor enemy)
    {
        if (_hasSpawnedStageThreeElites || enemy.BossStage < 3)
        {
            return;
        }

        _hasSpawnedStageThreeElites = true;
        foreach (var spawn in CreateStageThreeReinforcementSpawns(enemy))
        {
            enemy.QueueSpawnEnemy(spawn);
        }
    }

    private static IEnumerable<EnemySpawnDefinition> CreateStageTwoMinionSpawns(EnemyActor enemy)
    {
        var spawnPositions = GetOppositeSideSpawnPositions(enemy);
        yield return new EnemySpawnDefinition(
            EnemyKind.HornedRabbit,
            spawnPositions.MinionTop,
            EnemyAxisPreference.Horizontal);
        yield return new EnemySpawnDefinition(
            EnemyKind.HornedRabbit,
            spawnPositions.MinionMiddle,
            EnemyAxisPreference.Vertical);
        yield return new EnemySpawnDefinition(
            EnemyKind.HornedRabbit,
            spawnPositions.MinionBottom,
            EnemyAxisPreference.None);
    }

    private static IEnumerable<EnemySpawnDefinition> CreateStageThreeReinforcementSpawns(EnemyActor enemy)
    {
        var spawnPositions = GetOppositeSideSpawnPositions(enemy);
        yield return new EnemySpawnDefinition(
            EnemyKind.HornedRabbitElite,
            spawnPositions.EliteTop,
            EnemyAxisPreference.None);
        yield return new EnemySpawnDefinition(
            EnemyKind.HornedRabbitElite,
            spawnPositions.EliteBottom,
            EnemyAxisPreference.None);
        yield return new EnemySpawnDefinition(
            EnemyKind.HornedRabbit,
            spawnPositions.MinionTop,
            EnemyAxisPreference.Horizontal);
        yield return new EnemySpawnDefinition(
            EnemyKind.HornedRabbit,
            spawnPositions.MinionMiddle,
            EnemyAxisPreference.Vertical);
        yield return new EnemySpawnDefinition(
            EnemyKind.HornedRabbit,
            spawnPositions.MinionBottom,
            EnemyAxisPreference.None);
    }

    private static IEnumerable<Rectangle> CreateBombBounds(Rectangle bombArea)
    {
        var xSpacing = BombColumns == 1 ? 0f : (bombArea.Width - BombSize) / (float)(BombColumns - 1);
        var ySpacing = BombRows == 1 ? 0f : (bombArea.Height - BombSize) / (float)(BombRows - 1);

        for (var row = 0; row < BombRows; row++)
        {
            for (var column = 0; column < BombColumns; column++)
            {
                yield return new Rectangle(
                    (int)MathF.Round(bombArea.Left + (column * xSpacing)),
                    (int)MathF.Round(bombArea.Top + (row * ySpacing)),
                    BombSize,
                    BombSize);
            }
        }
    }

    private static Rectangle GetBombArea(EnemyActor enemy, Rectangle quadrant)
    {
        var areaWidth = quadrant.Width / 2;
        var areaHeight = quadrant.Height / 2;
        var targetLeft = ClampToRange(
            enemy.Bounds.Center.X - (areaWidth / 2),
            quadrant.Left + 12,
            quadrant.Right - areaWidth - 12);
        var targetTop = ClampToRange(
            enemy.Bounds.Center.Y - (areaHeight / 2),
            quadrant.Top + 12,
            quadrant.Bottom - areaHeight - 12);

        return new Rectangle(targetLeft, targetTop, areaWidth, areaHeight);
    }

    private static Vector2 GetLeapTarget(EnemyActor enemy, Rectangle playerBounds, Rectangle quadrant)
    {
        var inset = 28;
        var minX = quadrant.Left + inset;
        var maxX = quadrant.Right - enemy.Bounds.Width - inset;
        var minY = quadrant.Top + inset;
        var maxY = quadrant.Bottom - enemy.Bounds.Height - inset;

        return new Vector2(
            ClampToRange(playerBounds.Center.X - (enemy.Bounds.Width / 2), minX, maxX),
            ClampToRange(playerBounds.Center.Y - (enemy.Bounds.Height / 2), minY, maxY));
    }

    private static Rectangle GetQuadrant(Rectangle playerBounds)
    {
        var playerCenter = new Point(playerBounds.Center.X, playerBounds.Center.Y);
        var isRight = playerCenter.X >= ArenaPlayArea.Center.X;
        var isBottom = playerCenter.Y >= ArenaPlayArea.Center.Y;

        return new Rectangle(
            isRight ? ArenaPlayArea.Center.X : ArenaPlayArea.Left,
            isBottom ? ArenaPlayArea.Center.Y : ArenaPlayArea.Top,
            ArenaPlayArea.Width / 2,
            ArenaPlayArea.Height / 2);
    }

    private static Direction GetStageTwoDashDirection(EnemyActor enemy, Vector2 playerPosition)
    {
        var toPlayer = playerPosition - enemy.Position;
        if (Math.Abs(toPlayer.X) >= Math.Abs(toPlayer.Y))
        {
            return toPlayer.X < 0f ? Direction.Left : Direction.Right;
        }

        return toPlayer.Y < 0f ? Direction.Up : Direction.Down;
    }

    private static Vector2 GetArenaDashTarget(EnemyActor enemy, Direction direction)
    {
        const int inset = 24;
        return direction switch
        {
            Direction.Left => new Vector2(ArenaPlayArea.Left + inset, enemy.Position.Y),
            Direction.Right => new Vector2(ArenaPlayArea.Right - enemy.Bounds.Width - inset, enemy.Position.Y),
            Direction.Up => new Vector2(enemy.Position.X, ArenaPlayArea.Top + inset),
            Direction.Down => new Vector2(enemy.Position.X, ArenaPlayArea.Bottom - enemy.Bounds.Height - inset),
            _ => enemy.Position
        };
    }

    private static Vector2 GetCenter(Rectangle bounds)
    {
        return new Vector2(bounds.Center.X, bounds.Center.Y);
    }

    private static float GetAttackPauseSeconds(int bossStage)
    {
        return bossStage >= 3 ? StageThreeAttackPauseSeconds : AttackPauseSeconds;
    }

    private static float GetLeapSpeed(EnemyActor enemy)
    {
        return enemy.BossStage >= 3
            ? enemy.Settings.DashSpeed * StageThreeLeapSpeedMultiplier
            : enemy.Settings.DashSpeed;
    }

    private static float GetArenaDashSpeed(EnemyActor enemy)
    {
        return enemy.BossStage >= 3
            ? enemy.Settings.DashSpeed * StageThreeDashSpeedMultiplier
            : enemy.Settings.DashSpeed * StageTwoDashSpeedMultiplier;
    }

    private static ArenaSpawnPositions GetOppositeSideSpawnPositions(EnemyActor enemy)
    {
        var spawnOnRight = enemy.Bounds.Center.X < ArenaPlayArea.Center.X;
        var laneX = spawnOnRight ? ArenaPlayArea.Right - 120f : ArenaPlayArea.Left + 40f;
        var midX = spawnOnRight ? ArenaPlayArea.Right - 184f : ArenaPlayArea.Left + 104f;
        var eliteX = spawnOnRight ? ArenaPlayArea.Right - 72f : ArenaPlayArea.Left - 8f;
        var topY = ArenaPlayArea.Top + 16f;
        var middleY = ArenaPlayArea.Center.Y - 16f;
        var bottomY = ArenaPlayArea.Bottom - 56f;
        var eliteTopY = ArenaPlayArea.Top + 64f;
        var eliteBottomY = ArenaPlayArea.Bottom - 104f;

        return new ArenaSpawnPositions(
            new Vector2(laneX, topY),
            new Vector2(midX, middleY),
            new Vector2(laneX, bottomY),
            new Vector2(eliteX, eliteTopY),
            new Vector2(eliteX, eliteBottomY));
    }

    private readonly record struct ArenaSpawnPositions(
        Vector2 MinionTop,
        Vector2 MinionMiddle,
        Vector2 MinionBottom,
        Vector2 EliteTop,
        Vector2 EliteBottom);

    private static int ClampToRange(float value, int min, int max)
    {
        if (max < min)
        {
            return min;
        }

        return (int)MathF.Round(Math.Clamp(value, min, max));
    }
}
