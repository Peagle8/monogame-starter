using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Core;
using MyGame.Core.Input;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.World;

namespace MyGame.Tests.Gameplay.World;

public sealed class PlayerFireShieldResolverTests
{
    [Fact]
    public void Resolve_WhenEnemyRemainsInAura_DealsDamageEachTickInterval()
    {
        var resolver = new PlayerFireShieldResolver(new PlayerDefenseAbilitySettings
        {
            FireShieldActivationCost = 1f,
            FireShieldDamageTickSeconds = 3f
        });
        var player = CreateActiveFireShieldPlayer();
        var enemy = new EnemyActor(
            new EnemySettings { MaxHealth = 4, MoveSpeed = 0f, ChaseRange = 10f },
            new Vector2(500f, 240f));

        var hitEnemy = resolver.Resolve(player, [enemy], new FrameTime(TimeSpan.FromSeconds(3.1), TimeSpan.FromSeconds(3.1)));

        Assert.True(hitEnemy);
        Assert.Equal(3, enemy.CurrentHealth);

        resolver.Resolve(player, [enemy], new FrameTime(TimeSpan.FromSeconds(3.0), TimeSpan.FromSeconds(6.1)));
        Assert.Equal(2, enemy.CurrentHealth);
    }

    [Fact]
    public void Resolve_WhenEnemyLeavesAura_ResetsExposureTimer()
    {
        var resolver = new PlayerFireShieldResolver(new PlayerDefenseAbilitySettings
        {
            FireShieldActivationCost = 1f,
            FireShieldDamageTickSeconds = 3f
        });
        var player = CreateActiveFireShieldPlayer();
        var enemy = new EnemyActor(
            new EnemySettings { MaxHealth = 4, MoveSpeed = 0f, ChaseRange = 10f },
            new Vector2(500f, 240f));

        resolver.Resolve(player, [enemy], new FrameTime(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0)));
        Assert.Equal(4, enemy.CurrentHealth);

        enemy.RestoreState(new Vector2(760f, 240f), enemy.CurrentHealth);
        resolver.Resolve(player, [enemy], new FrameTime(TimeSpan.FromSeconds(0.2), TimeSpan.FromSeconds(2.2)));

        enemy.RestoreState(new Vector2(500f, 240f), enemy.CurrentHealth);
        resolver.Resolve(player, [enemy], new FrameTime(TimeSpan.FromSeconds(1.1), TimeSpan.FromSeconds(3.3)));
        Assert.Equal(4, enemy.CurrentHealth);

        resolver.Resolve(player, [enemy], new FrameTime(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(5.3)));
        Assert.Equal(3, enemy.CurrentHealth);
    }

    private static PlayerActor CreateActiveFireShieldPlayer()
    {
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty, GameAction.DefenseAbility),
            new PlayerCombatSettings { MaxAbilityPoints = 1f, AbilityPointRegenPerSecond = 0f },
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash, PlayerAbility.Fireball]),
            new PlayerAttackController(new PlayerAttackSettings()),
            new PlayerDefenseAbilityController(new PlayerDefenseAbilitySettings
            {
                FireShieldActivationCost = 1f
            }),
            new PlayerRangedAttackController(new PlayerRangedAttackSettings()));
        player.EquipDefenseAbility(PlayerDefenseAbilityKind.FireShield);
        player.Update(new FrameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)));
        return player;
    }

    private sealed class StubInputService : IInputService
    {
        private readonly HashSet<GameAction> _justPressedActions;
        private readonly InputSnapshot _current;

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
            return _justPressedActions.Remove(action);
        }

        public bool IsJustReleased(GameAction action)
        {
            return false;
        }
    }
}
