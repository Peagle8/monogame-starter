using MyGame.Core;
using MyGame.Gameplay.World;
using MyGame.Rendering.Gameplay;

namespace MyGame.Tests.Rendering.Gameplay;

public sealed class GameplayEntityRenderOrderingTests
{
    [Fact]
    public void Order_SortsRenderersByDrawOrder()
    {
        IGameplayEntityRenderer[] renderers =
        [
            new StubGameplayEntityRenderer(200),
            new StubGameplayEntityRenderer(50),
            new StubGameplayEntityRenderer(100)
        ];

        var ordered = GameplayEntityRenderOrdering.Order(renderers);

        Assert.Collection(
            ordered,
            renderer => Assert.Equal(50, renderer.DrawOrder),
            renderer => Assert.Equal(100, renderer.DrawOrder),
            renderer => Assert.Equal(200, renderer.DrawOrder));
    }

    private sealed class StubGameplayEntityRenderer : IGameplayEntityRenderer
    {
        public StubGameplayEntityRenderer(int drawOrder)
        {
            DrawOrder = drawOrder;
        }

        public int DrawOrder { get; }

        public void Draw(World world, FrameTime frameTime)
        {
        }
    }
}
