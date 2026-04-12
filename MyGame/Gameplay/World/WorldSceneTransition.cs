using Microsoft.Xna.Framework;

namespace MyGame.Gameplay.World;

public sealed class WorldSceneTransition
{
    private readonly Func<World, bool>? _canTrigger;

    public WorldSceneTransition(
        Rectangle triggerBounds,
        string targetSceneName,
        Vector2 targetPlayerPosition,
        Func<World, bool>? canTrigger = null)
    {
        TriggerBounds = triggerBounds;
        TargetSceneName = targetSceneName;
        TargetPlayerPosition = targetPlayerPosition;
        _canTrigger = canTrigger;
    }

    public Rectangle TriggerBounds { get; }

    public string TargetSceneName { get; }

    public Vector2 TargetPlayerPosition { get; }

    public bool CanTrigger(World world)
    {
        return _canTrigger?.Invoke(world) ?? true;
    }
}
