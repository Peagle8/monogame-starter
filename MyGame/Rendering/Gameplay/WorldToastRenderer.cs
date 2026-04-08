using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.World;

namespace MyGame.Rendering.Gameplay;

public sealed class WorldToastRenderer : IGameplayEntityRenderer
{
    private readonly IRenderContext _renderContext;

    public WorldToastRenderer(IRenderContext renderContext)
    {
        _renderContext = renderContext;
    }

    public int DrawOrder => 110;

    public void Draw(World world, FrameTime frameTime)
    {
        var font = _renderContext.Assets.DebugFont;
        if (font is null)
        {
            return;
        }

        foreach (var toast in world.Toasts)
        {
            if (!toast.IsActive)
            {
                continue;
            }

            var screenPosition = _renderContext.Camera.WorldToScreen(toast.Position);
            var textSize = font.MeasureString(toast.Text);
            _renderContext.SpriteBatch.DrawString(
                font,
                toast.Text,
                screenPosition - new Vector2(textSize.X * 0.5f, textSize.Y),
                toast.Color * toast.Alpha);
        }
    }
}
