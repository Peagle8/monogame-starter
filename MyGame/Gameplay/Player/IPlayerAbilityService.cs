namespace MyGame.Gameplay.Player;

public interface IPlayerAbilityService
{
    IReadOnlyCollection<PlayerAbility> UnlockedAbilities { get; }

    bool HasAbility(PlayerAbility ability);

    void Unlock(PlayerAbility ability);

    void Lock(PlayerAbility ability);

    void SetUnlockedAbilities(IEnumerable<PlayerAbility> abilities);
}
