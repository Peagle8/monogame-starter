using MyGame.Gameplay.Player;

namespace MyGame.Gameplay.Inventory;

public static class AbilityLoadoutCatalog
{
    private static readonly AbilityLoadoutSlot[] Slots =
    [
        AbilityLoadoutSlot.Dash,
        AbilityLoadoutSlot.Defense,
        AbilityLoadoutSlot.Ranged,
        AbilityLoadoutSlot.Melee
    ];

    private static readonly AbilityMenuAction[] Actions =
    [
        AbilityMenuAction.Equip,
        AbilityMenuAction.ViewUpgrades
    ];

    public static IReadOnlyList<AbilityLoadoutSlot> OrderedSlots => Slots;

    public static IReadOnlyList<AbilityMenuAction> MenuActions => Actions;

    public static string GetTabLabel()
    {
        return "Abilities / Loadout";
    }

    public static string GetSlotLabel(AbilityLoadoutSlot slot)
    {
        return slot switch
        {
            AbilityLoadoutSlot.Dash => "Dash",
            AbilityLoadoutSlot.Defense => "Defense",
            AbilityLoadoutSlot.Ranged => "Ranged",
            _ => "Melee"
        };
    }

    public static string GetActionLabel(AbilityMenuAction action)
    {
        return action switch
        {
            AbilityMenuAction.Equip => "Equip",
            _ => "View Upgrades"
        };
    }

    public static IReadOnlyList<AbilityCatalogEntry> GetEntries(AbilityLoadoutSlot slot)
    {
        return slot switch
        {
            AbilityLoadoutSlot.Dash => GetDashEntries(),
            AbilityLoadoutSlot.Defense => GetDefenseEntries(),
            AbilityLoadoutSlot.Ranged => GetRangedEntries(),
            _ => GetMeleeEntries()
        };
    }

    public static IReadOnlyList<AbilitySummaryEntry> CreateSummary(PlayerActor player)
    {
        return OrderedSlots
            .Select(slot => new AbilitySummaryEntry(slot, GetSlotLabel(slot), GetEquippedAbilityName(player, slot)))
            .ToArray();
    }

    public static IReadOnlyList<AbilityMenuOptionViewModel> CreateOptionViewModels(PlayerActor player, AbilityLoadoutSlot slot)
    {
        return GetEntries(slot)
            .Select(entry => new AbilityMenuOptionViewModel(
                entry.DisplayName,
                CanEquip(player, entry),
                IsEquipped(player, entry),
                IsUnlocked(player, entry)))
            .ToArray();
    }

    public static void Equip(PlayerActor player, AbilityCatalogEntry entry)
    {
        switch (entry.Kind)
        {
            case PlayerDashAbilityKind dashAbility:
                player.EquipDashAbility(dashAbility);
                return;
            case PlayerDefenseAbilityKind defenseAbility:
                player.EquipDefenseAbility(defenseAbility);
                return;
            case PlayerRangedAttackKind rangedAbility:
                player.EquipRangedAttack(rangedAbility);
                return;
            case PlayerMeleeAbilityKind meleeAbility:
                player.EquipMeleeAbility(meleeAbility);
                return;
            // TODO: need to handle Kind that is not defined in the cases, so a new Kind not yet accounted for here
            // TODO: OR assume melee as the default case like GetEquippedAbilityName
            // TODO: I would like us to use switch expressions like in GetEquippedAbilityName and not this switch/case syntax. More than anything I want to be consistent unless there is a good reason not to be and that should be commented. Let's add this as a coding standard
        }
    }

    public static string GetEquippedAbilityName(PlayerActor player, AbilityLoadoutSlot slot)
    {
        return slot switch
        {
            AbilityLoadoutSlot.Dash => GetDashName(player.EquippedDashAbility),
            AbilityLoadoutSlot.Defense => GetDefenseName(player.EquippedDefenseAbility),
            AbilityLoadoutSlot.Ranged => GetRangedName(player.EquippedRangedAttack),
            _ => GetMeleeName(player.EquippedMeleeAbility)
        };
    }

