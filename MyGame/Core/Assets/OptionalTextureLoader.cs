using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace MyGame.Core.Assets;

public static class OptionalTextureLoader
{
    public static Texture2D? TryLoadFromContentOrProject(
        GraphicsDevice graphicsDevice,
        params string[] relativeSearchPaths)
    {
        foreach (var path in relativeSearchPaths)
        {
            var resolvedPath = TryResolvePath(path);
            if (resolvedPath is null)
            {
                continue;
            }

            using var stream = File.OpenRead(resolvedPath);
            return Texture2D.FromStream(graphicsDevice, stream);
        }

        return null;
    }

    public static string? TryResolvePath(string relativePath)
    {
        var currentDirectory = new DirectoryInfo(AppContext.BaseDirectory);

        while (currentDirectory is not null)
        {
            var candidate = Path.Combine(currentDirectory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            currentDirectory = currentDirectory.Parent;
        }

        return null;
    }
}
