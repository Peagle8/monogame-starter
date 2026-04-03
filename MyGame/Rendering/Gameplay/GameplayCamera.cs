using Microsoft.Xna.Framework;
using MyGame.Core.Rendering;

namespace MyGame.Rendering.Gameplay;

public static class GameplayCamera
{
    public static RenderCamera Create(Vector2 playerPosition, Point viewportSize, Point playerSize)
    {
        var focusPosition = playerPosition + new Vector2(playerSize.X / 2f, playerSize.Y / 2f);
        var worldTopLeft = new Vector2(
            focusPosition.X - (viewportSize.X / 2f),
            focusPosition.Y - (viewportSize.Y / 2f));

        return new RenderCamera(worldTopLeft, viewportSize);
    }
}
