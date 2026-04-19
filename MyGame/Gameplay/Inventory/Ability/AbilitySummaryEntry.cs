namespace MyGame.Gameplay.Inventory;

public sealed record AbilitySummaryEntry(
    AbilityLoadoutSlot Slot,
    string SlotLabel,
    string EquippedAbilityName);
