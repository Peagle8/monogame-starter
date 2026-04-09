using Microsoft.Xna.Framework;

namespace MyGame.Gameplay.World;

public sealed class WorldSceneTransition
{
    public WorldSceneTransition(Rectangle triggerBounds, string targetSceneName, Vector2 targetPlayerPosition)
    {
        TriggerBounds = triggerBounds;
        TargetSceneName = targetSceneName;
        TargetPlayerPosition = targetPlayerPosition;
    }

    public Rectangle TriggerBounds { get; }

    public string TargetSceneName { get; }

    public Vector2 TargetPlayerPosition { get; }
}
