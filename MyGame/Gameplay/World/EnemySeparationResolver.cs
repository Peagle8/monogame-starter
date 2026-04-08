using Microsoft.Xna.Framework;
using MyGame.Gameplay.Enemies;

namespace MyGame.Gameplay.World;

public sealed class EnemySeparationResolver
{
    private const float MinimumDistanceEpsilon = 0.001f;
    private readonly WorldCombatSettings _settings;

    public EnemySeparationResolver(WorldCombatSettings settings)
    {
        _settings = settings;
    }

    public void Resolve(IReadOnlyList<EnemyActor> enemies)
    {
        var desiredDistance = _settings.EnemySeparationDistance;
        if (desiredDistance <= 0f || enemies.Count < 2)
        {
            return;
        }

        var iterations = Math.Max(1, _settings.EnemySeparationIterations);
        for (var iteration = 0; iteration < iterations; iteration++)
        {
            var movedAnyEnemy = false;

            for (var firstIndex = 0; firstIndex < enemies.Count - 1; firstIndex++)
            {
                var firstEnemy = enemies[firstIndex];
                if (firstEnemy.State == EnemyState.Dead)
                {
                    continue;
                }

                for (var secondIndex = firstIndex + 1; secondIndex < enemies.Count; secondIndex++)
                {
                    var secondEnemy = enemies[secondIndex];
                    if (secondEnemy.State == EnemyState.Dead)
                    {
                        continue;
                    }

                    movedAnyEnemy |= SeparatePair(firstEnemy, secondEnemy, desiredDistance);
                }
            }

            if (!movedAnyEnemy)
            {
                return;
            }
        }
    }

    private static bool SeparatePair(EnemyActor firstEnemy, EnemyActor secondEnemy, float desiredDistance)
    {
        var delta = secondEnemy.Position - firstEnemy.Position;
        var distance = delta.Length();

        if (distance >= desiredDistance)
        {
            return false;
        }

        var direction = distance <= MinimumDistanceEpsilon
            ? Vector2.UnitX
            : delta / distance;
        var overlap = desiredDistance - distance;
        var separation = direction * (overlap * 0.5f);

        firstEnemy.MoveBy(-separation);
        secondEnemy.MoveBy(separation);
        return true;
    }
}
