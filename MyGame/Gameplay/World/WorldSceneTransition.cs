using Microsoft.Xna.Framework;

namespace MyGame.Gameplay.World;

public sealed class WorldSceneTransition
{
    private readonly Func<World, bool>? _canTrigger;
    private readonly Func<World, Vector2>? _targetPositionResolver;

    public WorldSceneTransition(
        Rectangle triggerBounds,
        string targetSceneName,
        Vector2 targetPlayerPosition,
        Func<World, bool>? canTrigger = null)
        : this(triggerBounds, targetSceneName, _ => targetPlayerPosition, canTrigger)
    {
        TargetPlayerPosition = targetPlayerPosition;
    }

    public WorldSceneTransition(
        Rectangle triggerBounds,
        string targetSceneName,
        Func<World, Vector2> targetPositionResolver,
        Func<World, bool>? canTrigger = null)
    {
        TriggerBounds = triggerBounds;
        TargetSceneName = targetSceneName;
        TargetPlayerPosition = Vector2.Zero;
        _targetPositionResolver = targetPositionResolver;
        _canTrigger = canTrigger;
    }

    public Rectangle TriggerBounds { get; }

    public string TargetSceneName { get; }

    public Vector2 TargetPlayerPosition { get; }

    public bool CanTrigger(World world)
    {
        return _canTrigger?.Invoke(world) ?? true;
    }

    public Vector2 ResolveTargetPlayerPosition(World world)
    {
        return _targetPositionResolver?.Invoke(world) ?? TargetPlayerPosition;
    }
}
