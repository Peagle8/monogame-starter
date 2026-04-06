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
    private readonly EnemyContactResolver _enemyContactResolver;
    private readonly List<TreeProp> _treeProps;
    private float _remainingPlayerHitPauseSeconds;

    public World(PlayerActor player, EnemySettings enemySettings)
        : this(
            player,
            [
                new TreeProp(new Vector2(120f, 120f), new Point(72, 104)),
                new TreeProp(new Vector2(560f, 160f), new Point(64, 96)),
                new TreeProp(new Vector2(620f, 320f), new Point(80, 112))
            ],
            [new EnemyActor(enemySettings, new Vector2(520f, 240f))],
            enemySettings,
            enemyFactory: new EnemyFactory(new EnemySettingsCatalog(
                enemySettings,
                EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbit))),
            playerAttackHitResolver: new PlayerAttackHitResolver(),
            enemyContactResolver: new EnemyContactResolver(new WorldCombatSettings()),
            worldCombatSettings: new WorldCombatSettings())
    {
    }

    public World(
        PlayerActor player,
        IEnumerable<TreeProp> treeProps,
        IEnumerable<EnemyActor> enemies,
        EnemySettings? enemySettings = null,
        IEnemySettingsCatalog? enemySettingsCatalog = null,
        IEnemyFactory? enemyFactory = null,
        PlayerAttackHitResolver? playerAttackHitResolver = null,
        EnemyContactResolver? enemyContactResolver = null,
        WorldCombatSettings? worldCombatSettings = null)
    {
        Player = player;
        _enemySettings = enemySettings ?? new EnemySettings();
        _enemySettingsCatalog = enemySettingsCatalog ?? new EnemySettingsCatalog(
            _enemySettings,
            EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbit));
        var resolvedWorldCombatSettings = worldCombatSettings ?? new WorldCombatSettings();
        _enemyFactory = enemyFactory ?? new EnemyFactory(_enemySettingsCatalog);
        _playerAttackHitResolver = playerAttackHitResolver ?? new PlayerAttackHitResolver();
        _enemyContactResolver = enemyContactResolver ?? new EnemyContactResolver(resolvedWorldCombatSettings);
        _treeProps = treeProps.ToList();
        _enemies = enemies.ToList();
    }

    public PlayerActor Player { get; }

    public IReadOnlyList<EnemyActor> Enemies => _enemies;

    public int DefeatedEnemyCount => _countedDefeatedEnemies.Count;

    public IReadOnlyList<TreeProp> TreeProps => _treeProps;

    public void Update(FrameTime frameTime)
    {
        if (_remainingPlayerHitPauseSeconds > 0f)
        {
            _remainingPlayerHitPauseSeconds = Math.Max(0f, _remainingPlayerHitPauseSeconds - frameTime.DeltaSeconds);
            return;
        }

        Player.Update(frameTime);

        foreach (var enemy in _enemies)
        {
            enemy.Update(Player.Position, frameTime);
        }

        var playerHitEnemy = _playerAttackHitResolver.Resolve(Player, _enemies);
        TrackDefeatedEnemies();

        if (playerHitEnemy)
        {
            _remainingPlayerHitPauseSeconds = _enemySettings.PlayerHitPauseSeconds;
            return;
        }

        _enemyContactResolver.Resolve(Player, _enemies, frameTime);
    }

    public IReadOnlyDictionary<string, string> GetDebugState()
    {
        return new Dictionary<string, string>
        {
            ["DefeatedEnemyCount"] = DefeatedEnemyCount.ToString(),
            ["EnemyCount"] = _enemies.Count.ToString(),
            ["FirstEnemyState"] = _enemies.FirstOrDefault()?.State.ToString() ?? "<none>",
            ["PlayerAttackActive"] = Player.IsAttacking.ToString(),
            ["PlayerDead"] = Player.IsDead.ToString(),
            ["PlayerHealth"] = $"{Player.CurrentHealth}/{Player.MaxHealth}",
            ["PlayerPosition"] = $"{Player.Position.X:0.00}, {Player.Position.Y:0.00}",
            ["PlayerFacing"] = Player.Facing.ToString(),
            ["TreePropCount"] = _treeProps.Count.ToString()
        };
    }

    public SaveGameData CreateSaveData(string sceneName)
    {
        return new SaveGameData
        {
            SceneName = sceneName,
            DefeatedEnemyCount = DefeatedEnemyCount,
            Enemies = _enemies.Select(enemy => enemy.CreateSaveData()).ToArray(),
            PlayerHealth = Player.CurrentHealth,
            PlayerPositionX = Player.Position.X,
            PlayerPositionY = Player.Position.Y
        };
    }

    public void ApplySaveData(SaveGameData data)
    {
        Player.RestoreState(new Vector2(data.PlayerPositionX, data.PlayerPositionY), data.PlayerHealth);
        _remainingPlayerHitPauseSeconds = 0f;
        _playerAttackHitResolver.Reset();
        _enemyContactResolver.Reset();
        _countedDefeatedEnemies.Clear();
        _enemies.Clear();

        foreach (var enemyData in data.Enemies)
        {
            _enemies.Add(_enemyFactory.CreateFromSaveData(enemyData));
        }

        TrackDefeatedEnemies();
    }

    private void TrackDefeatedEnemies()
    {
        foreach (var enemy in _enemies)
        {
            if (enemy.State == EnemyState.Dead)
            {
                _countedDefeatedEnemies.Add(enemy);
            }
        }
    }
}
