using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.World;

namespace MyGame.Rendering.Gameplay;

public sealed class PlayerAttackEffectRenderer : IGameplayEntityRenderer
{
    private static readonly Color SlashColor = new(255, 244, 194, 180);

    private readonly IWorldRectangleRenderer _worldRectangleRenderer;

    public PlayerAttackEffectRenderer(IWorldRectangleRenderer worldRectangleRenderer)
    {
        _worldRectangleRenderer = worldRectangleRenderer;
    }

    public int DrawOrder => 105;

    public void Draw(World world, FrameTime frameTime)
    {
        if (world.Player.AttackBounds is null)
        {
            return;
        }

        _worldRectangleRenderer.Draw(world.Player.AttackBounds.Value, SlashColor);
    }
}
