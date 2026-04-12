using MyGame.Configuration;

namespace MyGame.Gameplay.Player;

public sealed class PlayerDefenseAbilityController
{
    private readonly PlayerDefenseAbilitySettings _settings;

    public PlayerDefenseAbilityController(PlayerDefenseAbilitySettings settings)
    {
        _settings = settings;
    }

    public float ShieldActivationCost => _settings.ShieldActivationCost;

    public PlayerDefenseAbilityUpdateResult Update(
        PlayerDefenseAbilityState currentState,
        bool defenseAbilityJustPressed,
        bool canActivateEquippedAbility)
    {
        if (!defenseAbilityJustPressed)
        {
            return new PlayerDefenseAbilityUpdateResult(currentState, Activated: false);
        }

        if (currentState.IsActive)
        {
            return new PlayerDefenseAbilityUpdateResult(currentState, Activated: false);
        }

        if (!canActivateEquippedAbility)
        {
            return new PlayerDefenseAbilityUpdateResult(currentState, Activated: false);
        }

        return new PlayerDefenseAbilityUpdateResult(
            currentState with
            {
                IsActive = true,
                RemainingCharges = _settings.ShieldMaxCharges
            },
            Activated: true);
    }

    public PlayerDefenseAbilityState ConsumeShieldCharge(PlayerDefenseAbilityState currentState)
    {
        if (!currentState.IsActive || currentState.RemainingCharges <= 0)
        {
            return currentState;
        }

        var remainingCharges = currentState.RemainingCharges - 1;
        return currentState with
        {
            IsActive = remainingCharges > 0,
            RemainingCharges = Math.Max(0, remainingCharges)
        };
    }
}