    private static bool CanEquip(PlayerActor player, AbilityCatalogEntry entry)
    {
        if (!entry.IsImplemented)
        {
            return false;
        }

        return !entry.RequiresUnlock || IsUnlocked(player, entry);
    }

    private static bool IsUnlocked(PlayerActor player, AbilityCatalogEntry entry)
    {
        if (!entry.RequiresUnlock)
        {
            return true;
        }

        return entry.Kind switch
        {
            PlayerDashAbilityKind.BaseDash => player.HasAbility(PlayerAbility.Dash),
            PlayerDashAbilityKind.BombDash => player.HasAbility(PlayerAbility.BombDash),
            PlayerRangedAttackKind.Fireball => player.HasAbility(PlayerAbility.Fireball),
            PlayerDefenseAbilityKind.Shield => true, // TODO: we should check this as well
            _ => false
        };
    }

    private static bool IsEquipped(PlayerActor player, AbilityCatalogEntry entry)
    {
        return entry.Kind switch
        {
            PlayerDashAbilityKind dashAbility => player.EquippedDashAbility == dashAbility,
            PlayerDefenseAbilityKind defenseAbility => player.EquippedDefenseAbility == defenseAbility,
            PlayerRangedAttackKind rangedAbility => player.EquippedRangedAttack == rangedAbility,
            PlayerMeleeAbilityKind meleeAbility => player.EquippedMeleeAbility == meleeAbility,
            _ => false
        };
    }

    private static IReadOnlyList<AbilityCatalogEntry> GetDashEntries()
    {
        return
        [
            new AbilityCatalogEntry(AbilityLoadoutSlot.Dash, PlayerDashAbilityKind.BaseDash, "Base Dash", true, true),
            new AbilityCatalogEntry(AbilityLoadoutSlot.Dash, PlayerDashAbilityKind.BombDash, "Bomb Dash", true, true),
            new AbilityCatalogEntry(AbilityLoadoutSlot.Dash, PlayerDashAbilityKind.LightningDash, "Lightning Dash", false, true),
            new AbilityCatalogEntry(AbilityLoadoutSlot.Dash, PlayerDashAbilityKind.Superspeed, "Superspeed", false, true)
        ];
    }

    private static IReadOnlyList<AbilityCatalogEntry> GetDefenseEntries()
    {
        return
        [
            new AbilityCatalogEntry(AbilityLoadoutSlot.Defense, PlayerDefenseAbilityKind.Shield, "Base Shield", true, true),
            new AbilityCatalogEntry(AbilityLoadoutSlot.Defense, PlayerDefenseAbilityKind.FireShield, "Fire Shield", true, false),
            new AbilityCatalogEntry(AbilityLoadoutSlot.Defense, PlayerDefenseAbilityKind.IceShield, "Ice Shield", false, true),
            new AbilityCatalogEntry(AbilityLoadoutSlot.Defense, PlayerDefenseAbilityKind.ElectricityShield, "Electricity Shield", false, true),
            new AbilityCatalogEntry(AbilityLoadoutSlot.Defense, PlayerDefenseAbilityKind.StealthShield, "Stealth Shield", false, true),
            new AbilityCatalogEntry(AbilityLoadoutSlot.Defense, PlayerDefenseAbilityKind.WindShield, "Wind Shield", false, true),
            new AbilityCatalogEntry(AbilityLoadoutSlot.Defense, PlayerDefenseAbilityKind.GodShield, "God Shield", false, true)
        ];
    }

