using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Props;
using MyGame.Gameplay.World;

namespace MyGame.Rendering.Gameplay;

public sealed class TownsfolkEntityRenderer : IGameplayEntityRenderer
{
    private static readonly Color HeadColor = new(222, 178, 136);
    private static readonly Color HairColor = new(52, 45, 40);
    private static readonly Color TunicColor = new(108, 132, 82);
    private static readonly Color PantsColor = new(69, 76, 88);

    private readonly IWorldRectangleRenderer _worldRectangleRenderer;

    public TownsfolkEntityRenderer(IWorldRectangleRenderer worldRectangleRenderer)
    {
        _worldRectangleRenderer = worldRectangleRenderer;
    }

    public int DrawOrder => 98;

    public void Draw(World world, FrameTime frameTime)
    {
        foreach (var townsfolk in world.GetProps<TownsfolkProp>())
        {
            DrawTownsfolk(townsfolk.Bounds);
        }
    }

    private void DrawTownsfolk(Rectangle bounds)
    {
        var headBounds = new Rectangle(bounds.X + 7, bounds.Y, bounds.Width - 14, 13);
        var hairBounds = new Rectangle(headBounds.X, headBounds.Y, headBounds.Width, 4);
        var tunicBounds = new Rectangle(bounds.X + 5, headBounds.Bottom, bounds.Width - 10, 17);
        var pantsBounds = new Rectangle(bounds.X + 8, tunicBounds.Bottom, bounds.Width - 16, bounds.Bottom - tunicBounds.Bottom);

        _worldRectangleRenderer.Draw(headBounds, HeadColor);
        _worldRectangleRenderer.Draw(hairBounds, HairColor);
        _worldRectangleRenderer.Draw(tunicBounds, TunicColor);
        _worldRectangleRenderer.Draw(pantsBounds, PantsColor);
    }
}
