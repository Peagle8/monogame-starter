using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Scenes.Gameplay;

namespace MyGame.Rendering.Gameplay;

public sealed class GameplayWorldRenderer : IRenderer<GameplayScene>
{
    private const int TileSize = 48;
    private static readonly Rectangle ArenaBackgroundBounds = new(0, 0, 960, 576);

    private readonly IRenderContext _renderContext;
    private readonly IWorldRectangleRenderer _worldRectangleRenderer;
    private readonly IWorldSpriteRenderer _worldSpriteRenderer;

    public GameplayWorldRenderer(
        IRenderContext renderContext,
        IWorldRectangleRenderer worldRectangleRenderer,
        IWorldSpriteRenderer worldSpriteRenderer)
    {
        _renderContext = renderContext;
        _worldRectangleRenderer = worldRectangleRenderer;
        _worldSpriteRenderer = worldSpriteRenderer;
    }

    public void Draw(GameplayScene model, FrameTime frameTime)
    {
        var viewportBounds = _renderContext.SpriteBatch.GraphicsDevice.Viewport.Bounds;
        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, viewportBounds, new Color(18, 27, 31));

        if (TryDrawArenaBackground(model))
        {
            return;
        }

        var viewBounds = GetTileViewBounds(model.World.WorldBounds);
        var startColumn = (int)MathF.Floor(viewBounds.Left / (float)TileSize);
        var endColumn = (int)MathF.Ceiling(viewBounds.Right / (float)TileSize);
        var startRow = (int)MathF.Floor(viewBounds.Top / (float)TileSize);
        var endRow = (int)MathF.Ceiling(viewBounds.Bottom / (float)TileSize);

        for (var row = startRow; row <= endRow; row++)
        {
            for (var column = startColumn; column <= endColumn; column++)
            {
                var worldTileBounds = new Rectangle(
                    column * TileSize,
                    row * TileSize,
                    TileSize,
                    TileSize);

                _worldRectangleRenderer.Draw(worldTileBounds, CheckerboardFloorPalette.GetTileColor(column, row, model.Name));
            }
        }
    }

    private Rectangle GetTileViewBounds(Rectangle? worldBounds)
    {
        if (worldBounds is not Rectangle bounds)
        {
            return _renderContext.Camera.WorldViewBounds;
        }

        return Rectangle.Intersect(_renderContext.Camera.WorldViewBounds, bounds);
    }

    private bool TryDrawArenaBackground(GameplayScene model)
    {
        if (model.Name != GameplaySceneNames.Arena || _renderContext.Assets.ArenaBackground is null)
        {
            return false;
        }

        _worldSpriteRenderer.Draw(_renderContext.Assets.ArenaBackground, ArenaBackgroundBounds, Color.White);
        return true;
    }
}
