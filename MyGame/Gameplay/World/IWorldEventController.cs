using MyGame.Core;

namespace MyGame.Gameplay.World;

public interface IWorldEventController
{
    bool IsComplete { get; }

    void Initialize(World world);

    void Update(World world, FrameTime frameTime);
}
