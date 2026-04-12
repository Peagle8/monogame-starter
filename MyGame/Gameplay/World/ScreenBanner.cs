using MyGame.Core;

namespace MyGame.Gameplay.World;

public sealed class ScreenBanner
{
    private float _remainingSeconds;

    public ScreenBanner(string text, float durationSeconds)
    {
        Text = text;
        _remainingSeconds = durationSeconds;
    }

    public string Text { get; }

    public bool IsActive => _remainingSeconds > 0f;

    public float Alpha => IsActive ? 1f : 0f;

    public void Update(FrameTime frameTime)
    {
        if (!IsActive)
        {
            return;
        }

        _remainingSeconds = Math.Max(0f, _remainingSeconds - frameTime.DeltaSeconds);
    }
}
