using MyGame.Gameplay.Enemies;

namespace MyGame.Configuration;

public sealed class EnemySettingsCatalog : IEnemySettingsCatalog
{
    private readonly IReadOnlyDictionary<EnemyKind, EnemySettings> _settingsByKind;

    public EnemySettingsCatalog(
        EnemySettings crabSettings,
        EnemySettings hornedRabbitSettings,
        EnemySettings? batSettings = null,
        EnemySettings? grasshopperSettings = null)
    {
        _settingsByKind = new Dictionary<EnemyKind, EnemySettings>
        {
            [EnemyKind.Crab] = Normalize(crabSettings, EnemyKind.Crab),
            [EnemyKind.HornedRabbit] = Normalize(hornedRabbitSettings, EnemyKind.HornedRabbit),
            [EnemyKind.Bat] = Normalize(batSettings ?? CreateDefault(EnemyKind.Bat), EnemyKind.Bat),
            [EnemyKind.Grasshopper] = Normalize(grasshopperSettings ?? CreateDefault(EnemyKind.Grasshopper), EnemyKind.Grasshopper)
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
            // TODO: these settings should be config driven then loaded in from JSON right? Instead of hard coded
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
            EnemyKind.Bat => new EnemySettings
            {
                Kind = EnemyKind.Bat,
                MaxHealth = 2,
                MoveSpeed = 144f,
                ChaseRange = 280f,
                RecoverySeconds = 0.5f,
                DefeatedVisibleSeconds = 0.8f,
                PlayerHitKnockbackDistance = 18f,
                PlayerHitKnockbackSeconds = 0.1f,
                PlayerHitPauseSeconds = 0.05f,
                DashSpeed = 256f,
                DashSeconds = 0.85f,
                DashPauseSeconds = 1.0f,
                InitialDashPauseMinSeconds = 0.0f,
                InitialDashPauseMaxSeconds = 0.8f,
                AttackHitboxPadding = 8
            },
            EnemyKind.Grasshopper => new EnemySettings
            {
                Kind = EnemyKind.Grasshopper,
                MaxHealth = 2,
                MoveSpeed = 128f,
                ChaseRange = 280f,
                RecoverySeconds = 0.5f,
                DefeatedVisibleSeconds = 0.8f,
                PlayerHitKnockbackDistance = 18f,
                PlayerHitKnockbackSeconds = 0.1f,
                PlayerHitPauseSeconds = 0.05f,
                DashSpeed = 304f,
                DashSeconds = 0.18f,
                DashPauseSeconds = 1.0f,
                InitialDashPauseMinSeconds = 0.0f,
                InitialDashPauseMaxSeconds = 0.8f,
                AttackHitboxPadding = 4
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
            InitialDashPauseMaxSeconds = settings.InitialDashPauseMaxSeconds,
            AttackHitboxPadding = settings.AttackHitboxPadding
        };
    }
}
