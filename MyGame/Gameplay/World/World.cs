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
    // TODO: move these two constants into config
    private const int ContactDamage = 1;
    private const float ContactDamageCooldownSeconds = 0.5f;

    private readonly List<EnemyActor> _enemies;
    private readonly HashSet<EnemyActor> _countedDefeatedEnemies = [];
    private readonly Dictionary<EnemyActor, int> _enemyLastHitByAttackSequence = new();
    private readonly EnemySettings _enemySettings;
    private readonly IEnemySettingsCatalog _enemySettingsCatalog;
    private readonly List<TreeProp> _treeProps;
    private float _remainingContactDamageCooldown;
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
            enemySettings)
    {
    }

    public World(
        PlayerActor player,
        IEnumerable<TreeProp> treeProps,
        IEnumerable<EnemyActor> enemies,
        EnemySettings? enemySettings = null,
        IEnemySettingsCatalog? enemySettingsCatalog = null)
    {
        Player = player;
        _enemySettings = enemySettings ?? new EnemySettings();
        _enemySettingsCatalog = enemySettingsCatalog ?? new EnemySettingsCatalog(
            _enemySettings,
            EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbit));
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

        var playerHitEnemy = ResolvePlayerAttackHits();
        TrackDefeatedEnemies();

        if (playerHitEnemy)
        {
            _remainingPlayerHitPauseSeconds = _enemySettings.PlayerHitPauseSeconds;
            return;
        }

        ResolveEnemyContacts(frameTime);
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
        _remainingContactDamageCooldown = 0f;
        _remainingPlayerHitPauseSeconds = 0f;
        _enemyLastHitByAttackSequence.Clear();
        _countedDefeatedEnemies.Clear();
        _enemies.Clear();

        foreach (var enemyData in data.Enemies)
        {
            var enemySettings = _enemySettingsCatalog.Get(enemyData.Kind);
            var enemy = new EnemyActor(
                enemySettings,
                new Vector2(enemyData.PositionX, enemyData.PositionY),
                axisPreference: enemyData.AxisPreference);
            enemy.RestoreState(enemy.Position, enemyData.CurrentHealth);
            _enemies.Add(enemy);
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

    // TODO: this should be a small service injected and invoked rather than bake this into world
    // TODO: is there common elements between the player and enemy methods that could be refactored?
    private bool ResolvePlayerAttackHits()
    {
        if (!Player.IsAttacking || Player.AttackBounds is null)
        {
            return false;
        }

        var hitEnemy = false;

        foreach (var enemy in _enemies)
        {
            if (enemy.State == EnemyState.Dead)
            {
                continue;
            }

            if (_enemyLastHitByAttackSequence.TryGetValue(enemy, out var lastAttackSequence)
                && lastAttackSequence == Player.AttackSequence)
            {
                continue;
            }

            if (!enemy.Bounds.Intersects(Player.AttackBounds.Value))
            {
                continue;
            }

            enemy.TakeDamage(Player.AttackDamage);
            enemy.ApplyKnockback(GetEnemyKnockbackDirection(enemy));
            _enemyLastHitByAttackSequence[enemy] = Player.AttackSequence;
            hitEnemy = true;
        }

        return hitEnemy;
    }

    private Vector2 GetEnemyKnockbackDirection(EnemyActor enemy)
    {
        var knockbackDirection = enemy.Position - Player.Position;
        if (knockbackDirection.LengthSquared() > 0.0001f)
        {
            return knockbackDirection;
        }

        return Player.Facing switch
        {
            Direction.Up => new Vector2(0f, -1f),
            Direction.Down => new Vector2(0f, 1f),
            Direction.Left => new Vector2(-1f, 0f),
            Direction.Right => new Vector2(1f, 0f),
            _ => Vector2.Zero
        };
    }

    // TODO: this should prob be a small service and called from world rather then a private method here
    private void ResolveEnemyContacts(FrameTime frameTime)
    {
        _remainingContactDamageCooldown = Math.Max(0f, _remainingContactDamageCooldown - frameTime.DeltaSeconds);

        if (_remainingContactDamageCooldown > 0f || Player.IsDead)
        {
            return;
        }

        foreach (var enemy in _enemies)
        {
            if (!enemy.CanDealContactDamage)
            {
                continue;
            }

            if (!enemy.Bounds.Intersects(Player.Bounds))
            {
                continue;
            }

            Player.TakeDamage(ContactDamage);
            Player.ApplyKnockback(GetPlayerKnockbackDirection(enemy));
            enemy.BeginRecovery();
            _remainingContactDamageCooldown = ContactDamageCooldownSeconds;
            break;
        }
    }

    private Vector2 GetPlayerKnockbackDirection(EnemyActor enemy)
    {
        var knockbackDirection = Player.Position - enemy.Position;
        if (knockbackDirection.LengthSquared() > 0.0001f)
        {
            return knockbackDirection;
        }

        return Player.Facing switch
        {
            Direction.Up => new Vector2(0f, 1f),
            Direction.Down => new Vector2(0f, -1f),
            Direction.Left => new Vector2(1f, 0f),
            Direction.Right => new Vector2(-1f, 0f),
            _ => Vector2.Zero
        };
    }
}
