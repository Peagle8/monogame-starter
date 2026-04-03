using Microsoft.Xna.Framework.Graphics;
using MyGame.Core.Assets;

namespace MyGame.Core.Rendering;

public sealed class RenderContext : IRenderContext
{
    private SpriteBatch? _spriteBatch;
    private IAssetCatalog? _assets;
    private RenderCamera? _camera;

    public SpriteBatch SpriteBatch => _spriteBatch ?? throw new InvalidOperationException("RenderContext is not bound to a SpriteBatch for the current frame.");

    public IAssetCatalog Assets => _assets ?? throw new InvalidOperationException("RenderContext is not bound to assets for the current frame.");

    public RenderCamera Camera => _camera ?? throw new InvalidOperationException("RenderContext is not bound to a camera for the current frame.");

    public void Bind(SpriteBatch spriteBatch, IAssetCatalog assetCatalog, RenderCamera camera)
    {
        _spriteBatch = spriteBatch;
        _assets = assetCatalog;
        _camera = camera;
    }
}
