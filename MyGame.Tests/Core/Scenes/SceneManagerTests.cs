using Microsoft.Xna.Framework.Graphics;
using MyGame.Core;
using MyGame.Core.Assets;
using MyGame.Core.Scenes;

namespace MyGame.Tests.Core.Scenes;

public sealed class SceneManagerTests
{
    [Fact]
    public void ChangeScene_ExitsPreviousScene_AndEntersNextScene()
    {
        var sceneManager = new SceneManager();
        var firstScene = new FakeScene("First");
        var secondScene = new FakeScene("Second");

        sceneManager.ChangeScene(firstScene);
        sceneManager.ChangeScene(secondScene);

        Assert.Equal(1, firstScene.ExitCalls);
        Assert.Equal(1, firstScene.EnterCalls);
        Assert.Equal(1, secondScene.EnterCalls);
        Assert.Equal("Second", sceneManager.CurrentSceneName);
    }

    private sealed class FakeScene : IScene
    {
        public FakeScene(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public int EnterCalls { get; private set; }

        public int ExitCalls { get; private set; }

        public void Enter()
        {
            EnterCalls++;
        }

        public void Exit()
        {
            ExitCalls++;
        }

        public void Update(FrameTime frameTime)
        {
        }

        public void Draw(FrameTime frameTime, SpriteBatch spriteBatch, IAssetCatalog assetCatalog)
        {
        }

        public IReadOnlyDictionary<string, string> GetDebugState()
        {
            return new Dictionary<string, string>();
        }
    }
}
