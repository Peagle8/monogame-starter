using Microsoft.Xna.Framework;

namespace MyGame.Gameplay.Enemies;

internal static class ConeAttackHitTester
{
    public static bool Intersects(EnemyActor enemy, Rectangle targetBounds)
    {
        if (targetBounds.Width <= 0 || targetBounds.Height <= 0)
        {
            return false;
        }

        foreach (var segment in BatMiniBossConeShape.GetSegments(enemy))
        {
            if (segment.Intersects(targetBounds))
            {
                return true;
            }
        }

        return false;
    }
}
