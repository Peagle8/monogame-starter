using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Props;
using MyGame.Gameplay.World;

namespace MyGame.Rendering.Gameplay;

public sealed class HouseExteriorEntityRenderer : IGameplayEntityRenderer
{
    private static readonly Color WallColor = new(211, 189, 154);
    private static readonly Color RoofColor = new(134, 78, 60);
    private static readonly Color RoofShadeColor = new(93, 52, 40);
    private static readonly Color DoorColor = new(89, 63, 47);
    private static readonly Color WindowColor = new(132, 192, 208);

    private readonly IWorldRectangleRenderer _worldRectangleRenderer;

    public HouseExteriorEntityRenderer(IWorldRectangleRenderer worldRectangleRenderer)
    {
        _worldRectangleRenderer = worldRectangleRenderer;
    }

    public int DrawOrder => 42;

    public void Draw(World world, FrameTime frameTime)
    {
        foreach (var house in world.GetProps<HouseExteriorProp>())
        {
            DrawHouse(house.Bounds);
        }
    }

    private void DrawHouse(Rectangle bounds)
    {
        var roofBounds = new Rectangle(
            bounds.X + 8,
            bounds.Y,
            Math.Max(24, bounds.Width - 16),
            Math.Max(20, bounds.Height / 4));
        var wallBounds = new Rectangle(
            bounds.X + 14,
            roofBounds.Bottom - 4,
            Math.Max(24, bounds.Width - 28),
            Math.Max(28, bounds.Height - roofBounds.Height - 8));
        var roofShadeBounds = new Rectangle(
            roofBounds.X,
            roofBounds.Bottom - 8,
            roofBounds.Width,
            8);
        var doorBounds = new Rectangle(
            wallBounds.Center.X - 12,
            wallBounds.Bottom - 28,
            24,
            28);
        var leftWindow = new Rectangle(wallBounds.X + 14, wallBounds.Y + 18, 20, 18);
        var rightWindow = new Rectangle(wallBounds.Right - 34, wallBounds.Y + 18, 20, 18);

        _worldRectangleRenderer.Draw(wallBounds, WallColor);
        _worldRectangleRenderer.Draw(roofBounds, RoofColor);
        _worldRectangleRenderer.Draw(roofShadeBounds, RoofShadeColor);
        _worldRectangleRenderer.Draw(leftWindow, WindowColor);
        _worldRectangleRenderer.Draw(rightWindow, WindowColor);
        _worldRectangleRenderer.Draw(doorBounds, DoorColor);
    }
}
