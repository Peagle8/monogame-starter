using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Gameplay.Player;

namespace MyGame.Tests.Gameplay.Player;

public sealed class PlayerBombDashControllerTests
{
    [Fact]
    public void Update_WhenDashSequenceStarts_SpawnsInitialBomb()
    {
        var controller = new PlayerBombDashController();

        var result = controller.Update(
            PlayerBombTrailState.Default,
            new PlayerDashState(true, Direction.Right, 1, 0.18f, 0.35f),
            new Rectangle(100, 120, 32, 32),
            canSpawnBombs: true,
            new FrameTime(TimeSpan.FromSeconds(0.01), TimeSpan.FromSeconds(0.01)));

        Assert.Equal(2, result.SpawnedBombs.Count);
        Assert.Equal(new Rectangle(110, 123, 12, 12), result.SpawnedBombs[0].Bounds);
        Assert.Equal(new Rectangle(110, 137, 12, 12), result.SpawnedBombs[1].Bounds);
        Assert.Equal(1, result.State.DashSequence);
        Assert.Equal(1, result.State.SpawnedRowCount);
        Assert.Equal(new Vector2(116f, 136f), result.State.LastRowCenter);
    }

    [Fact]
    public void Update_WhenDashContinuesPastDropInterval_SpawnsAdditionalTrailRows()
    {
        var controller = new PlayerBombDashController();
        var initialState = new PlayerBombTrailState(1, 0.045f, 0.045f, 1, new Vector2(116f, 136f), Vector2.Zero);

        var result = controller.Update(
            initialState,
            new PlayerDashState(true, Direction.Right, 1, 0.10f, 0.35f),
            new Rectangle(140, 120, 32, 32),
            canSpawnBombs: true,
            new FrameTime(TimeSpan.FromSeconds(0.09), TimeSpan.FromSeconds(0.09)));

        Assert.Equal(4, result.SpawnedBombs.Count);
        Assert.Equal(new Rectangle(150, 123, 12, 12), result.SpawnedBombs[0].Bounds);
        Assert.Equal(new Rectangle(150, 137, 12, 12), result.SpawnedBombs[1].Bounds);
        Assert.Equal(3, result.State.SpawnedRowCount);
    }

    [Fact]
    public void Update_WhenBombDashIsUnavailable_DoesNotSpawnBombs()
    {
        var controller = new PlayerBombDashController();

        var result = controller.Update(
            PlayerBombTrailState.Default,
            new PlayerDashState(true, Direction.Right, 1, 0.18f, 0.35f),
            new Rectangle(100, 120, 32, 32),
            canSpawnBombs: false,
            new FrameTime(TimeSpan.FromSeconds(0.05), TimeSpan.FromSeconds(0.05)));

        Assert.Empty(result.SpawnedBombs);
        Assert.Equal(1, result.State.DashSequence);
        Assert.Equal(0f, result.State.RemainingDropSeconds);
        Assert.Equal(0, result.State.SpawnedRowCount);
    }

    [Fact]
    public void Update_AfterDashEnds_CompletesRemainingTrailRows()
    {
        var controller = new PlayerBombDashController();
        var currentState = new PlayerBombTrailState(2, 0.01f, 0.045f, 3, new Vector2(136f, 196f), new Vector2(0f, 18f));

        var result = controller.Update(
            currentState,
            new PlayerDashState(false, Direction.Down, 2, 0f, 0.30f),
            new Rectangle(120, 150, 32, 32),
            canSpawnBombs: true,
            new FrameTime(TimeSpan.FromSeconds(0.10), TimeSpan.FromSeconds(0.25)));

        Assert.Equal(4, result.SpawnedBombs.Count);
        Assert.Equal(5, result.State.SpawnedRowCount);
        Assert.Equal(new Rectangle(123, 208, 12, 12), result.SpawnedBombs[0].Bounds);
        Assert.Equal(new Rectangle(137, 208, 12, 12), result.SpawnedBombs[1].Bounds);
    }
}
