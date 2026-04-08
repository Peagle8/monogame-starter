using Microsoft.Xna.Framework;
using MyGame.Core.Rendering;

namespace MyGame.Rendering.Gameplay;

public static class GameplayCamera
{
    private const float DefaultZoom = 1.15f;

    public static RenderCamera Create(Vector2 playerPosition, Point viewportSize, Point playerSize)
    {
        var focusPosition = playerPosition + new Vector2(playerSize.X / 2f, playerSize.Y / 2f);
        var worldViewWidth = viewportSize.X / DefaultZoom;
        var worldViewHeight = viewportSize.Y / DefaultZoom;
        var worldTopLeft = new Vector2(
            focusPosition.X - (worldViewWidth / 2f),
            focusPosition.Y - (worldViewHeight / 2f));

        return new RenderCamera(worldTopLeft, viewportSize, DefaultZoom);
    }
}
