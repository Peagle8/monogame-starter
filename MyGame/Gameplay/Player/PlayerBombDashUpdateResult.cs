namespace MyGame.Gameplay.Player;

public sealed record PlayerBombDashUpdateResult(
    PlayerBombTrailState State,
    IReadOnlyList<PlayerBomb> SpawnedBombs);
