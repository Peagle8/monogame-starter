using Microsoft.Xna.Framework;
using MyGame.Configuration;

namespace MyGame.Gameplay.Enemies;

internal sealed class EnemyAbilityState
{
    private readonly EnemySettings _settings;

    public EnemyAbilityState(EnemySettings settings)
    {
        _settings = settings;
        CurrentAbilityPoints = _settings.MaxAbilityPoints;
    }

    public float CurrentAbilityPoints { get; private set; }

    public float MaxAbilityPoints => _settings.MaxAbilityPoints;

    public int ShieldCharges { get; private set; }

    public bool IsShieldActive => ShieldCharges > 0;

    public void Regenerate(float deltaSeconds)
    {
        if (_settings.AbilityPointRegenPerSecond <= 0f || MaxAbilityPoints <= 0f)
        {
            return;
        }

        CurrentAbilityPoints = MathHelper.Clamp(
            CurrentAbilityPoints + (_settings.AbilityPointRegenPerSecond * deltaSeconds),
            0f,
            MaxAbilityPoints);
    }

    public bool TryActivateShield()
    {
        if (IsShieldActive
            || _settings.ShieldMaxCharges <= 0
            || !TrySpend(_settings.ShieldActivationCost))
        {
            return false;
        }

        ShieldCharges = _settings.ShieldMaxCharges;
        return true;
    }

    public bool TryAbsorbShieldHit()
    {
        if (!IsShieldActive)
        {
            return false;
        }

        ShieldCharges = Math.Max(0, ShieldCharges - 1);
        return true;
    }

    public void Restore(float currentAbilityPoints, int shieldCharges)
    {
        CurrentAbilityPoints = MathHelper.Clamp(currentAbilityPoints, 0f, MaxAbilityPoints);
        ShieldCharges = Math.Clamp(shieldCharges, 0, _settings.ShieldMaxCharges);
    }

    private bool TrySpend(float amount)
    {
        if (amount <= 0f)
        {
            return true;
        }

        if (CurrentAbilityPoints < amount)
        {
            return false;
        }

        CurrentAbilityPoints -= amount;
        return true;
    }
}
