using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Scenes.Gameplay;

namespace MyGame.Rendering.Gameplay;

public sealed class GameplayOverlayRenderer : IRenderer<GameplayPauseMenu>
{
    private static readonly Vector2 InstructionPosition = new(10f, 440f);

    private readonly IRenderContext _renderContext;
    private readonly IRenderer<GameplayPauseMenu> _pauseMenuRenderer;

    public GameplayOverlayRenderer(IRenderContext renderContext, IRenderer<GameplayPauseMenu> pauseMenuRenderer)
    {
        _renderContext = renderContext;
        _pauseMenuRenderer = pauseMenuRenderer;
    }

    public void Draw(GameplayPauseMenu model, FrameTime frameTime)
    {
        if (_renderContext.Assets.DebugFont is not null)
        {
            _renderContext.SpriteBatch.DrawString(
                _renderContext.Assets.DebugFont,
                "Move with WASD or arrows. Press Esc or P to pause.",
                InstructionPosition,
                Color.White);
        }

        _pauseMenuRenderer.Draw(model, frameTime);
    }
}
