using Microsoft.Xna.Framework;
using MyGame.Gameplay.Player;

namespace MyGame.Tests.Gameplay.Player;

public sealed class DirectionHelperTests
{
    [Theory]
    [InlineData(Direction.Up, 0f, -1f)]
    [InlineData(Direction.Down, 0f, 1f)]
    [InlineData(Direction.Left, -1f, 0f)]
    [InlineData(Direction.Right, 1f, 0f)]
    public void ToVector_ReturnsExpectedUnitVector(Direction direction, float expectedX, float expectedY)
    {
        var vector = DirectionHelper.ToVector(direction);

        Assert.Equal(new Vector2(expectedX, expectedY), vector);
    }

    [Fact]
    public void FromDominantAxis_WhenVectorIsZero_UsesFallback()
    {
        var direction = DirectionHelper.FromDominantAxis(Vector2.Zero, Direction.Left);

        Assert.Equal(Direction.Left, direction);
    }

    [Theory]
    [InlineData(3f, 1f, Direction.Right)]
    [InlineData(-3f, 1f, Direction.Left)]
    [InlineData(1f, 3f, Direction.Down)]
    [InlineData(1f, -3f, Direction.Up)]
    public void FromDominantAxis_UsesLargestAxis(float x, float y, Direction expected)
    {
        var direction = DirectionHelper.FromDominantAxis(new Vector2(x, y), Direction.Down);

        Assert.Equal(expected, direction);
    }
}
