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
    private readonly List<IWorldProp> _props;
    private readonly List<PlayerProjectile> _playerProjectiles;
    private readonly List<WorldToast> _toasts;
    private readonly List<WorldSceneTransition> _sceneTransitions;
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
        IEnumerable<WorldSceneTransition>? sceneTransitions = null)
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
    }

    public PlayerActor Player { get; }

    public IReadOnlyList<EnemyActor> Enemies => _enemies;

    public int DefeatedEnemyCount => _countedDefeatedEnemies.Count;

    public IReadOnlyList<IWorldProp> Props => _props;

    public IReadOnlyList<TreeProp> TreeProps => _props.OfType<TreeProp>().ToArray();

    public IReadOnlyList<GrassProp> GrassProps => _props.OfType<GrassProp>().ToArray();

    public IReadOnlyList<PlayerProjectile> PlayerProjectiles => _playerProjectiles;

    public IReadOnlyList<WorldToast> Toasts => _toasts;

    public IReadOnlyList<TProp> GetProps<TProp>()
        where TProp : class, IWorldProp
    {
        return _props.OfType<TProp>().ToArray();
    }

    public void Update(FrameTime frameTime)
    {
        if (_remainingPlayerHitPauseSeconds > 0f)
        {
            _remainingPlayerHitPauseSeconds = Math.Max(0f, _remainingPlayerHitPauseSeconds - frameTime.DeltaSeconds);
            UpdateToasts(frameTime);
            return;
        }

        Player.Update(frameTime);
        _playerProjectiles.AddRange(Player.ConsumeSpawnedProjectiles());
        _worldObstacleResolver.ResolvePlayer(Player, _props);

        foreach (var enemy in _enemies)
        {
            enemy.Update(Player.Position, frameTime);
        }

        _worldObstacleResolver.Resolve(_enemies, _props);
        _enemySeparationResolver.Resolve(_enemies);

        UpdateProjectiles(frameTime);
        UpdateToasts(frameTime);
        var projectileHitEnemy = _playerProjectileResolver.Resolve(_playerProjectiles, _enemies, _props);
        _playerProjectiles.RemoveAll(projectile => !projectile.IsActive);

        var playerHitEnemy = _playerAttackHitResolver.Resolve(Player, _enemies);
        TrackDefeatedEnemies(rewardPlayer: true);

        if (playerHitEnemy || projectileHitEnemy)
        {
            _remainingPlayerHitPauseSeconds = _enemySettings.PlayerHitPauseSeconds;
            return;
        }

        _enemyContactResolver.Resolve(Player, _enemies, frameTime);
        _worldObstacleResolver.ResolvePlayer(Player, _props);
        QueueSceneTransitionIfTriggered();
    }

    public WorldSceneTransition? ConsumePendingSceneTransition()
    {
        var pendingTransition = _pendingSceneTransition;
        _pendingSceneTransition = null;
        return pendingTransition;
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

    private void UpdateToasts(FrameTime frameTime)
    {
        foreach (var toast in _toasts)
        {
            toast.Update(frameTime);
        }

        _toasts.RemoveAll(toast => !toast.IsActive);
    }

    private void QueueSceneTransitionIfTriggered()
    {
        if (_pendingSceneTransition is not null)
        {
            return;
        }

        foreach (var transition in _sceneTransitions)
        {
            if (Player.Bounds.Intersects(transition.TriggerBounds))
            {
                _pendingSceneTransition = transition;
                return;
            }
        }
    }
}
