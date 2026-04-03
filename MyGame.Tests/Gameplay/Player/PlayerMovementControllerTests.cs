using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Core;
using MyGame.Core.Input;
using MyGame.Gameplay.Player;

namespace MyGame.Tests.Gameplay.Player;

public sealed class PlayerMovementControllerTests
{
    [Fact]
    public void Update_UsesConfiguredMoveSpeed()
    {
        var controller = new PlayerMovementController(new PlayerMovementSettings { MoveSpeed = 240f });

        var result = controller.Update(
            new Vector2(10f, 20f),
            Direction.Down,
            new InputSnapshot(new HashSet<GameAction> { GameAction.MoveRight }),
            new FrameTime(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(1)));

        Assert.Equal(new Vector2(130f, 20f), result.Position);
        Assert.Equal(Direction.Right, result.Facing);
    }

    [Fact]
    public void Update_NormalizesDiagonalMovement()
    {
        var controller = new PlayerMovementController(new PlayerMovementSettings { MoveSpeed = 100f });

        var result = controller.Update(
            Vector2.Zero,
            Direction.Down,
            new InputSnapshot(new HashSet<GameAction> { GameAction.MoveUp, GameAction.MoveRight }),
            new FrameTime(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)));

        Assert.Equal(new Vector2(70.71068f, -70.71068f), result.Position, new Vector2Comparer(0.0001f));
        Assert.Equal(Direction.Right, result.Facing);
    }

    private sealed class Vector2Comparer : IEqualityComparer<Vector2>
    {
        private readonly float _tolerance;

        public Vector2Comparer(float tolerance)
        {
            _tolerance = tolerance;
        }

        public bool Equals(Vector2 x, Vector2 y)
        {
            return MathF.Abs(x.X - y.X) <= _tolerance
                && MathF.Abs(x.Y - y.Y) <= _tolerance;
        }

        public int GetHashCode(Vector2 obj)
        {
            return HashCode.Combine(obj.X, obj.Y);
        }
    }
}
