using Microsoft.Xna.Framework;

namespace MyGame.Gameplay.Shops;

public sealed class ShopDialogueController
{
    private const int HorizontalPromptRange = 24;
    private const int VerticalPromptRange = 36;

    public ShopDialogueState Update(
        ShopDialogueState state,
        Rectangle playerBounds,
        Rectangle? counterBounds,
        bool interactJustPressed,
        bool cancelJustPressed,
        bool previousTabJustPressed,
        bool nextTabJustPressed)
    {
        var isNearCounter = counterBounds is Rectangle bounds && GetInteractionBounds(bounds).Intersects(playerBounds);
        if (!isNearCounter)
        {
            return ShopDialogueState.Default;
        }

        var nextState = state with { IsPromptVisible = true };
        if (!nextState.IsOpen)
        {
            return interactJustPressed
                ? nextState with { IsOpen = true }
                : nextState with { ActiveTab = ShopDialogueTab.Buy };
        }

        if (cancelJustPressed)
        {
            return nextState with { IsOpen = false };
        }

        if (previousTabJustPressed)
        {
            return nextState with { ActiveTab = Previous(nextState.ActiveTab) };
        }

        if (nextTabJustPressed)
        {
            return nextState with { ActiveTab = Next(nextState.ActiveTab) };
        }

        return nextState;
    }

    private static Rectangle GetInteractionBounds(Rectangle counterBounds)
    {
        return new Rectangle(
            counterBounds.X - HorizontalPromptRange,
            counterBounds.Y - VerticalPromptRange,
            counterBounds.Width + (HorizontalPromptRange * 2),
            counterBounds.Height + (VerticalPromptRange * 2));
    }

    private static ShopDialogueTab Previous(ShopDialogueTab tab)
    {
        return tab switch
        {
            ShopDialogueTab.Buy => ShopDialogueTab.Sell,
            _ => ShopDialogueTab.Buy
        };
    }

    private static ShopDialogueTab Next(ShopDialogueTab tab)
    {
        return tab switch
        {
            ShopDialogueTab.Sell => ShopDialogueTab.Buy,
            _ => ShopDialogueTab.Sell
        };
    }
}
