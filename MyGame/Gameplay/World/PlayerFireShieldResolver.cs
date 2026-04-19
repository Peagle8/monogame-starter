using MyGame.Configuration;
using MyGame.Core;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;

namespace MyGame.Gameplay.World;

public sealed class PlayerFireShieldResolver
{
    private readonly PlayerDefenseAbilitySettings _settings;
    private readonly Dictionary<EnemyActor, float> _exposureSecondsByEnemy = [];

    public PlayerFireShieldResolver(PlayerDefenseAbilitySettings settings)
    {
        _settings = settings;
    }

    public bool Resolve(PlayerActor player, IReadOnlyList<EnemyActor> enemies, FrameTime frameTime)
    {
        if (!player.IsFireShieldActive || player.IsDead)
        {
            _exposureSecondsByEnemy.Clear();
            return false;
        }

        var hitEnemy = false;

        foreach (var enemy in enemies)
        {
            if (enemy.State == EnemyState.Dead || enemy.IsBossStageTransitioning)
            {
                _exposureSecondsByEnemy.Remove(enemy);
                continue;
            }

            if (!PlayerFireShieldArea.Intersects(player.Bounds, enemy.Bounds, _settings.FireShieldRadiusMultiplier))
            {
                _exposureSecondsByEnemy.Remove(enemy);
                continue;
            }

            var exposureSeconds = _exposureSecondsByEnemy.GetValueOrDefault(enemy) + frameTime.DeltaSeconds;

            while (exposureSeconds >= _settings.FireShieldDamageTickSeconds && enemy.State != EnemyState.Dead && !enemy.IsBossStageTransitioning)
            {
                enemy.TakeDamage(_settings.FireShieldDamage);
                exposureSeconds -= _settings.FireShieldDamageTickSeconds;
                hitEnemy = true;
            }

            if (enemy.State == EnemyState.Dead || enemy.IsBossStageTransitioning)
            {
                _exposureSecondsByEnemy.Remove(enemy);
                continue;
            }

            _exposureSecondsByEnemy[enemy] = exposureSeconds;
        }

        ClearRemovedEnemies(enemies);
        return hitEnemy;
    }

    public void Reset()
    {
        _exposureSecondsByEnemy.Clear();
    }

    private void ClearRemovedEnemies(IReadOnlyList<EnemyActor> enemies)
    {
        if (_exposureSecondsByEnemy.Count == 0)
        {
            return;
        }

        var removedEnemies = _exposureSecondsByEnemy.Keys
            .Where(enemy => !enemies.Contains(enemy))
            .ToArray();

        foreach (var enemy in removedEnemies)
        {
            _exposureSecondsByEnemy.Remove(enemy);
        }
    }
}
