using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Gameplay.Player;

namespace MyGame.Core.Assets;

public sealed class AssetCatalog : IAssetCatalog
{
    public AssetCatalog(ContentManager contentManager, GraphicsDevice graphicsDevice)
    {
        Pixel = new Texture2D(graphicsDevice, 1, 1);
        Pixel.SetData([Color.White]);

        PlayerSprite = CreatePlayerSpriteSheet(graphicsDevice);
        DebugFont = TryLoadFont(contentManager, "DebugFont");
    }

    public Texture2D Pixel { get; }

    public Texture2D PlayerSprite { get; }

    public SpriteFont? DebugFont { get; }

    private static Texture2D CreatePlayerSpriteSheet(GraphicsDevice graphicsDevice)
    {
        var texture = new Texture2D(graphicsDevice, PlayerSpriteSheet.SheetWidth, PlayerSpriteSheet.SheetHeight);
        var pixels = new Color[PlayerSpriteSheet.SheetWidth * PlayerSpriteSheet.SheetHeight];

        for (var y = 0; y < PlayerSpriteSheet.SheetHeight; y++)
        {
            for (var x = 0; x < PlayerSpriteSheet.SheetWidth; x++)
            {
                pixels[(y * PlayerSpriteSheet.SheetWidth) + x] = GetSpriteColor(PlayerSpriteSheet.Rows[y][x]);
            }
        }

        texture.SetData(pixels);
        return texture;
    }

    private static SpriteFont? TryLoadFont(ContentManager contentManager, string assetName)
    {
        try
        {
            return contentManager.Load<SpriteFont>(assetName);
        }
        catch (ContentLoadException)
        {
            return null;
        }
    }

    private static Color GetSpriteColor(char spriteKey)
    {
        return spriteKey switch
        {
            'h' => new Color(92, 56, 30),
            's' => new Color(240, 208, 176),
            'e' => new Color(38, 70, 83),
            'c' => new Color(46, 134, 171),
            'b' => new Color(58, 66, 74),
            _ => Color.Transparent
        };
    }
}
