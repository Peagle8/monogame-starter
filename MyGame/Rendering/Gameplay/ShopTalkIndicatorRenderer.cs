using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Props;
using MyGame.Gameplay.World;

namespace MyGame.Rendering.Gameplay;

public sealed class ShopTalkIndicatorRenderer : IGameplayEntityRenderer
{
    private static readonly Color OutlineColor = Color.Black;
    private static readonly Color FillColor = Color.White;

    private readonly IWorldRectangleRenderer _worldRectangleRenderer;

    public ShopTalkIndicatorRenderer(IWorldRectangleRenderer worldRectangleRenderer)
    {
        _worldRectangleRenderer = worldRectangleRenderer;
    }

    public int DrawOrder => 120;

    public void Draw(World world, FrameTime frameTime)
    {
        foreach (var shopkeeper in world.GetProps<ShopkeeperProp>())
        {
            DrawBalloon(shopkeeper.Bounds, frameTime.TotalSeconds);
        }
    }

    private void DrawBalloon(Rectangle shopkeeperBounds, float totalSeconds)
    {
        var bobOffset = (int)MathF.Round(MathF.Sin(totalSeconds * 4f) * 3f);
        var outerBounds = new Rectangle(
            shopkeeperBounds.Center.X - 16,
            shopkeeperBounds.Y - 34 + bobOffset,
            32,
            20);
        var innerBounds = new Rectangle(
            outerBounds.X + 2,
            outerBounds.Y + 2,
            outerBounds.Width - 4,
            outerBounds.Height - 4);
        var tailOuterTop = new Rectangle(outerBounds.Center.X - 3, outerBounds.Bottom - 2, 6, 6);
        var tailOuterBottom = new Rectangle(outerBounds.Center.X - 1, outerBounds.Bottom + 4, 4, 4);
        var tailInnerTop = new Rectangle(tailOuterTop.X + 1, tailOuterTop.Y + 1, tailOuterTop.Width - 2, tailOuterTop.Height - 2);
        var tailInnerBottom = new Rectangle(tailOuterBottom.X + 1, tailOuterBottom.Y + 1, tailOuterBottom.Width - 2, tailOuterBottom.Height - 2);

        _worldRectangleRenderer.Draw(outerBounds, OutlineColor);
        _worldRectangleRenderer.Draw(innerBounds, FillColor);
        _worldRectangleRenderer.Draw(tailOuterTop, OutlineColor);
        _worldRectangleRenderer.Draw(tailOuterBottom, OutlineColor);
        _worldRectangleRenderer.Draw(tailInnerTop, FillColor);
        _worldRectangleRenderer.Draw(tailInnerBottom, FillColor);
    }
}
