using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Core;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.Props;
using MyGame.Infrastructure.Save;

namespace MyGame.Gameplay.World;

public sealed class World
{
    private readonly List<EnemyActor> _enemies;
    private readonly HashSet<EnemyActor> _countedDefeatedEnemies = [];
    private readonly EnemySettings _enemySettings;
    private readonly IEnemyFactory _enemyFactory;
    private readonly IEnemySettingsCatalog _enemySettingsCatalog;
    private readonly PlayerAttackHitResolver _playerAttackHitResolver;
    private readonly PlayerProjectileResolver _playerProjectileResolver;
    private readonly WorldObstacleResolver _worldObstacleResolver;
    private readonly EnemySeparationResolver _enemySeparationResolver;
    private readonly EnemyContactResolver _enemyContactResolver;
    private readonly IWorldEventController? _eventController;
    private readonly List<IWorldProp> _props;
    private readonly List<PlayerProjectile> _playerProjectiles;
    private readonly List<WorldToast> _toasts;
    private readonly List<WorldSceneTransition> _sceneTransitions;
    private readonly HashSet<WorldSceneTransition> _suppressedSceneTransitions = [];
    private ScreenBanner? _screenBanner;
    private float _remainingPlayerHitPauseSeconds;
    private WorldSceneTransition? _pendingSceneTransition;

