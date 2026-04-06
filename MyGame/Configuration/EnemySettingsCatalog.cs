using MyGame.Gameplay.Enemies;

namespace MyGame.Configuration;

public sealed class EnemySettingsCatalog : IEnemySettingsCatalog
{
    private readonly IReadOnlyDictionary<EnemyKind, EnemySettings> _settingsByKind;

    public EnemySettingsCatalog(EnemySettings crabSettings, EnemySettings hornedRabbitSettings)
    {
        _settingsByKind = new Dictionary<EnemyKind, EnemySettings>
        {
            [EnemyKind.Crab] = Normalize(crabSettings, EnemyKind.Crab),
            [EnemyKind.HornedRabbit] = Normalize(hornedRabbitSettings, EnemyKind.HornedRabbit)
        };
    }

    public EnemySettings Get(EnemyKind kind)
    {
        return _settingsByKind.TryGetValue(kind, out var settings)
            ? settings
            : throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unsupported enemy kind.");
    }

    public static EnemySettings CreateDefault(EnemyKind kind)
    {
        return kind switch
        {
            EnemyKind.Crab => new EnemySettings
            {
                Kind = EnemyKind.Crab
            },
            EnemyKind.HornedRabbit => new EnemySettings
            {
                Kind = EnemyKind.HornedRabbit,
                MaxHealth = 2,
                MoveSpeed = 210f,
                ChaseRange = 260f,
                RecoverySeconds = 0.5f,
                DefeatedVisibleSeconds = 0.8f,
                PlayerHitKnockbackDistance = 20f,
                PlayerHitKnockbackSeconds = 0.1f,
                PlayerHitPauseSeconds = 0.05f,
                DashSpeed = 504f,
                DashSeconds = 0.252f,
                DashPauseSeconds = 1.0f,
                InitialDashPauseMinSeconds = 0.0f,
                InitialDashPauseMaxSeconds = 0.8f
            },
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unsupported enemy kind.")
        };
    }

    private static EnemySettings Normalize(EnemySettings settings, EnemyKind kind)
    {
        return new EnemySettings
        {
            Kind = kind,
            MaxHealth = settings.MaxHealth,
            MoveSpeed = settings.MoveSpeed,
            ChaseRange = settings.ChaseRange,
            RecoverySeconds = settings.RecoverySeconds,
            DefeatedVisibleSeconds = settings.DefeatedVisibleSeconds,
            PlayerHitKnockbackDistance = settings.PlayerHitKnockbackDistance,
            PlayerHitKnockbackSeconds = settings.PlayerHitKnockbackSeconds,
            PlayerHitPauseSeconds = settings.PlayerHitPauseSeconds,
            DashSpeed = settings.DashSpeed,
            DashSeconds = settings.DashSeconds,
            DashPauseSeconds = settings.DashPauseSeconds,
            InitialDashPauseMinSeconds = settings.InitialDashPauseMinSeconds,
            InitialDashPauseMaxSeconds = settings.InitialDashPauseMaxSeconds
        };
    }
}
