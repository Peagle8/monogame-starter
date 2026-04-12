using MyGame.Gameplay.Enemies;

namespace MyGame.Configuration;

public sealed class EnemySettingsCatalog : IEnemySettingsCatalog
{
    private readonly IReadOnlyDictionary<EnemyKind, EnemySettings> _settingsByKind;

    public EnemySettingsCatalog(
        EnemySettings crabSettings,
        EnemySettings hornedRabbitSettings,
        EnemySettings? hornedRabbitEliteSettings = null,
        EnemySettings? batSettings = null,
        EnemySettings? grasshopperSettings = null,
        EnemySettings? batMiniBossSettings = null,
        EnemySettings? hornedRabbitBossSettings = null)
    {
        _settingsByKind = new Dictionary<EnemyKind, EnemySettings>
        {
            [EnemyKind.Crab] = Normalize(crabSettings, EnemyKind.Crab),
            [EnemyKind.HornedRabbit] = Normalize(hornedRabbitSettings, EnemyKind.HornedRabbit),
            [EnemyKind.HornedRabbitBoss] = Normalize(hornedRabbitBossSettings ?? CreateDefault(EnemyKind.HornedRabbitBoss), EnemyKind.HornedRabbitBoss),
            [EnemyKind.HornedRabbitElite] = Normalize(hornedRabbitEliteSettings ?? CreateDefault(EnemyKind.HornedRabbitElite), EnemyKind.HornedRabbitElite),
            [EnemyKind.Bat] = Normalize(batSettings ?? CreateDefault(EnemyKind.Bat), EnemyKind.Bat),
            [EnemyKind.BatMiniBoss] = Normalize(batMiniBossSettings ?? CreateDefault(EnemyKind.BatMiniBoss), EnemyKind.BatMiniBoss),
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
            EnemyKind.HornedRabbitBoss => new EnemySettings
            {
                Kind = EnemyKind.HornedRabbitBoss,
                MaxHealth = 6,
                MoveSpeed = 0f,
                ChaseRange = 1200f,
                RecoverySeconds = 0.6f,
                DefeatedVisibleSeconds = 1.0f,
                PlayerHitKnockbackDistance = 22f,
                PlayerHitKnockbackSeconds = 0.12f,
                PlayerHitPauseSeconds = 0.06f,
                DashSpeed = 640f,
                DashSeconds = 0.35f,
                DashPauseSeconds = 1.1f,
                AttackHitboxPadding = 10,
                BoundsWidth = 48,
                BoundsHeight = 48
            },
            EnemyKind.HornedRabbitElite => new EnemySettings
            {
                Kind = EnemyKind.HornedRabbitElite,
                MaxHealth = 4,
                MoveSpeed = 216f,
                ChaseRange = 1200f,
                RecoverySeconds = 0.55f,
                DefeatedVisibleSeconds = 0.9f,
                PlayerHitKnockbackDistance = 22f,
                PlayerHitKnockbackSeconds = 0.12f,
                PlayerHitPauseSeconds = 0.055f,
                DashSpeed = 520f,
                DashSeconds = 0.3f,
                DashPauseSeconds = 0.9f,
                InitialDashPauseMinSeconds = 0.0f,
                InitialDashPauseMaxSeconds = 0.8f,
                AttackHitboxPadding = 6,
                BoundsWidth = 32,
                BoundsHeight = 32
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
            EnemyKind.BatMiniBoss => new EnemySettings
            {
                Kind = EnemyKind.BatMiniBoss,
                MaxHealth = 6,
                MoveSpeed = 132f,
                ChaseRange = 360f,
                RecoverySeconds = 0.6f,
                DefeatedVisibleSeconds = 1.0f,
                PlayerHitKnockbackDistance = 20f,
                PlayerHitKnockbackSeconds = 0.12f,
                PlayerHitPauseSeconds = 0.06f,
                DashSpeed = 256f,
                DashSeconds = 0.85f,
                DashPauseSeconds = 1.0f,
                AttackHitboxPadding = 12,
                BoundsWidth = 56,
                BoundsHeight = 56,
                SpecialAttackDamage = 2,
                SpecialAttackRange = 154f,
                SpecialAttackPauseSeconds = 1.0f,
                SpecialAttackStunSeconds = 2.0f,
                SpecialAttackConeHalfAngleDegrees = 35f
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
            AttackHitboxPadding = settings.AttackHitboxPadding,
            BoundsWidth = settings.BoundsWidth,
            BoundsHeight = settings.BoundsHeight,
            SpecialAttackDamage = settings.SpecialAttackDamage,
            SpecialAttackRange = settings.SpecialAttackRange,
            SpecialAttackPauseSeconds = settings.SpecialAttackPauseSeconds,
            SpecialAttackStunSeconds = settings.SpecialAttackStunSeconds,
            SpecialAttackConeHalfAngleDegrees = settings.SpecialAttackConeHalfAngleDegrees
        };
    }
}
