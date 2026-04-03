using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyGame.Core.Diagnostics;

public sealed class DebugOverlay
{
    private readonly Dictionary<string, string> _values = new();
    private Vector2 _position = new(10f, 10f);
    private SpriteBatch? _spriteBatch;
    private SpriteFont? _font;

    public void SetSpriteBatch(SpriteBatch spriteBatch)
    {
        _spriteBatch = spriteBatch;
    }

    public void SetFont(SpriteFont? font)
    {
        _font = font;
    }

    public void SetValue(string key, string value)
    {
        _values[key] = value;
    }

    public void SetPosition(Vector2 position)
    {
        _position = position;
    }

    public void Draw()
    {
        if (_spriteBatch is null || _font is null || _values.Count == 0)
        {
            return;
        }

        var lines = _values.Select(static pair => $"{pair.Key}: {pair.Value}").ToArray();
        var position = _position;

        _spriteBatch.Begin();

        foreach (var line in lines)
        {
            _spriteBatch.DrawString(_font, line, position, Color.LimeGreen);
            position.Y += 20f;
        }

        _spriteBatch.End();
    }
}
