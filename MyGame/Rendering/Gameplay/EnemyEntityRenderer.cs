using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.World;

namespace MyGame.Rendering.Gameplay;

public sealed class EnemyEntityRenderer : IGameplayEntityRenderer
{
    private readonly IRenderer<EnemyActor> _enemyRenderer;

    public EnemyEntityRenderer(IRenderer<EnemyActor> enemyRenderer)
    {
        _enemyRenderer = enemyRenderer;
    }

    public int DrawOrder => 90;

    public void Draw(World world, FrameTime frameTime)
    {
        foreach (var enemy in world.Enemies)
        {
            if (!enemy.IsRenderable)
            {
                continue;
            }

            _enemyRenderer.Draw(enemy, frameTime);
        }
    }
}
