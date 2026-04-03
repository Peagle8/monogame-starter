using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.World;

namespace MyGame.Rendering.Gameplay;

public sealed class PlayerEntityRenderer : IGameplayEntityRenderer
{
    private readonly IRenderer<PlayerActor> _playerRenderer;

    public PlayerEntityRenderer(IRenderer<PlayerActor> playerRenderer)
    {
        _playerRenderer = playerRenderer;
    }

    public int DrawOrder => 100;

    public void Draw(World world, FrameTime frameTime)
    {
        _playerRenderer.Draw(world.Player, frameTime);
    }
}
