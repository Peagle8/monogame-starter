using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.World;

namespace MyGame.Rendering.Gameplay;

public sealed class PlayerAttackEffectRenderer : IGameplayEntityRenderer
{
    private static readonly Color BladeColor = new(226, 230, 236, 220);
    private static readonly Color BladeEdgeColor = new(255, 249, 222, 235);
    private static readonly Color HiltColor = new(124, 86, 48, 230);
    private static readonly Color GuardColor = new(196, 164, 88, 230);
    private static readonly Color TrailColor = new(255, 244, 194, 110);

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

        DrawSwordSwing(world.Player.AttackBounds.Value, world.Player.Facing, world.Player.AttackSequence);
    }

    private void DrawSwordSwing(Rectangle attackBounds, Direction facing, int attackSequence)
    {
        var isAlternateSwing = attackSequence % 2 == 0;
        var visualBounds = ExpandVisualBounds(attackBounds, facing);

        foreach (var segment in CreateTrailSegments(visualBounds, facing, isAlternateSwing))
        {
            _worldRectangleRenderer.Draw(segment, TrailColor);
        }

        _worldRectangleRenderer.Draw(CreateBladeBounds(visualBounds, facing, isAlternateSwing), BladeColor);
        _worldRectangleRenderer.Draw(CreateBladeEdgeBounds(visualBounds, facing, isAlternateSwing), BladeEdgeColor);
        _worldRectangleRenderer.Draw(CreateGuardBounds(visualBounds, facing, isAlternateSwing), GuardColor);
        _worldRectangleRenderer.Draw(CreateHiltBounds(visualBounds, facing, isAlternateSwing), HiltColor);
    }

    private static Rectangle ExpandVisualBounds(Rectangle attackBounds, Direction facing)
    {
        return facing switch
        {
            Direction.Up => new Rectangle(attackBounds.X - 4, attackBounds.Y - 6, attackBounds.Width + 8, attackBounds.Height + 6),
            Direction.Down => new Rectangle(attackBounds.X - 4, attackBounds.Y, attackBounds.Width + 8, attackBounds.Height + 6),
            Direction.Left => new Rectangle(attackBounds.X - 6, attackBounds.Y - 4, attackBounds.Width + 6, attackBounds.Height + 8),
            Direction.Right => new Rectangle(attackBounds.X, attackBounds.Y - 4, attackBounds.Width + 6, attackBounds.Height + 8),
            _ => attackBounds
        };
    }

    private static IReadOnlyList<Rectangle> CreateTrailSegments(Rectangle attackBounds, Direction facing, bool isAlternateSwing)
    {
        return facing switch
        {
            Direction.Up =>
            [
                new Rectangle(attackBounds.X + (isAlternateSwing ? 2 : 6), attackBounds.Y + 8, attackBounds.Width - 10, 6),
                new Rectangle(attackBounds.X + (isAlternateSwing ? 8 : 4), attackBounds.Y + 16, attackBounds.Width - 16, 6)
            ],
            Direction.Down =>
            [
                new Rectangle(attackBounds.X + (isAlternateSwing ? 6 : 2), attackBounds.Bottom - 14, attackBounds.Width - 10, 6),
                new Rectangle(attackBounds.X + (isAlternateSwing ? 4 : 8), attackBounds.Bottom - 22, attackBounds.Width - 16, 6)
            ],
            Direction.Left =>
            [
                new Rectangle(attackBounds.X + 8, attackBounds.Y + (isAlternateSwing ? 2 : 6), 6, attackBounds.Height - 10),
                new Rectangle(attackBounds.X + 16, attackBounds.Y + (isAlternateSwing ? 8 : 4), 6, attackBounds.Height - 16)
            ],
            Direction.Right =>
            [
                new Rectangle(attackBounds.Right - 14, attackBounds.Y + (isAlternateSwing ? 6 : 2), 6, attackBounds.Height - 10),
                new Rectangle(attackBounds.Right - 22, attackBounds.Y + (isAlternateSwing ? 4 : 8), 6, attackBounds.Height - 16)
            ],
            _ => []
        };
    }

    private static Rectangle CreateBladeBounds(Rectangle attackBounds, Direction facing, bool isAlternateSwing)
    {
        return facing switch
        {
            Direction.Up => new Rectangle(attackBounds.X + (isAlternateSwing ? 9 : 15), attackBounds.Y, 8, attackBounds.Height - 4),
            Direction.Down => new Rectangle(attackBounds.X + (isAlternateSwing ? 15 : 9), attackBounds.Y + 4, 8, attackBounds.Height - 4),
            Direction.Left => new Rectangle(attackBounds.X, attackBounds.Y + (isAlternateSwing ? 9 : 15), attackBounds.Width - 4, 8),
            Direction.Right => new Rectangle(attackBounds.X + 4, attackBounds.Y + (isAlternateSwing ? 15 : 9), attackBounds.Width - 4, 8),
            _ => attackBounds
        };
    }

    private static Rectangle CreateBladeEdgeBounds(Rectangle attackBounds, Direction facing, bool isAlternateSwing)
    {
        return facing switch
        {
            Direction.Up => new Rectangle(attackBounds.X + (isAlternateSwing ? 10 : 16), attackBounds.Y, 4, attackBounds.Height - 8),
            Direction.Down => new Rectangle(attackBounds.X + (isAlternateSwing ? 18 : 12), attackBounds.Y + 8, 4, attackBounds.Height - 8),
            Direction.Left => new Rectangle(attackBounds.X, attackBounds.Y + (isAlternateSwing ? 10 : 16), attackBounds.Width - 8, 4),
            Direction.Right => new Rectangle(attackBounds.X + 8, attackBounds.Y + (isAlternateSwing ? 18 : 12), attackBounds.Width - 8, 4),
            _ => attackBounds
        };
    }

    private static Rectangle CreateGuardBounds(Rectangle attackBounds, Direction facing, bool isAlternateSwing)
    {
        return facing switch
        {
            Direction.Up => new Rectangle(attackBounds.X + (isAlternateSwing ? 5 : 11), attackBounds.Bottom - 7, 16, 4),
            Direction.Down => new Rectangle(attackBounds.X + (isAlternateSwing ? 11 : 5), attackBounds.Y + 3, 16, 4),
            Direction.Left => new Rectangle(attackBounds.Right - 7, attackBounds.Y + (isAlternateSwing ? 5 : 11), 4, 16),
            Direction.Right => new Rectangle(attackBounds.X + 3, attackBounds.Y + (isAlternateSwing ? 11 : 5), 4, 16),
            _ => attackBounds
        };
    }

    private static Rectangle CreateHiltBounds(Rectangle attackBounds, Direction facing, bool isAlternateSwing)
    {
        return facing switch
        {
            Direction.Up => new Rectangle(attackBounds.X + (isAlternateSwing ? 10 : 16), attackBounds.Bottom - 3, 6, 3),
            Direction.Down => new Rectangle(attackBounds.X + (isAlternateSwing ? 16 : 10), attackBounds.Y, 6, 3),
            Direction.Left => new Rectangle(attackBounds.Right - 3, attackBounds.Y + (isAlternateSwing ? 10 : 16), 3, 6),
            Direction.Right => new Rectangle(attackBounds.X, attackBounds.Y + (isAlternateSwing ? 16 : 10), 3, 6),
            _ => attackBounds
        };
    }
}
