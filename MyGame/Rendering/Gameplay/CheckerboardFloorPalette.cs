using Microsoft.Xna.Framework;
using MyGame.Scenes.Gameplay;

namespace MyGame.Rendering.Gameplay;

public static class CheckerboardFloorPalette
{
    private static readonly Color DefaultLightTileColor = new(34, 52, 59);
    private static readonly Color DefaultDarkTileColor = new(26, 41, 47);
    private static readonly Color ArenaLightTileColor = new(160, 135, 98);
    private static readonly Color ArenaDarkTileColor = new(141, 116, 81);

    public static Color GetTileColor(int column, int row, string? sceneName = null)
    {
        var (lightTileColor, darkTileColor) = GetPalette(sceneName);

        return (column + row) % 2 == 0
            ? lightTileColor
            : darkTileColor;
    }

    private static (Color Light, Color Dark) GetPalette(string? sceneName)
    {
        return sceneName == GameplaySceneNames.Arena
            ? (ArenaLightTileColor, ArenaDarkTileColor)
            : (DefaultLightTileColor, DefaultDarkTileColor);
    }
}
