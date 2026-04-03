using Microsoft.Xna.Framework.Graphics;
using MyGame.Core.Assets;

namespace MyGame.Core.Rendering;

public interface IRenderContext
{
    SpriteBatch SpriteBatch { get; }

    IAssetCatalog Assets { get; }

    RenderCamera Camera { get; }

    void Bind(SpriteBatch spriteBatch, IAssetCatalog assetCatalog, RenderCamera camera);
}
