using MyGame.Configuration;
using MyGame.Core;

namespace MyGame.Gameplay.Player;

public sealed class PlayerDefenseAbilityController
{
    private readonly PlayerDefenseAbilitySettings _settings;

    public PlayerDefenseAbilityController(PlayerDefenseAbilitySettings settings)
    {
        _settings = settings;
    }

    public float ShieldActivationCost => _settings.ShieldActivationCost;

    public float FireShieldActivationCost => _settings.FireShieldActivationCost;

    public float FireShieldRadiusMultiplier => _settings.FireShieldRadiusMultiplier;

    public int FireShieldDamage => _settings.FireShieldDamage;

    public float FireShieldDamageTickSeconds => _settings.FireShieldDamageTickSeconds;

    public int FireShieldRingThickness => _settings.FireShieldRingThickness;

    public float FireShieldPulseAmplitude => _settings.FireShieldPulseAmplitude;

    public PlayerDefenseAbilityUpdateResult Update(
        PlayerDefenseAbilityState currentState,
        bool defenseAbilityJustPressed,
        bool canActivateEquippedAbility,
        FrameTime frameTime)
    {
        var nextState = UpdateActiveState(currentState, frameTime.DeltaSeconds);

        if (!defenseAbilityJustPressed)
        {
            return new PlayerDefenseAbilityUpdateResult(nextState, Activated: false);
        }

        if (nextState.IsActive)
        {
            return new PlayerDefenseAbilityUpdateResult(nextState, Activated: false);
        }

        if (!canActivateEquippedAbility)
        {
            return new PlayerDefenseAbilityUpdateResult(nextState, Activated: false);
        }

        return Activate(nextState);
    }

    public PlayerDefenseAbilityState ConsumeShieldCharge(PlayerDefenseAbilityState currentState)
    {
        if (!currentState.IsActive
            || currentState.RemainingCharges <= 0)
        {
            return currentState;
        }

        var remainingCharges = currentState.RemainingCharges - 1;
        return currentState with
        {
            IsActive = remainingCharges > 0,
            RemainingCharges = Math.Max(0, remainingCharges),
            RemainingActiveSeconds = 0f
        };
    }

    public float GetActivationCost(PlayerDefenseAbilityKind ability)
    {
        return ability switch
        {
            PlayerDefenseAbilityKind.Shield => _settings.ShieldActivationCost,
            PlayerDefenseAbilityKind.FireShield => _settings.FireShieldActivationCost,
            _ => float.MaxValue
        };
    }

    private PlayerDefenseAbilityUpdateResult Activate(PlayerDefenseAbilityState currentState)
    {
        return currentState.EquippedAbility switch
        {
            PlayerDefenseAbilityKind.Shield => new PlayerDefenseAbilityUpdateResult(
                currentState with
                {
                    IsActive = true,
                    RemainingCharges = _settings.ShieldMaxCharges,
                    RemainingActiveSeconds = 0f
                },
                Activated: true),
            PlayerDefenseAbilityKind.FireShield => new PlayerDefenseAbilityUpdateResult(
                currentState with
                {
                    IsActive = true,
                    RemainingCharges = _settings.ShieldMaxCharges,
                    RemainingActiveSeconds = 0f
                },
                Activated: true),
            _ => new PlayerDefenseAbilityUpdateResult(currentState, Activated: false)
        };
    }

    private static PlayerDefenseAbilityState UpdateActiveState(PlayerDefenseAbilityState currentState, float deltaSeconds)
    {
        if (!currentState.IsActive
            || currentState.EquippedAbility != PlayerDefenseAbilityKind.FireShield
            || currentState.RemainingActiveSeconds <= 0f)
        {
            return currentState;
        }

        var remainingActiveSeconds = Math.Max(0f, currentState.RemainingActiveSeconds - deltaSeconds);
        return currentState with
        {
            IsActive = remainingActiveSeconds > 0f,
            RemainingActiveSeconds = remainingActiveSeconds
        };
    }
}
