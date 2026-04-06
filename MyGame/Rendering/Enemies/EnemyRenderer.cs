using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Enemies;

namespace MyGame.Rendering.Enemies;

public sealed class EnemyRenderer : IRenderer<EnemyActor>
{
    private readonly IReadOnlyDictionary<EnemyKind, IEnemyKindRenderer> _renderersByKind;

    public EnemyRenderer(IEnumerable<IEnemyKindRenderer> renderers)
    {
        _renderersByKind = renderers.ToDictionary(renderer => renderer.Kind);
    }

    public void Draw(EnemyActor model, FrameTime frameTime)
    {
        if (_renderersByKind.TryGetValue(model.Kind, out var renderer))
        {
            renderer.Draw(model, frameTime);
            return;
        }

        throw new ArgumentOutOfRangeException(nameof(model.Kind), model.Kind, "Unsupported enemy kind.");
    }
}
