using Microsoft.Xna.Framework;
using MyGame.Core;

namespace MyGame.Gameplay.World;

public sealed class WorldToast
{
    private readonly Vector2 _velocity;
    private float _remainingSeconds;

    public WorldToast(string text, Vector2 position, Color color, float lifetimeSeconds = 0.75f)
    {
        Text = text;
        Position = position;
        Color = color;
        _remainingSeconds = lifetimeSeconds;
        _velocity = new Vector2(0f, -18f);
    }

    public string Text { get; }

    public Vector2 Position { get; private set; }

    public Color Color { get; }

    public bool IsActive => _remainingSeconds > 0f;

    public float Alpha => MathHelper.Clamp(_remainingSeconds / 0.75f, 0f, 1f);

    public void Update(FrameTime frameTime)
    {
        if (!IsActive)
        {
            return;
        }

        Position += _velocity * frameTime.DeltaSeconds;
        _remainingSeconds = Math.Max(0f, _remainingSeconds - frameTime.DeltaSeconds);
    }
}
