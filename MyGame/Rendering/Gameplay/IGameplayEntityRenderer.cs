using MyGame.Core;
using MyGame.Scenes.Gameplay;

namespace MyGame.Rendering.Gameplay;

public interface IGameplayEntityRenderer
{
    int DrawOrder { get; }

    void Draw(GameplayScene scene, FrameTime frameTime);
}
