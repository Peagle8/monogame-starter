using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Gameplay.Enemies;

namespace MyGame.Gameplay.World;

public sealed class ArenaEncounterController : IWorldEventController
{
    private const float BannerDurationSeconds = 3f;

    private readonly IEnemyFactory _enemyFactory;
    private readonly IReadOnlyList<IReadOnlyList<EnemySpawnDefinition>> _waves;
    private readonly bool _fullHealBetweenWaves;
    private int _activeWaveIndex;
    private int _nextWaveIndex = 1;
    private float _remainingWaveDelaySeconds;
    private bool _hasAppliedInterWaveRecovery;

    public ArenaEncounterController(
        IEnemyFactory enemyFactory,
        bool fullHealBetweenWaves = true,
        params IEnumerable<EnemySpawnDefinition>[] waves)
    {
        _enemyFactory = enemyFactory;
        _fullHealBetweenWaves = fullHealBetweenWaves;
        _waves = waves.Select(wave => (IReadOnlyList<EnemySpawnDefinition>)wave.ToArray()).ToArray();
    }

    public bool IsComplete => _activeWaveIndex >= _waves.Count;

    public void Initialize(World world)
    {
        Reset(world);
    }

    public void Reset(World world)
    {
        _activeWaveIndex = 0;
        _nextWaveIndex = 1;
        _remainingWaveDelaySeconds = 0f;
        _hasAppliedInterWaveRecovery = false;
        SpawnWave(world, _waves[0]);
        world.ShowBanner(GetWaveLabel(1), BannerDurationSeconds);
    }

    public void Update(World world, FrameTime frameTime)
    {
        if (IsComplete)
        {
            return;
        }

        if (world.HasLivingEnemies())
        {
            return;
        }

        if (_nextWaveIndex >= _waves.Count)
        {
            _activeWaveIndex = _waves.Count;
            return;
        }

        if (_remainingWaveDelaySeconds <= 0f)
        {
            ApplyInterWaveRecovery(world);
            world.ShowBanner(GetWaveLabel(_nextWaveIndex + 1), BannerDurationSeconds);
            _remainingWaveDelaySeconds = BannerDurationSeconds;
        }

        _remainingWaveDelaySeconds = Math.Max(0f, _remainingWaveDelaySeconds - frameTime.DeltaSeconds);
        if (_remainingWaveDelaySeconds > 0f)
        {
            return;
        }

        SpawnWave(world, _waves[_nextWaveIndex]);
        _activeWaveIndex = _nextWaveIndex;
        _nextWaveIndex++;
        _remainingWaveDelaySeconds = 0f;
        _hasAppliedInterWaveRecovery = false;
    }

    private void SpawnWave(World world, IEnumerable<EnemySpawnDefinition> spawns)
    {
        foreach (var spawn in spawns)
        {
            world.SpawnEnemy(_enemyFactory.Create(spawn));
        }
    }

    private static string GetWaveLabel(int waveNumber)
    {
        return $"Wave {waveNumber}";
    }

    private void ApplyInterWaveRecovery(World world)
    {
        if (_hasAppliedInterWaveRecovery)
        {
            return;
        }

        _hasAppliedInterWaveRecovery = true;
        if (_fullHealBetweenWaves)
        {
            world.RestorePlayerToFull();
        }
    }
}
