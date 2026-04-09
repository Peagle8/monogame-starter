using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Props;
using MyGame.Gameplay.World;

namespace MyGame.Rendering.Gameplay;

public sealed class ShopkeeperEntityRenderer : IGameplayEntityRenderer
{
    private static readonly Color HeadColor = new(240, 198, 160);
    private static readonly Color HairColor = new(88, 55, 35);
    private static readonly Color ApronColor = new(60, 110, 146);
    private static readonly Color ShirtColor = new(231, 222, 196);

    private readonly IWorldRectangleRenderer _worldRectangleRenderer;

    public ShopkeeperEntityRenderer(IWorldRectangleRenderer worldRectangleRenderer)
    {
        _worldRectangleRenderer = worldRectangleRenderer;
    }

    public int DrawOrder => 98;

    public void Draw(World world, FrameTime frameTime)
    {
        foreach (var shopkeeper in world.GetProps<ShopkeeperProp>())
        {
            DrawShopkeeper(shopkeeper.Bounds);
        }
    }

    private void DrawShopkeeper(Rectangle bounds)
    {
        var headBounds = new Rectangle(bounds.X + 8, bounds.Y, bounds.Width - 16, 14);
        var hairBounds = new Rectangle(headBounds.X, headBounds.Y, headBounds.Width, 5);
        var shirtBounds = new Rectangle(bounds.X + 5, headBounds.Bottom, bounds.Width - 10, 10);
        var apronBounds = new Rectangle(bounds.X + 9, shirtBounds.Bottom, bounds.Width - 18, bounds.Height - headBounds.Height - shirtBounds.Height);

        _worldRectangleRenderer.Draw(headBounds, HeadColor);
        _worldRectangleRenderer.Draw(hairBounds, HairColor);
        _worldRectangleRenderer.Draw(shirtBounds, ShirtColor);
        _worldRectangleRenderer.Draw(apronBounds, ApronColor);
    }
}
