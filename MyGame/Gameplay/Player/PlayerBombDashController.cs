using Microsoft.Xna.Framework;
using MyGame.Core;

namespace MyGame.Gameplay.Player;

public sealed class PlayerBombDashController
{
    private const int TotalTrailRows = 5;
    private const float BombDropIntervalSeconds = 0.08f;
    private const float BombFuseSeconds = 0.42f;
    private const float BombExplosionDurationSeconds = 0.18f;
    private const int BombSize = 12;
    private const float DefaultTrailRowSpacing = 18f;
    private const int BombPairSpacing = 14;
    private const int BombExplosionPadding = 10;
    private const int BombDamage = 1;

    public PlayerBombDashUpdateResult Update(
        PlayerBombTrailState currentState,
        PlayerDashState dashState,
        Rectangle playerBounds,
        bool canSpawnBombs,
        FrameTime frameTime)
    {
        if (!canSpawnBombs)
        {
            return new PlayerBombDashUpdateResult(
                PlayerBombTrailState.Default with
                {
                    DashSequence = dashState.DashSequence
                },
                []);
        }

        if (dashState.DashSequence != currentState.DashSequence)
        {
            var dropIntervalSeconds = ResolveDropIntervalSeconds(dashState);
            var rowCenter = GetBoundsCenter(playerBounds);
            return new PlayerBombDashUpdateResult(
                new PlayerBombTrailState(
                    dashState.DashSequence,
                    dropIntervalSeconds,
                    dropIntervalSeconds,
                    1,
                    rowCenter,
                    Vector2.Zero),
                CreateBombPair(rowCenter, dashState.Direction));
        }

        if (currentState.SpawnedRowCount >= TotalTrailRows)
        {
            return new PlayerBombDashUpdateResult(
                PlayerBombTrailState.Default with { DashSequence = currentState.DashSequence },
                []);
        }

        var remainingDropSeconds = currentState.RemainingDropSeconds - frameTime.DeltaSeconds;
        if (remainingDropSeconds > 0f)
        {
            return new PlayerBombDashUpdateResult(
                currentState with { RemainingDropSeconds = remainingDropSeconds },
                []);
        }

        var spawnedBombs = new List<PlayerBomb>();
        var spawnedRowCount = currentState.SpawnedRowCount;
        var lastRowCenter = currentState.LastRowCenter;
        var lastRowStep = currentState.LastRowStep;
        while (remainingDropSeconds <= 0f && spawnedRowCount < TotalTrailRows)
        {
            var nextRowCenter = ResolveNextRowCenter(
                dashState,
                playerBounds,
                spawnedRowCount,
                lastRowCenter,
                lastRowStep);
            spawnedBombs.AddRange(CreateBombPair(nextRowCenter, dashState.Direction));
            spawnedRowCount++;
            lastRowStep = nextRowCenter - lastRowCenter;
            lastRowCenter = nextRowCenter;
            remainingDropSeconds += currentState.DropIntervalSeconds;
        }

        return new PlayerBombDashUpdateResult(
            currentState with
            {
                RemainingDropSeconds = remainingDropSeconds,
                SpawnedRowCount = spawnedRowCount,
                LastRowCenter = lastRowCenter,
                LastRowStep = lastRowStep
            },
            spawnedBombs);
    }

    private static float ResolveDropIntervalSeconds(PlayerDashState dashState)
    {
        return Math.Max(0.01f, dashState.RemainingActiveSeconds / (TotalTrailRows - 1));
    }

    private static Vector2 ResolveNextRowCenter(
        PlayerDashState dashState,
        Rectangle playerBounds,
        int spawnedRowCount,
        Vector2 lastRowCenter,
        Vector2 lastRowStep)
    {
        if (dashState.IsDashing)
        {
            var currentCenter = GetBoundsCenter(playerBounds);
            if (spawnedRowCount <= 0)
            {
                return currentCenter;
            }

            return currentCenter;
        }

        var step = lastRowStep;
        if (step.LengthSquared() <= 0.0001f)
        {
            step = DirectionHelper.ToVector(dashState.Direction) * DefaultTrailRowSpacing;
        }

        return lastRowCenter + step;
    }

    private static Vector2 GetBoundsCenter(Rectangle playerBounds)
    {
        return new Vector2(playerBounds.Center.X, playerBounds.Center.Y);
    }

    private static IReadOnlyList<PlayerBomb> CreateBombPair(Vector2 rowCenter, Direction dashDirection)
    {
        var offset = GetPerpendicularOffset(dashDirection);
        return
        [
            CreateBomb(rowCenter, new Point(-offset.X, -offset.Y)),
            CreateBomb(rowCenter, offset)
        ];
    }

    private static Point GetPerpendicularOffset(Direction dashDirection)
    {
        return dashDirection switch
        {
            Direction.Left or Direction.Right => new Point(0, BombPairSpacing / 2),
            _ => new Point(BombPairSpacing / 2, 0)
        };
    }

    private static PlayerBomb CreateBomb(Vector2 rowCenter, Point offset)
    {
        var bombBounds = new Rectangle(
            (int)MathF.Round(rowCenter.X) - (BombSize / 2) + offset.X,
            (int)MathF.Round(rowCenter.Y) - (BombSize / 2) + offset.Y,
            BombSize,
            BombSize);
        return new PlayerBomb(
            bombBounds,
            BombDamage,
            BombFuseSeconds,
            BombExplosionDurationSeconds,
            BombExplosionPadding);
    }
}
