using Microsoft.Xna.Framework.Graphics;
using MyGame.Core.Assets;

namespace MyGame.Core.Scenes;

public interface IScene
{
    string Name { get; }

    void Enter();

    void Exit();

    void Update(FrameTime frameTime);

    void Draw(FrameTime frameTime, SpriteBatch spriteBatch, IAssetCatalog assetCatalog);

    IReadOnlyDictionary<string, string> GetDebugState();
}
