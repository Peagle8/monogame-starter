namespace MyGame.Gameplay.Player;

public sealed class PlayerShieldBreakEffect
{
    private const float DurationSeconds = 0.45f;
    private float _remainingSeconds;

    public bool IsActive => _remainingSeconds > 0f;

    public PlayerDefenseAbilityKind Kind { get; private set; } = PlayerDefenseAbilityKind.Shield;

    public float Alpha => Math.Clamp(_remainingSeconds / DurationSeconds, 0f, 1f);

    public float Progress => 1f - Alpha;

    public void Begin(PlayerDefenseAbilityKind kind)
    {
        Kind = kind;
        _remainingSeconds = DurationSeconds;
    }

    public void Update(float deltaSeconds)
    {
        if (_remainingSeconds <= 0f || deltaSeconds <= 0f)
        {
            return;
        }

        _remainingSeconds = Math.Max(0f, _remainingSeconds - deltaSeconds);
    }

    public void Reset()
    {
        _remainingSeconds = 0f;
    }
}
