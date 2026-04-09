namespace MyGame.Gameplay.Shops;

public sealed record ShopDialogueState(bool IsPromptVisible, bool IsOpen, ShopDialogueTab ActiveTab)
{
    public static readonly ShopDialogueState Default = new(false, false, ShopDialogueTab.Buy);
}
