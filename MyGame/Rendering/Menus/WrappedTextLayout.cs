using Microsoft.Xna.Framework.Graphics;

namespace MyGame.Rendering.Menus;

public static class WrappedTextLayout
{
    public static IReadOnlyList<string> WrapText(SpriteFont font, string text, float maxWidth)
    {
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (words.Length == 0)
        {
            return [string.Empty];
        }

        var lines = new List<string>();
        var currentLine = words[0];

        for (var index = 1; index < words.Length; index++)
        {
            var candidate = $"{currentLine} {words[index]}";
            if (font.MeasureString(candidate).X <= maxWidth)
            {
                currentLine = candidate;
                continue;
            }

            lines.Add(currentLine);
            currentLine = words[index];
        }

        lines.Add(currentLine);
        return lines;
    }
}
