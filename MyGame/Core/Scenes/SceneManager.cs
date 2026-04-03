using Microsoft.Xna.Framework.Graphics;
using MyGame.Core.Assets;

namespace MyGame.Core.Scenes;

public sealed class SceneManager
{
    private IScene? _currentScene;

    public string CurrentSceneName => _currentScene?.Name ?? "<none>";

    public void ChangeScene(IScene nextScene)
    {
        _currentScene?.Exit();
        _currentScene = nextScene;
        _currentScene.Enter();
    }

    public void Update(FrameTime frameTime)
    {
        _currentScene?.Update(frameTime);
    }

    public void Draw(FrameTime frameTime, SpriteBatch spriteBatch, IAssetCatalog assetCatalog)
    {
        _currentScene?.Draw(frameTime, spriteBatch, assetCatalog);
    }

    public IReadOnlyDictionary<string, string> GetDebugState()
    {
        return _currentScene?.GetDebugState() ?? new Dictionary<string, string>();
    }
}
