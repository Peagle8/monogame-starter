using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Scenes.Gameplay;

namespace MyGame.Rendering.Gameplay;

public sealed class GameplaySceneRenderer : IRenderer<GameplayScene>
{
    private readonly GameplayWorldRenderer _worldRenderer;
    private readonly IReadOnlyList<IGameplayEntityRenderer> _entityRenderers;
    private readonly GameplayOverlayRenderer _overlayRenderer;

    public GameplaySceneRenderer(
        GameplayWorldRenderer worldRenderer,
        IEnumerable<IGameplayEntityRenderer> entityRenderers,
        GameplayOverlayRenderer overlayRenderer)
    {
        _worldRenderer = worldRenderer;
        _entityRenderers = GameplayEntityRenderOrdering.Order(entityRenderers);
        _overlayRenderer = overlayRenderer;
    }

    public void Draw(GameplayScene model, FrameTime frameTime)
    {
        _worldRenderer.Draw(model, frameTime);

        foreach (var entityRenderer in _entityRenderers)
        {
            entityRenderer.Draw(model.World, frameTime);
        }

        _overlayRenderer.Draw(model.PauseMenu, frameTime);
    }
}
