using Microsoft.Xna.Framework.Graphics;
using MyGame.Core.Assets;
using MyGame.Core.Rendering;

namespace MyGame.Tests.Core.Rendering;

public sealed class RenderContextTests
{
    [Fact]
    public void SpriteBatch_ThrowsBeforeBind()
    {
        var renderContext = new RenderContext();

        var exception = Assert.Throws<InvalidOperationException>(() => _ = renderContext.SpriteBatch);

        Assert.Contains("SpriteBatch", exception.Message);
    }

    [Fact]
    public void Assets_ThrowsBeforeBind()
    {
        var renderContext = new RenderContext();

        var exception = Assert.Throws<InvalidOperationException>(() => _ = renderContext.Assets);

        Assert.Contains("assets", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Camera_ThrowsBeforeBind()
    {
        var renderContext = new RenderContext();

        var exception = Assert.Throws<InvalidOperationException>(() => _ = renderContext.Camera);

        Assert.Contains("camera", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Bind_StoresCurrentFrameDependencies()
    {
        var renderContext = new RenderContext();
        var spriteBatch = CreateUninitialized<SpriteBatch>();
        var assetCatalog = new StubAssetCatalog();
        var camera = RenderCamera.CreateIdentity(new Microsoft.Xna.Framework.Point(800, 480));

        renderContext.Bind(spriteBatch, assetCatalog, camera);

        Assert.Same(spriteBatch, renderContext.SpriteBatch);
        Assert.Same(assetCatalog, renderContext.Assets);
        Assert.Same(camera, renderContext.Camera);
    }

    private static T CreateUninitialized<T>()
        where T : class
    {
        return (T)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(T));
    }

    private sealed class StubAssetCatalog : IAssetCatalog
    {
        public Texture2D? ArenaBackground => null;

        public Texture2D BatSprite => null!;

        public Texture2D CrabSprite => null!;

        public Texture2D Pixel => null!;

        public Texture2D PlayerSprite => null!;

        public SpriteFont? DebugFont => null;
    }
}
