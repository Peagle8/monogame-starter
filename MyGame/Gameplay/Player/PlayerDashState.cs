namespace MyGame.Gameplay.Player;

public sealed record PlayerDashState(
    bool IsDashing,
    Direction Direction,
    int DashSequence,
    float RemainingActiveSeconds,
    float RemainingCooldownSeconds)
{
    public static readonly PlayerDashState Idle = new(
        false,
        Direction.Down,
        0,
        0f,
        0f);
}
