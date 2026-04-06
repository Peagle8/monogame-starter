using MyGame.Configuration;

namespace MyGame.Gameplay.Enemies;

public interface IEnemySettingsCatalog
{
    EnemySettings Get(EnemyKind kind);
}
