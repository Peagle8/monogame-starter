namespace MyGame.Gameplay.Inventory;

public sealed record AbilityCatalogEntry(
    AbilityLoadoutSlot Slot,
    Enum Kind,
    string DisplayName,
    bool IsImplemented,
    bool RequiresUnlock);
