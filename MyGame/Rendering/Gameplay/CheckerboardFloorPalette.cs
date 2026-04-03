using Microsoft.Xna.Framework;

namespace MyGame.Rendering.Gameplay;

public static class CheckerboardFloorPalette
{
    private static readonly Color LightTileColor = new(34, 52, 59);
    private static readonly Color DarkTileColor = new(26, 41, 47);

    public static Color GetTileColor(int column, int row)
    {
        return (column + row) % 2 == 0
            ? LightTileColor
            : DarkTileColor;
    }
}
