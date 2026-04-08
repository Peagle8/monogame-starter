using Microsoft.Xna.Framework;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.Props;

namespace MyGame.Gameplay.World;

public sealed class WorldObstacleResolver
{
    private readonly WorldCombatSettings _settings;

    public WorldObstacleResolver(WorldCombatSettings settings)
    {
        _settings = settings;
    }

    public void ResolvePlayer(PlayerActor player, IReadOnlyList<IWorldProp> props)
    {
        if (player.IsDead)
        {
            return;
        }

        ResolveActors(
            iterations: _settings.PlayerObstacleResolutionIterations,
            props,
            [player],
            static actor => actor.Bounds,
            static actor => actor.PreviousBounds,
            static (actor, delta) => actor.MoveBy(delta));
    }

    public void Resolve(IReadOnlyList<EnemyActor> enemies, IReadOnlyList<IWorldProp> props)
    {
        ResolveActors(
            iterations: _settings.EnemyObstacleResolutionIterations,
            props,
            enemies.Where(enemy => enemy.State != EnemyState.Dead),
            static actor => actor.Bounds,
            static actor => actor.PreviousBounds,
            static (actor, delta) => actor.MoveBy(delta));
    }

    private static void ResolveActors<TActor>(
        int iterations,
        IReadOnlyList<IWorldProp> props,
        IEnumerable<TActor> actors,
        Func<TActor, Rectangle> getBounds,
        Func<TActor, Rectangle> getPreviousBounds,
        Action<TActor, Vector2> moveBy)
    {
        if (props.Count == 0)
        {
            return;
        }

        var actorList = actors.ToList();
        if (actorList.Count == 0)
        {
            return;
        }

        for (var iteration = 0; iteration < Math.Max(1, iterations); iteration++)
        {
            var movedAnyActor = false;

            foreach (var actor in actorList)
            {
                movedAnyActor |= ResolveActor(actor, props, getBounds, getPreviousBounds, moveBy);
            }

            if (!movedAnyActor)
            {
                return;
            }
        }
    }

    private static bool ResolveActor<TActor>(
        TActor actor,
        IReadOnlyList<IWorldProp> props,
        Func<TActor, Rectangle> getBounds,
        Func<TActor, Rectangle> getPreviousBounds,
        Action<TActor, Vector2> moveBy)
    {
        var movedActor = false;
        var previousBounds = getPreviousBounds(actor);
        foreach (var prop in props)
        {
            var actorBounds = getBounds(actor);
            if (!prop.BlocksMovement || !actorBounds.Intersects(prop.CollisionBounds))
            {
                continue;
            }

            moveBy(actor, ResolveOverlap(previousBounds, actorBounds, prop.CollisionBounds));
            movedActor = true;
        }

        return movedActor;
    }

    private static Vector2 ResolveOverlap(
        Rectangle previousBounds,
        Rectangle actorBounds,
        Rectangle obstacleBounds)
    {
        var moveLeft = obstacleBounds.Left - actorBounds.Right;
        var moveRight = obstacleBounds.Right - actorBounds.Left;
        var moveUp = obstacleBounds.Top - actorBounds.Bottom;
        var moveDown = obstacleBounds.Bottom - actorBounds.Top;

        if (previousBounds.Right <= obstacleBounds.Left)
        {
            return new Vector2(moveLeft, 0f);
        }

        if (previousBounds.Left >= obstacleBounds.Right)
        {
            return new Vector2(moveRight, 0f);
        }

        if (previousBounds.Bottom <= obstacleBounds.Top)
        {
            return new Vector2(0f, moveUp);
        }

        if (previousBounds.Top >= obstacleBounds.Bottom)
        {
            return new Vector2(0f, moveDown);
        }

        var actorCenterX = actorBounds.Center.X;
        var actorCenterY = actorBounds.Center.Y;
        var obstacleCenterX = obstacleBounds.Center.X;
        var obstacleCenterY = obstacleBounds.Center.Y;
        var horizontalMove = actorCenterX < obstacleCenterX ? moveLeft : moveRight;
        var verticalMove = actorCenterY < obstacleCenterY ? moveUp : moveDown;

        return Math.Abs(horizontalMove) < Math.Abs(verticalMove)
            ? new Vector2(horizontalMove, 0f)
            : new Vector2(0f, verticalMove);
    }
}
