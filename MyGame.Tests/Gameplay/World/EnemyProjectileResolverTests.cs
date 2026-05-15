using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Core;
using MyGame.Core.Input;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.Props;
using MyGame.Gameplay.World;

namespace MyGame.Tests.Gameplay.World;

public sealed class EnemyProjectileResolverTests
{
    [Fact]
    public void Resolve_WhenArrowHitsPlayer_DealsDamageAndDeactivatesProjectile()
    {
        var player = CreatePlayer();
        var resolver = new EnemyProjectileResolver();
        var projectile = new EnemyProjectile(new Vector2(100f, 100f), new Vector2(200f, 0f), 1f, 10, 1);

        projectile.Update(new FrameTime(TimeSpan.FromSeconds(0.15), TimeSpan.FromSeconds(0.15)));
        var hitPlayer = resolver.Resolve([projectile], player, []);

        Assert.True(hitPlayer);
        Assert.Equal(player.MaxHealth - 1, player.CurrentHealth);
        Assert.False(projectile.IsActive);
    }

    [Fact]
    public void Resolve_WhenArrowHitsShield_ConsumesShieldChargeWithoutDamage()
    {
        var player = CreateShieldedPlayer();
        var resolver = new EnemyProjectileResolver();
        var projectile = new EnemyProjectile(new Vector2(100f, 100f), new Vector2(200f, 0f), 1f, 10, 1);

        projectile.Update(new FrameTime(TimeSpan.FromSeconds(0.15), TimeSpan.FromSeconds(0.15)));
        var hitPlayer = resolver.Resolve([projectile], player, []);

        Assert.False(hitPlayer);
        Assert.Equal(player.MaxHealth, player.CurrentHealth);
        Assert.Equal(2, player.ShieldCharges);
        Assert.False(projectile.IsActive);
    }

    [Fact]
    public void Resolve_WhenArrowHitsBlockingProp_DeactivatesProjectile()
    {
        var player = CreatePlayer();
        var resolver = new EnemyProjectileResolver();
        var projectile = new EnemyProjectile(new Vector2(100f, 100f), new Vector2(200f, 0f), 1f, 10, 1);
        var props = new IWorldProp[]
        {
            new WallProp(new Vector2(120f, 96f), new Point(24, 24))
        };

        projectile.Update(new FrameTime(TimeSpan.FromSeconds(0.15), TimeSpan.FromSeconds(0.15)));
        var hitPlayer = resolver.Resolve([projectile], player, props);

        Assert.False(hitPlayer);
        Assert.Equal(player.MaxHealth, player.CurrentHealth);
        Assert.False(projectile.IsActive);
    }

    private static PlayerActor CreatePlayer()
    {
        var player = new PlayerActor(
            new StubInputService(),
            new PlayerCombatSettings(),
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()));
        player.RestoreState(new Vector2(128f, 100f), player.MaxHealth);
        return player;
    }

    private static PlayerActor CreateShieldedPlayer()
    {
        var inputService = new StubInputService(GameAction.DefenseAbility);
        var player = new PlayerActor(
            inputService,
            new PlayerCombatSettings { MaxAbilityPoints = 3f, AbilityPointRegenPerSecond = 0f },
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash]),
            new PlayerAttackController(new PlayerAttackSettings()),
            new PlayerDefenseAbilityController(new PlayerDefenseAbilitySettings()),
            new PlayerRangedAttackController(new PlayerRangedAttackSettings()));
        player.RestoreState(new Vector2(128f, 100f), player.MaxHealth, player.MaxAbilityPoints);
        player.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        return player;
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

        public bool IsPressed(GameAction action)
        {
            return false;
        }

        public bool IsJustPressed(GameAction action)
        {
            return _justPressedActions.Remove(action);
        }

        public bool IsJustReleased(GameAction action)
        {
            return false;
        }

        public void Update()
        {
        }
    }
}
