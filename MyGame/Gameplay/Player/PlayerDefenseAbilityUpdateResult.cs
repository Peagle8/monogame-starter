namespace MyGame.Gameplay.Player;

public sealed record PlayerDefenseAbilityUpdateResult(
    PlayerDefenseAbilityState State,
    bool Activated);
