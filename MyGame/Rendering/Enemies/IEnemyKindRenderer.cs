using MyGame.Core;
using MyGame.Gameplay.Enemies;

namespace MyGame.Rendering.Enemies;

public interface IEnemyKindRenderer
{
    EnemyKind Kind { get; }

    void Draw(EnemyActor enemy, FrameTime frameTime);
}
