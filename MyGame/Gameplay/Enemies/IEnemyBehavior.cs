using Microsoft.Xna.Framework;
using MyGame.Core;

namespace MyGame.Gameplay.Enemies;

internal interface IEnemyBehavior
{
    void Update(EnemyActor enemy, Vector2 playerPosition, Rectangle playerBounds, FrameTime frameTime);

    void Reset(EnemyActor enemy);
}
