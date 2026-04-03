namespace MyGame.Rendering.Gameplay;

public static class GameplayEntityRenderOrdering
{
    public static IReadOnlyList<IGameplayEntityRenderer> Order(IEnumerable<IGameplayEntityRenderer> renderers)
    {
        return renderers
            .OrderBy(renderer => renderer.DrawOrder)
            .ToArray();
    }
}
