using Microsoft.Xna.Framework;

namespace MyGame.Rendering.MainMenu;

public static class MainMenuBackgroundPalette
{
    private static readonly Color[] StripeColors =
    [
        new(15, 26, 47),
        new(22, 40, 71),
        new(34, 58, 99)
    ];

    public static Color GetStripeColor(int stripeIndex)
    {
        return StripeColors[stripeIndex % StripeColors.Length];
    }
}
