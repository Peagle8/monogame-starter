using MyGame.Core;
using MyGame.Gameplay.World;

namespace MyGame.Rendering.Gameplay;

public interface IGameplayEntityRenderer
{
    int DrawOrder { get; }

    void Draw(World world, FrameTime frameTime);
}
