using Microsoft.Xna.Framework;
using MyGame.Gameplay.World;
using MyGame.Infrastructure.Save;

namespace MyGame.Gameplay.Enemies;

public interface IEnemyFactory
{
    EnemyActor Create(EnemySpawnDefinition spawn);

    EnemyActor CreateFromSaveData(EnemySaveData saveData);

    EnemyActor Create(EnemyKind kind, Vector2 position, EnemyAxisPreference axisPreference = EnemyAxisPreference.None);
}
