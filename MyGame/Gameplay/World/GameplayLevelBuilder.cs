using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.Props;

namespace MyGame.Gameplay.World;

public sealed class GameplayLevelBuilder
{
    private readonly EnemySettings _defaultEnemySettings;
    private readonly IEnemyFactory _enemyFactory;
    private readonly IEnemySettingsCatalog _enemySettingsCatalog;
    private readonly WorldCombatSettings _worldCombatSettings;
    private readonly PlayerAttackHitResolver _playerAttackHitResolver;
    private readonly EnemyContactResolver _enemyContactResolver;

    public GameplayLevelBuilder(
        EnemySettings defaultEnemySettings,
        IEnemyFactory enemyFactory,
        IEnemySettingsCatalog enemySettingsCatalog,
        WorldCombatSettings worldCombatSettings,
        PlayerAttackHitResolver playerAttackHitResolver,
        EnemyContactResolver enemyContactResolver)
    {
        _defaultEnemySettings = defaultEnemySettings;
        _enemyFactory = enemyFactory;
        _enemySettingsCatalog = enemySettingsCatalog;
        _worldCombatSettings = worldCombatSettings;
        _playerAttackHitResolver = playerAttackHitResolver;
        _enemyContactResolver = enemyContactResolver;
    }

    public World BuildDefaultLevel(PlayerActor player)
    {
        TreeProp[] treeProps =
        [
            new(new Vector2(120f, 120f), new Point(72, 104)),
            new(new Vector2(560f, 160f), new Point(64, 96)),
            new(new Vector2(620f, 320f), new Point(80, 112))
        ];

        var spawnMap = new EnemySpawnMap(
        [
            new EnemySpawnDefinition(EnemyKind.HornedRabbit, new Vector2(560f, 180f), EnemyAxisPreference.Horizontal),
            new EnemySpawnDefinition(EnemyKind.HornedRabbit, new Vector2(660f, 240f), EnemyAxisPreference.Vertical),
            new EnemySpawnDefinition(EnemyKind.HornedRabbit, new Vector2(560f, 320f), EnemyAxisPreference.Horizontal),
            new EnemySpawnDefinition(EnemyKind.HornedRabbit, new Vector2(720f, 320f), EnemyAxisPreference.None)
        ]);

        return new World(
            player,
            treeProps,
            spawnMap.Spawns.Select(_enemyFactory.Create),
            _defaultEnemySettings,
            _enemySettingsCatalog,
            _enemyFactory,
            _playerAttackHitResolver,
            _enemyContactResolver,
            _worldCombatSettings);
    }
}
