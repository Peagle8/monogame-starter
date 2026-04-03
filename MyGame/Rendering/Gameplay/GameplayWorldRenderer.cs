using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Scenes.Gameplay;

namespace MyGame.Rendering.Gameplay;

public sealed class GameplayWorldRenderer : IRenderer<GameplayScene>
{
    private const int TileSize = 48;

    private readonly IRenderContext _renderContext;
    private readonly IWorldRectangleRenderer _worldRectangleRenderer;

    public GameplayWorldRenderer(IRenderContext renderContext, IWorldRectangleRenderer worldRectangleRenderer)
    {
        _renderContext = renderContext;
        _worldRectangleRenderer = worldRectangleRenderer;
    }

    public void Draw(GameplayScene model, FrameTime frameTime)
    {
        var viewportBounds = _renderContext.SpriteBatch.GraphicsDevice.Viewport.Bounds;
        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, viewportBounds, new Color(18, 27, 31));

        var viewBounds = _renderContext.Camera.WorldViewBounds;
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

                _worldRectangleRenderer.Draw(worldTileBounds, CheckerboardFloorPalette.GetTileColor(column, row));
            }
        }
    }
}
