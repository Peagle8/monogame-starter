namespace MyGame.Gameplay.Player;

public interface IPlayerAbilityService
{
    bool HasAbility(PlayerAbility ability);

    void Unlock(PlayerAbility ability);

    void Lock(PlayerAbility ability);
}
