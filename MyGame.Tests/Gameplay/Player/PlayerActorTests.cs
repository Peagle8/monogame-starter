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
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash]),
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
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash]),
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
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));

        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.True(player.IsAttacking);
        Assert.Equal(1, player.AttackSequence);
        Assert.Equal(new Microsoft.Xna.Framework.Rectangle(400, 272, 32, 22), player.AttackBounds);
    }

    [Fact]
    public void ApplyKnockback_MovesPlayerImmediatelyAndStartsRecoil()
    {
        var player = new PlayerActor(
            new StubInputService(),
            new PlayerMovementController(new PlayerMovementSettings { ContactKnockbackDistance = 20f, ContactKnockbackSeconds = 0.2f }),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));

        player.ApplyKnockback(new Microsoft.Xna.Framework.Vector2(1f, 0f));

        Assert.Equal(new Microsoft.Xna.Framework.Vector2(410f, 240f), player.Position);
        Assert.True(player.IsRecoiling);
    }

    [Fact]
    public void Update_WhenRecoiling_ContinuesKnockbackAndSkipsInputMovement()
    {
        var player = new PlayerActor(
            new StubInputService(),
            new PlayerMovementController(new PlayerMovementSettings { ContactKnockbackDistance = 20f, ContactKnockbackSeconds = 0.2f, MoveSpeed = 180f }),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));
        player.ApplyKnockback(new Microsoft.Xna.Framework.Vector2(1f, 0f));

        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.Equal(new Microsoft.Xna.Framework.Vector2(415f, 240f), player.Position);
        Assert.True(player.IsRecoiling);
    }

    [Fact]
    public void Update_WhenDashPressed_StartsDashing()
    {
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.Dash),
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings { DashDistance = 72f, DashSeconds = 0.18f }),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));

        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.True(player.IsDashing);
        Assert.True(player.Position.Y > 240f);
    }

    [Fact]
    public void Update_WhenDashPressedWithoutDashAbility_DoesNotStartDashing()
    {
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.Dash),
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings { DashDistance = 72f, DashSeconds = 0.18f }),
            new PlayerAbilityService(),
            new PlayerAttackController(new PlayerAttackSettings()));

        player.Update(new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));

        Assert.False(player.IsDashing);
        Assert.Equal(new Microsoft.Xna.Framework.Vector2(400f, 240f), player.Position);
    }

    private sealed class StubInputService : IInputService
    {
        private readonly HashSet<GameAction> _justPressedActions;
        private readonly InputSnapshot _current;

        public StubInputService(params GameAction[] justPressedActions)
        {
            _current = InputSnapshot.Empty;
            _justPressedActions = justPressedActions.ToHashSet();
        }

        public StubInputService(InputSnapshot current, params GameAction[] justPressedActions)
        {
            _current = current;
            _justPressedActions = justPressedActions.ToHashSet();
        }

        public InputSnapshot Current => _current;

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
