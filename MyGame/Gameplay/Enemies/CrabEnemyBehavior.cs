using Microsoft.Xna.Framework;
using MyGame.Core;

namespace MyGame.Gameplay.Enemies;

internal sealed class CrabEnemyBehavior : IEnemyBehavior
{
    public void Update(EnemyActor enemy, Vector2 playerPosition, Rectangle playerBounds, FrameTime frameTime)
    {
        var toPlayer = playerPosition - enemy.Position;
        var distanceToPlayer = toPlayer.Length();

        if (distanceToPlayer > enemy.Settings.ChaseRange || distanceToPlayer <= 0.001f)
        {
            enemy.SetState(EnemyState.Idle, isMoving: false);
            return;
        }

        toPlayer.Normalize();
        enemy.MoveBy(toPlayer * enemy.Settings.MoveSpeed * frameTime.DeltaSeconds);
        enemy.SetState(EnemyState.Chasing, isMoving: true);
    }

    public void Reset(EnemyActor enemy)
    {
        enemy.DashDirection = MyGame.Gameplay.Player.Direction.Left;
    }
}
