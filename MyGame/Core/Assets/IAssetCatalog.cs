using Microsoft.Xna.Framework.Graphics;

namespace MyGame.Core.Assets;

public interface IAssetCatalog
{
    Texture2D CrabSprite { get; }

    Texture2D Pixel { get; }

    Texture2D PlayerSprite { get; }

    SpriteFont? DebugFont { get; }
}
