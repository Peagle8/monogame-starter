using Microsoft.Xna.Framework;

namespace MyGame.Rendering.Menus;

public static class VerticalMenuLayout
{
    public static Vector2 GetItemPosition(Vector2 startPosition, float itemSpacing, int index)
    {
        return new Vector2(startPosition.X, startPosition.Y + (index * itemSpacing));
    }

    public static Color GetItemColor(int index, int selectedIndex)
    {
        return index == selectedIndex ? Color.Yellow : Color.Gray;
    }

    public static Color GetItemColor(int index, int selectedIndex, bool isEnabled)
    {
        if (!isEnabled)
        {
            return new Color(110, 110, 110);
        }

        return GetItemColor(index, selectedIndex);
    }
}