    private static IReadOnlyList<AbilityCatalogEntry> GetRangedEntries()
    {
        return
        [
            new AbilityCatalogEntry(AbilityLoadoutSlot.Ranged, PlayerRangedAttackKind.Fireball, "Fireball", true, true),
            new AbilityCatalogEntry(AbilityLoadoutSlot.Ranged, PlayerRangedAttackKind.WindCutter, "Wind Cutter", false, true),
            new AbilityCatalogEntry(AbilityLoadoutSlot.Ranged, PlayerRangedAttackKind.Missile, "Missile", false, true),
            new AbilityCatalogEntry(AbilityLoadoutSlot.Ranged, PlayerRangedAttackKind.Bow, "Bow", false, true),
            new AbilityCatalogEntry(AbilityLoadoutSlot.Ranged, PlayerRangedAttackKind.CompactBow, "Compact Bow", false, true),
            new AbilityCatalogEntry(AbilityLoadoutSlot.Ranged, PlayerRangedAttackKind.LegendaryBow, "Legendary Bow", false, true)
        ];
    }

    private static IReadOnlyList<AbilityCatalogEntry> GetMeleeEntries()
    {
        return
        [
            new AbilityCatalogEntry(AbilityLoadoutSlot.Melee, PlayerMeleeAbilityKind.BaseAttack, "Base Attack", true, false),
            new AbilityCatalogEntry(AbilityLoadoutSlot.Melee, PlayerMeleeAbilityKind.ChargedAttack, "Charged Attack", false, false),
            new AbilityCatalogEntry(AbilityLoadoutSlot.Melee, PlayerMeleeAbilityKind.FireSword, "Fire Sword", false, false),
            new AbilityCatalogEntry(AbilityLoadoutSlot.Melee, PlayerMeleeAbilityKind.IceSword, "Ice Sword", false, false),
            new AbilityCatalogEntry(AbilityLoadoutSlot.Melee, PlayerMeleeAbilityKind.LightningSword, "Lightning Sword", false, false),
            new AbilityCatalogEntry(AbilityLoadoutSlot.Melee, PlayerMeleeAbilityKind.SwordGod, "Sword God", false, false)
        ];
    }

    private static string GetDashName(PlayerDashAbilityKind kind)
    {
        return kind switch
        {
            PlayerDashAbilityKind.BaseDash => "Base Dash",
            PlayerDashAbilityKind.BombDash => "Bomb Dash",
            PlayerDashAbilityKind.LightningDash => "Lightning Dash",
            _ => "Superspeed"
        };
    }

    private static string GetDefenseName(PlayerDefenseAbilityKind kind)
    {
        return kind switch
        {
            PlayerDefenseAbilityKind.Shield => "Base Shield",
            PlayerDefenseAbilityKind.FireShield => "Fire Shield",
            PlayerDefenseAbilityKind.IceShield => "Ice Shield",
            PlayerDefenseAbilityKind.ElectricityShield => "Electricity Shield",
            PlayerDefenseAbilityKind.StealthShield => "Stealth Shield",
            PlayerDefenseAbilityKind.WindShield => "Wind Shield",
            _ => "God Shield"
        };
    }

    private static string GetRangedName(PlayerRangedAttackKind kind)
    {
        return kind switch
        {
            PlayerRangedAttackKind.Fireball => "Fireball",
            PlayerRangedAttackKind.WindCutter => "Wind Cutter",
            PlayerRangedAttackKind.Missile => "Missile",
            PlayerRangedAttackKind.Bow => "Bow",
            PlayerRangedAttackKind.CompactBow => "Compact Bow",
            _ => "Legendary Bow"
        };
    }

    private static string GetMeleeName(PlayerMeleeAbilityKind kind)
    {
        return kind switch
        {
            PlayerMeleeAbilityKind.BaseAttack => "Base Attack",
            PlayerMeleeAbilityKind.ChargedAttack => "Charged Attack",
            PlayerMeleeAbilityKind.FireSword => "Fire Sword",
            PlayerMeleeAbilityKind.IceSword => "Ice Sword",
            PlayerMeleeAbilityKind.LightningSword => "Lightning Sword",
            _ => "Sword God"
        };
    }
}
