using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Scenes.MainMenu;

namespace MyGame.Rendering.MainMenu;

public sealed class MainMenuBackgroundRenderer : IRenderer<MainMenuScene>
{
    private readonly IRenderContext _renderContext;

    public MainMenuBackgroundRenderer(IRenderContext renderContext)
    {
        _renderContext = renderContext;
    }

    public void Draw(MainMenuScene model, FrameTime frameTime)
    {
        var viewportBounds = _renderContext.SpriteBatch.GraphicsDevice.Viewport.Bounds;
        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, viewportBounds, new Color(9, 17, 33));

        var stripeHeight = Math.Max(72, viewportBounds.Height / 6);
        for (var stripeIndex = 0; stripeIndex < 4; stripeIndex++)
        {
            var stripeBounds = new Rectangle(
                -40,
                70 + (stripeIndex * stripeHeight),
                viewportBounds.Width + 80,
                Math.Max(36, stripeHeight - 18));

            _renderContext.SpriteBatch.Draw(
                _renderContext.Assets.Pixel,
                stripeBounds,
                MainMenuBackgroundPalette.GetStripeColor(stripeIndex));
        }
    }
}
