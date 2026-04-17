using MyGame.Core.Assets;

namespace MyGame.Tests.Core.Assets;

public sealed class OptionalTextureLoaderTests
{
    [Fact]
    public void TryResolvePath_WhenFileExistsInProjectContent_ReturnsAbsolutePath()
    {
        var path = OptionalTextureLoader.TryResolvePath(Path.Combine("MyGame", "Content", "Content.mgcb"));

        Assert.NotNull(path);
        Assert.True(Path.IsPathRooted(path));
        Assert.EndsWith(Path.Combine("MyGame", "Content", "Content.mgcb"), path!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TryResolvePath_WhenFileIsMissing_ReturnsNull()
    {
        var path = OptionalTextureLoader.TryResolvePath(Path.Combine("MyGame", "Content", "DefinitelyMissingArenaArt.png"));

        Assert.Null(path);
    }
}
