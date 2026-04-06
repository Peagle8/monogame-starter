namespace MyGame.Gameplay.Player;

public sealed class PlayerAbilityService : IPlayerAbilityService
{
    private readonly HashSet<PlayerAbility> _unlockedAbilities;

    public PlayerAbilityService(IEnumerable<PlayerAbility>? unlockedAbilities = null)
    {
        _unlockedAbilities = unlockedAbilities?.ToHashSet() ?? [];
    }

    public bool HasAbility(PlayerAbility ability)
    {
        return _unlockedAbilities.Contains(ability);
    }

    public void Unlock(PlayerAbility ability)
    {
        _unlockedAbilities.Add(ability);
    }

    public void Lock(PlayerAbility ability)
    {
        _unlockedAbilities.Remove(ability);
    }
}
