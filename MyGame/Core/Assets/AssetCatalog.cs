using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;
using System.IO;

namespace MyGame.Core.Assets;

public sealed class AssetCatalog : IAssetCatalog
{
    public AssetCatalog(ContentManager contentManager, GraphicsDevice graphicsDevice)
    {
        Pixel = new Texture2D(graphicsDevice, 1, 1);
        Pixel.SetData([Color.White]);

        BatSprite = LoadTextureFromOutput("Content", "BatSpriteSheet.png", graphicsDevice);
        CrabSprite = CreateTexture(graphicsDevice, CrabSpriteSheet.Rows);
        PlayerSprite = CreatePlayerSpriteSheet(graphicsDevice);
        DebugFont = TryLoadFont(contentManager, "DebugFont");
    }

    public Texture2D BatSprite { get; }

    public Texture2D CrabSprite { get; }

    public Texture2D Pixel { get; }

    public Texture2D PlayerSprite { get; }

    public SpriteFont? DebugFont { get; }

    private static Texture2D CreatePlayerSpriteSheet(GraphicsDevice graphicsDevice)
    {
        return CreateTexture(graphicsDevice, PlayerSpriteSheet.Rows);
    }

    private static Texture2D LoadTextureFromOutput(string relativeDirectory, string fileName, GraphicsDevice graphicsDevice)
    {
        var path = Path.Combine(AppContext.BaseDirectory, relativeDirectory, fileName);
        using var stream = File.OpenRead(path);
        var texture = Texture2D.FromStream(graphicsDevice, stream);
        ApplyWhiteTransparency(texture);
        return texture;
    }

    private static void ApplyWhiteTransparency(Texture2D texture)
    {
        var pixels = new Color[texture.Width * texture.Height];
        texture.GetData(pixels);

        for (var index = 0; index < pixels.Length; index++)
        {
            var pixel = pixels[index];
            if (pixel.A > 0 && pixel.R >= 250 && pixel.G >= 250 && pixel.B >= 250)
            {
                pixels[index] = Color.Transparent;
            }
        }

        texture.SetData(pixels);
    }

    private static Texture2D CreateTexture(GraphicsDevice graphicsDevice, IReadOnlyList<string> rows)
    {
        var width = rows[0].Length;
        var height = rows.Count;
        var texture = new Texture2D(graphicsDevice, width, height);
        var pixels = new Color[width * height];

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                pixels[(y * width) + x] = GetSpriteColor(rows[y][x]);
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
            'r' => new Color(196, 73, 56),
            'o' => new Color(229, 122, 52),
            'w' => new Color(255, 247, 236),
            'k' => new Color(88, 30, 26),
            _ => Color.Transparent
        };
    }
}
