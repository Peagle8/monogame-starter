using MyGame.Gameplay.Inventory;
using MyGame.Gameplay.Player;
using MyGame.Configuration;

namespace MyGame.Tests.Gameplay.Inventory;

public sealed class AbilityLoadoutCatalogTests
{
    [Fact]
    public void CreateOptionViewModels_ForDefenseSlot_EnablesFireShield()
    {
        var player = new PlayerActor(
            new StubInputService(),
            new PlayerCombatSettings(),
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash, PlayerAbility.Fireball]),
            new PlayerAttackController(new PlayerAttackSettings()));

        var options = AbilityLoadoutCatalog.CreateOptionViewModels(player, AbilityLoadoutSlot.Defense);
        var fireShield = Assert.Single(options.Where(option => option.DisplayName == "Fire Shield"));

        Assert.True(fireShield.IsEnabled);
        Assert.True(fireShield.IsUnlocked);
    }

    [Fact]
    public void CreateOptionViewModels_ForRangedSlot_EnablesMissileWhenUnlocked()
    {
        var player = new PlayerActor(
            new StubInputService(),
            new PlayerCombatSettings(),
            new PlayerMovementController(new PlayerMovementSettings()),
            new PlayerDashController(new PlayerMovementSettings()),
            new PlayerAbilityService([PlayerAbility.Dash, PlayerAbility.Fireball, PlayerAbility.Missile]),
            new PlayerAttackController(new PlayerAttackSettings()));

        var options = AbilityLoadoutCatalog.CreateOptionViewModels(player, AbilityLoadoutSlot.Ranged);
        var missile = Assert.Single(options.Where(option => option.DisplayName == "Missile"));

        Assert.True(missile.IsEnabled);
        Assert.True(missile.IsUnlocked);
    }

    private sealed class StubInputService : MyGame.Core.Input.IInputService
    {
        public MyGame.Core.Input.InputSnapshot Current => MyGame.Core.Input.InputSnapshot.Empty;

        public MyGame.Core.Input.InputSnapshot Previous => MyGame.Core.Input.InputSnapshot.Empty;

        public void Update()
        {
        }

        public bool IsPressed(MyGame.Core.Input.GameAction action)
        {
            return false;
        }

        public bool IsJustPressed(MyGame.Core.Input.GameAction action)
        {
            return false;
        }

        public bool IsJustReleased(MyGame.Core.Input.GameAction action)
        {
            return false;
        }
    }
}
