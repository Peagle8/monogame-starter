using Microsoft.Xna.Framework;

namespace MyGame.Gameplay.Player;

public sealed record PlayerBombTrailState(
    int DashSequence,
    float RemainingDropSeconds,
    float DropIntervalSeconds,
    int SpawnedRowCount,
    Vector2 LastRowCenter,
    Vector2 LastRowStep)
{
    public static readonly PlayerBombTrailState Default = new(0, 0f, 0f, 0, Vector2.Zero, Vector2.Zero);
}