    public World(PlayerActor player, EnemySettings enemySettings)
        : this(
            player,
            [
                new TreeProp(new Vector2(120f, 120f), new Point(72, 104)),
                new TreeProp(new Vector2(560f, 160f), new Point(64, 96)),
                new TreeProp(new Vector2(620f, 320f), new Point(80, 112)),
                new GrassProp(new Vector2(188f, 180f), new Point(52, 36)),
                new GrassProp(new Vector2(308f, 132f), new Point(44, 28)),
                new GrassProp(new Vector2(500f, 360f), new Point(56, 34))
            ],
            [new EnemyActor(enemySettings, new Vector2(520f, 240f))],
            enemySettings,
            enemyFactory: new EnemyFactory(new EnemySettingsCatalog(
                enemySettings,
                EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbit))),
            playerAttackHitResolver: new PlayerAttackHitResolver(),
            playerProjectileResolver: new PlayerProjectileResolver(),
            worldObstacleResolver: new WorldObstacleResolver(new WorldCombatSettings()),
            enemySeparationResolver: new EnemySeparationResolver(new WorldCombatSettings()),
            enemyContactResolver: new EnemyContactResolver(new WorldCombatSettings()),
            worldCombatSettings: new WorldCombatSettings(),
            sceneTransitions: [])
    {
    }

    public World(
        PlayerActor player,
        IEnumerable<IWorldProp> props,
        IEnumerable<EnemyActor> enemies,
        EnemySettings? enemySettings = null,
        IEnemySettingsCatalog? enemySettingsCatalog = null,
        IEnemyFactory? enemyFactory = null,
        PlayerAttackHitResolver? playerAttackHitResolver = null,
        PlayerProjectileResolver? playerProjectileResolver = null,
        WorldObstacleResolver? worldObstacleResolver = null,
        EnemySeparationResolver? enemySeparationResolver = null,
        EnemyContactResolver? enemyContactResolver = null,
        WorldCombatSettings? worldCombatSettings = null,
        IEnumerable<WorldSceneTransition>? sceneTransitions = null,
        Rectangle? worldBounds = null,
        IWorldEventController? eventController = null)
    {
        Player = player;
        _enemySettings = enemySettings ?? new EnemySettings();
        _enemySettingsCatalog = enemySettingsCatalog ?? new EnemySettingsCatalog(
            _enemySettings,
            EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbit));
        var resolvedWorldCombatSettings = worldCombatSettings ?? new WorldCombatSettings();
        _enemyFactory = enemyFactory ?? new EnemyFactory(_enemySettingsCatalog);
        _playerAttackHitResolver = playerAttackHitResolver ?? new PlayerAttackHitResolver();
        _playerProjectileResolver = playerProjectileResolver ?? new PlayerProjectileResolver();
        _worldObstacleResolver = worldObstacleResolver ?? new WorldObstacleResolver(resolvedWorldCombatSettings);
        _enemySeparationResolver = enemySeparationResolver ?? new EnemySeparationResolver(resolvedWorldCombatSettings);
        _enemyContactResolver = enemyContactResolver ?? new EnemyContactResolver(resolvedWorldCombatSettings);
        _props = props.ToList();
        _enemies = enemies.ToList();
        _playerProjectiles = [];
        _toasts = [];
        _sceneTransitions = sceneTransitions?.ToList() ?? [];
        WorldBounds = worldBounds;
        _eventController = eventController;
        _eventController?.Initialize(this);
    }

    public PlayerActor Player { get; }

    public IReadOnlyList<EnemyActor> Enemies => _enemies;

    public Rectangle? WorldBounds { get; }

    public bool IsObjectiveComplete => _eventController?.IsComplete ?? true;

    public int DefeatedEnemyCount => _countedDefeatedEnemies.Count;

    public IReadOnlyList<IWorldProp> Props => _props;

    public IReadOnlyList<TreeProp> TreeProps => _props.OfType<TreeProp>().ToArray();

    public IReadOnlyList<GrassProp> GrassProps => _props.OfType<GrassProp>().ToArray();

    public IReadOnlyList<PlayerProjectile> PlayerProjectiles => _playerProjectiles;

    public IReadOnlyList<WorldToast> Toasts => _toasts;

    public ScreenBanner? ActiveScreenBanner => _screenBanner is { IsActive: true } ? _screenBanner : null;

    public IReadOnlyList<TProp> GetProps<TProp>()
        where TProp : class, IWorldProp
    {
        return _props.OfType<TProp>().ToArray();
    }

    public bool HasLivingEnemy(EnemyKind kind)
    {
        return _enemies.Any(enemy => enemy.Kind == kind && enemy.State != EnemyState.Dead);
    }

    public bool HasLivingEnemies()
    {
        return _enemies.Any(enemy => enemy.State != EnemyState.Dead);
    }

    public void SpawnEnemy(EnemyActor enemy)
    {
        _enemies.Add(enemy);
    }

    public void ShowBanner(string text, float durationSeconds)
    {
        if (string.IsNullOrWhiteSpace(text) || durationSeconds <= 0f)
        {
            return;
        }

        _screenBanner = new ScreenBanner(text, durationSeconds);
    }

    public void RestorePlayerToFull()
    {
        Player.RestoreState(Player.Position, Player.MaxHealth, Player.MaxAbilityPoints);
    }

    public void Update(FrameTime frameTime)
    {
        if (_remainingPlayerHitPauseSeconds > 0f)
        {
            _remainingPlayerHitPauseSeconds = Math.Max(0f, _remainingPlayerHitPauseSeconds - frameTime.DeltaSeconds);
            UpdateToasts(frameTime);
            UpdateScreenBanner(frameTime);
            _eventController?.Update(this, frameTime);
            return;
        }

        Player.Update(frameTime);
        _playerProjectiles.AddRange(Player.ConsumeSpawnedProjectiles());
        _worldObstacleResolver.ResolvePlayer(Player, _props);

        foreach (var enemy in _enemies)
        {
            enemy.Update(Player.Position, Player.Bounds, frameTime);
        }

        ApplyBossStageTransitionLockToPlayer();

        SpawnPendingEnemies();

        _worldObstacleResolver.Resolve(_enemies, _props);
        _enemySeparationResolver.Resolve(_enemies);
        ResolveQueuedEnemyAttacks();
        ResolveActiveEnemySpecialAttacks();
        ResolveActiveEnemyBombExplosions();

        UpdateProjectiles(frameTime);
        UpdateToasts(frameTime);
        UpdateScreenBanner(frameTime);
        var projectileHitEnemy = _playerProjectileResolver.Resolve(_playerProjectiles, _enemies, _props);
        _playerProjectiles.RemoveAll(projectile => !projectile.IsActive);

        var playerHitEnemy = _playerAttackHitResolver.Resolve(Player, _enemies);
        TrackDefeatedEnemies(rewardPlayer: true);
        _eventController?.Update(this, frameTime);

        if (playerHitEnemy || projectileHitEnemy)
        {
            _remainingPlayerHitPauseSeconds = _enemySettings.PlayerHitPauseSeconds;
            return;
        }

        _enemyContactResolver.Resolve(Player, _enemies, frameTime);
        _worldObstacleResolver.ResolvePlayer(Player, _props);
        ReleaseSceneTransitionSuppressions();
        QueueSceneTransitionIfTriggered();
    }

    public WorldSceneTransition? ConsumePendingSceneTransition()
    {
        var pendingTransition = _pendingSceneTransition;
        _pendingSceneTransition = null;
        return pendingTransition;
    }

    public void SuppressIntersectingSceneTransitions()
    {
        foreach (var transition in _sceneTransitions)
        {
            if (Player.Bounds.Intersects(transition.TriggerBounds))
            {
                _suppressedSceneTransitions.Add(transition);
            }
        }
    }

    public IReadOnlyDictionary<string, string> GetDebugState()
    {
        return new Dictionary<string, string>
        {
            ["DefeatedEnemyCount"] = DefeatedEnemyCount.ToString(),
            ["EnemyCount"] = _enemies.Count.ToString(),
            ["FirstEnemyState"] = _enemies.FirstOrDefault()?.State.ToString() ?? "<none>",
            ["PlayerAttackActive"] = Player.IsAttacking.ToString(),
            ["PlayerAbilityPoints"] = $"{Player.CurrentAbilityPoints:0.00}/{Player.MaxAbilityPoints:0.00}",
            ["PlayerDead"] = Player.IsDead.ToString(),
            ["PlayerHealth"] = $"{Player.CurrentHealth}/{Player.MaxHealth}",
            ["PlayerPosition"] = $"{Player.Position.X:0.00}, {Player.Position.Y:0.00}",
            ["PlayerFacing"] = Player.Facing.ToString(),
            ["PlayerStunned"] = Player.IsStunned.ToString(),
            ["ObjectiveComplete"] = IsObjectiveComplete.ToString(),
            ["ScreenBanner"] = ActiveScreenBanner?.Text ?? "<none>",
            ["GrassPropCount"] = GrassProps.Count.ToString(),
            ["PropCount"] = _props.Count.ToString(),
            ["ProjectileCount"] = _playerProjectiles.Count.ToString(),
            ["TreePropCount"] = TreeProps.Count.ToString()
        };
    }

    public SaveGameData CreateSaveData(string sceneName)
    {
        return new SaveGameData
        {
            SceneName = sceneName,
            DefeatedEnemyCount = DefeatedEnemyCount,
            Enemies = _enemies.Select(enemy => enemy.CreateSaveData()).ToArray(),
            PlayerAbilityPoints = Player.CurrentAbilityPoints,
            PlayerHealth = Player.CurrentHealth,
            PlayerPositionX = Player.Position.X,
            PlayerPositionY = Player.Position.Y
        };
    }

    public void ApplySaveData(SaveGameData data)
    {
        Player.RestoreState(
            new Vector2(data.PlayerPositionX, data.PlayerPositionY),
            data.PlayerHealth,
            data.PlayerAbilityPoints);
        _remainingPlayerHitPauseSeconds = 0f;
        _playerAttackHitResolver.Reset();
        _enemyContactResolver.Reset();
        _countedDefeatedEnemies.Clear();
        _enemies.Clear();
        _playerProjectiles.Clear();
        _toasts.Clear();
        _screenBanner = null;
        _pendingSceneTransition = null;

        foreach (var enemyData in data.Enemies)
        {
            _enemies.Add(_enemyFactory.CreateFromSaveData(enemyData));
        }

        TrackDefeatedEnemies(rewardPlayer: false);
    }

    private void TrackDefeatedEnemies(bool rewardPlayer)
    {
        foreach (var enemy in _enemies)
        {
            if (enemy.State != EnemyState.Dead || _countedDefeatedEnemies.Contains(enemy))
            {
                continue;
            }

            _countedDefeatedEnemies.Add(enemy);

            if (rewardPlayer)
            {
                var previousAbilityPoints = Player.CurrentAbilityPoints;
                Player.AddAbilityPoints(1f);
                var grantedAbilityPoints = Player.CurrentAbilityPoints - previousAbilityPoints;
                if (grantedAbilityPoints > 0f)
                {
                    _toasts.Add(new WorldToast(
                        $"+{grantedAbilityPoints:0.#} AP",
                        GetPlayerToastAnchor(),
                        new Color(128, 214, 255)));
                }
            }
        }
    }

    private Vector2 GetPlayerToastAnchor()
    {
        return new Vector2(Player.Bounds.Center.X, Player.Bounds.Top - 4f);
    }

    private void UpdateProjectiles(FrameTime frameTime)
    {
        foreach (var projectile in _playerProjectiles)
        {
            projectile.Update(frameTime);
        }
    }

    private void ResolveActiveEnemySpecialAttacks()
    {
        foreach (var enemy in _enemies)
        {
            if (!enemy.TryConsumeActiveSpecialAttack(Player.Bounds, out var attack))
            {
                continue;
            }

            if (Player.IsDead)
            {
                continue;
            }

            if (Player.TryAbsorbShieldHit())
            {
                continue;
            }

            Player.TakeDamage(attack.Damage);
            Player.ApplyStun(attack.StunSeconds);
        }
    }

    private void ResolveQueuedEnemyAttacks()
    {
        foreach (var enemy in _enemies)
        {
            if (!enemy.TryConsumePendingAttack(out var attack))
            {
                continue;
            }

            if (Player.IsDead)
            {
                continue;
            }

            if (Player.TryAbsorbShieldHit())
            {
                continue;
            }

            Player.TakeDamage(attack.Damage);
            Player.ApplyStun(attack.StunSeconds);
        }
    }

    private void ResolveActiveEnemyBombExplosions()
    {
        foreach (var enemy in _enemies)
        {
            if (!enemy.TryConsumeActiveBombExplosion(Player.Bounds, out var attack, out var explosionBounds))
            {
                continue;
            }

            if (Player.IsDead)
            {
                continue;
            }

            if (Player.TryAbsorbShieldHit())
            {
                continue;
            }

            Player.TakeDamage(attack.Damage);
            Player.ApplyStun(attack.StunSeconds);
            Player.ApplyKnockback(GetExplosionKnockbackDirection(explosionBounds));
        }
    }

    private Vector2 GetExplosionKnockbackDirection(Rectangle explosionBounds)
    {
        var explosionCenter = new Vector2(explosionBounds.Center.X, explosionBounds.Center.Y);
        var playerCenter = new Vector2(Player.Bounds.Center.X, Player.Bounds.Center.Y);
        var knockbackDirection = playerCenter - explosionCenter;
        if (knockbackDirection.LengthSquared() > 0.0001f)
        {
            return knockbackDirection;
        }

        return -DirectionHelper.ToVector(Player.Facing);
    }

    private void UpdateToasts(FrameTime frameTime)
    {
        foreach (var toast in _toasts)
        {
            toast.Update(frameTime);
        }

        _toasts.RemoveAll(toast => !toast.IsActive);
    }

    private void UpdateScreenBanner(FrameTime frameTime)
    {
        _screenBanner?.Update(frameTime);
        if (_screenBanner is { IsActive: false })
        {
            _screenBanner = null;
        }
    }

    private void SpawnPendingEnemies()
    {
        foreach (var spawn in _enemies.SelectMany(enemy => enemy.ConsumePendingEnemySpawns()).ToArray())
        {
            _enemies.Add(_enemyFactory.Create(spawn));
        }
    }

    private void ApplyBossStageTransitionLockToPlayer()
    {
        var transitionSeconds = _enemies
            .Where(enemy => enemy.IsBossStageTransitioning)
            .Select(enemy => enemy.RemainingBossStageTransitionSeconds)
            .DefaultIfEmpty(0f)
            .Max();

        if (transitionSeconds > 0f)
        {
            Player.ApplyStun(transitionSeconds);
        }
    }

    private void QueueSceneTransitionIfTriggered()
    {
        if (_pendingSceneTransition is not null)
        {
            return;
        }

        foreach (var transition in _sceneTransitions)
        {
            if (_suppressedSceneTransitions.Contains(transition))
            {
                continue;
            }

            if (Player.Bounds.Intersects(transition.TriggerBounds) && transition.CanTrigger(this))
            {
                _pendingSceneTransition = transition;
                return;
            }
        }
    }

    private void ReleaseSceneTransitionSuppressions()
    {
        _suppressedSceneTransitions.RemoveWhere(transition => !Player.Bounds.Intersects(transition.TriggerBounds));
    }
}
