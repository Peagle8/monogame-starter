using MyGame.Configuration;
using MyGame.Core.Input;
using MyGame.Gameplay.Player;

namespace MyGame.Tests.Gameplay.Player;

public sealed class PlayerActorTests
{
    [Fact]
    public void Constructor_StartsWithDefaultHealth()
    {
        var player = new PlayerActor(
            new StubInputService(),
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerAttackController(new PlayerAttackSettings()));

        Assert.Equal(5, player.CurrentHealth);
        Assert.Equal(5, player.MaxHealth);
        Assert.False(player.IsDead);
    }

    [Fact]
    public void TakeDamage_ReducesHealthWithoutGoingBelowZero()
    {
        var player = new PlayerActor(
            new StubInputService(),
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerAttackController(new PlayerAttackSettings()));

        player.TakeDamage(2);
        player.TakeDamage(10);

        Assert.Equal(0, player.CurrentHealth);
        Assert.True(player.IsDead);
    }

    [Fact]
    public void Update_WhenAttackPressed_StartsPlayerAttack()
    {
        var player = new PlayerActor(
            new StubInputService(GameAction.Attack),
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerAttackController(new PlayerAttackSettings()));

        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.True(player.IsAttacking);
        Assert.Equal(1, player.AttackSequence);
        Assert.Equal(new Microsoft.Xna.Framework.Rectangle(400, 272, 32, 22), player.AttackBounds);
    }

    private sealed class StubInputService : IInputService
    {
        private readonly HashSet<GameAction> _justPressedActions;

        public StubInputService(params GameAction[] justPressedActions)
        {
            _justPressedActions = justPressedActions.ToHashSet();
        }

        public InputSnapshot Current => InputSnapshot.Empty;

        public InputSnapshot Previous => InputSnapshot.Empty;

        public void Update()
        {
        }

        public bool IsPressed(GameAction action)
        {
            return false;
        }

        public bool IsJustPressed(GameAction action)
        {
            return _justPressedActions.Contains(action);
        }

        public bool IsJustReleased(GameAction action)
        {
            return false;
        }
    }
}
