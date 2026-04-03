using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Player;
using MyGame.Scenes.Gameplay;

namespace MyGame.Rendering.Gameplay;

public sealed class PlayerEntityRenderer : IGameplayEntityRenderer
{
    private readonly IRenderer<PlayerActor> _playerRenderer;

    public PlayerEntityRenderer(IRenderer<PlayerActor> playerRenderer)
    {
        _playerRenderer = playerRenderer;
    }

    public int DrawOrder => 100;

    public void Draw(GameplayScene scene, FrameTime frameTime)
    {
        _playerRenderer.Draw(scene.Player, frameTime);
    }
}
