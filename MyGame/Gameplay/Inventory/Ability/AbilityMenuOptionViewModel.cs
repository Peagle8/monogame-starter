namespace MyGame.Gameplay.Inventory;

public sealed record AbilityMenuOptionViewModel(
    string DisplayName,
    bool IsEnabled,
    bool IsEquipped,
    bool IsUnlocked);
