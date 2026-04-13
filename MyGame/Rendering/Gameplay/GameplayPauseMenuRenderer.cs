using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Inventory;
using MyGame.Rendering.Menus;
using MyGame.Scenes.Gameplay;

namespace MyGame.Rendering.Gameplay;

public sealed class GameplayPauseMenuRenderer : IRenderer<GameplayPauseMenu>
{
    private static readonly Rectangle PanelBounds = new(220, 68, 360, 344);
    private static readonly Rectangle MapModalBounds = new(118, 48, 564, 384);
    private static readonly Rectangle MapContentBounds = new(150, 120, 500, 260);
    private static readonly Rectangle InventoryModalBounds = new(168, 72, 464, 336);
    private static readonly Rectangle InventoryTabBounds = new(200, 132, 400, 40);
    private static readonly Rectangle InventoryContentBounds = new(200, 190, 400, 140);
    private static readonly Vector2 TitlePosition = new(330f, 104f);
    private static readonly Vector2 ItemsStartPosition = new(300f, 168f);
    private static readonly Vector2 FooterPosition = new(252f, 374f);
    private const float FooterWidth = 296f;
    private const float FooterLineSpacing = 18f;
    private const float ItemSpacing = 34f;
    private const float ControlsLineSpacing = 24f;
    private static readonly Color OverlayColor = new(8, 10, 14, 170);
    private static readonly Color ModalFillColor = new(243, 237, 226, 245);
    private static readonly Color ModalBorderColor = new(23, 23, 27);
    private static readonly Color HeaderColor = new(29, 35, 42);
    private static readonly Color MapFrameFillColor = new(232, 227, 212);
    private static readonly Color MapFrameBorderColor = new(45, 52, 59);
    private static readonly Color MapTownColor = new(192, 146, 88);
    private static readonly Color MapWildColor = new(118, 145, 96);
    private static readonly Color MapActiveColor = new(76, 108, 154);
    private static readonly Color MapPlayerColor = new(215, 78, 62);
    private static readonly Color ActiveTabFillColor = new(53, 81, 112);
    private static readonly Color InactiveTabFillColor = new(201, 194, 182);
    private static readonly Color ActiveTabTextColor = Color.White;
    private static readonly Color InactiveTabTextColor = new(54, 50, 47);

    private readonly IRenderContext _renderContext;

    public GameplayPauseMenuRenderer(IRenderContext renderContext)
    {
        _renderContext = renderContext;
    }

    public void Draw(GameplayPauseMenu model, FrameTime frameTime)
    {
        if (!model.IsOpen || _renderContext.Assets.DebugFont is null)
        {
            return;
        }

        var viewportBounds = _renderContext.SpriteBatch.GraphicsDevice.Viewport.Bounds;
        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, viewportBounds, Color.Black * 0.55f);

        if (model.IsShowingControls)
        {
            DrawControlsPanel();
            return;
        }

        if (model.IsShowingInventoryMenu)
        {
            DrawInventoryPanel(model);
            return;
        }

        if (model.IsShowingMap)
        {
            DrawMapPanel(model);
            return;
        }

        if (model.IsShowingReplayMenu)
        {
            DrawReplayPanel(model);
            return;
        }

        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, PanelBounds, Color.Black * 0.85f);
        _renderContext.SpriteBatch.DrawString(_renderContext.Assets.DebugFont, "Paused", TitlePosition, Color.White);

        for (var index = 0; index < model.Items.Count; index++)
        {
            _renderContext.SpriteBatch.DrawString(
                _renderContext.Assets.DebugFont,
                model.Items[index].Text,
                VerticalMenuLayout.GetItemPosition(ItemsStartPosition, ItemSpacing, index),
                VerticalMenuLayout.GetItemColor(index, model.SelectedIndex, model.Items[index].IsEnabled));
        }

        DrawFooter(model.FooterText);
    }

    private void DrawControlsPanel()
    {
        var viewport = _renderContext.SpriteBatch.GraphicsDevice.Viewport;
        var viewportSize = new Point(viewport.Width, viewport.Height);
        var panelBounds = ControlsOverlayLayout.GetPanelBounds(viewportSize);

        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, panelBounds, Color.Black * 0.9f);
        _renderContext.SpriteBatch.DrawString(
            _renderContext.Assets.DebugFont!,
            "Controls",
            ControlsOverlayLayout.GetTitlePosition(viewportSize),
            Color.White);

        for (var index = 0; index < ControlsOverlayText.Lines.Count; index++)
        {
            _renderContext.SpriteBatch.DrawString(
                _renderContext.Assets.DebugFont!,
                ControlsOverlayText.Lines[index],
                VerticalMenuLayout.GetItemPosition(
                    ControlsOverlayLayout.GetLinesStartPosition(viewportSize),
                    ControlsLineSpacing,
                    index),
                Color.White);
        }

        _renderContext.SpriteBatch.DrawString(
            _renderContext.Assets.DebugFont!,
            ControlsOverlayText.HintLineOne,
            ControlsOverlayLayout.GetHintLineOnePosition(viewportSize),
            new Color(170, 198, 190));

        _renderContext.SpriteBatch.DrawString(
            _renderContext.Assets.DebugFont!,
            ControlsOverlayText.HintLineTwo,
            ControlsOverlayLayout.GetHintLineTwoPosition(viewportSize),
            new Color(170, 198, 190));
    }

    private void DrawReplayPanel(GameplayPauseMenu model)
    {
        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, PanelBounds, Color.Black * 0.88f);
        _renderContext.SpriteBatch.DrawString(_renderContext.Assets.DebugFont!, "Replay", TitlePosition, Color.White);

        for (var index = 0; index < model.ReplayItems.Count; index++)
        {
            _renderContext.SpriteBatch.DrawString(
                _renderContext.Assets.DebugFont!,
                model.ReplayItems[index].Text,
                VerticalMenuLayout.GetItemPosition(ItemsStartPosition, ItemSpacing, index),
                VerticalMenuLayout.GetItemColor(index, model.ReplaySelectedIndex, model.ReplayItems[index].IsEnabled));
        }

        DrawFooter(model.FooterText);
    }

    private void DrawMapPanel(GameplayPauseMenu model)
    {
        var font = _renderContext.Assets.DebugFont!;
        var viewportBounds = _renderContext.SpriteBatch.GraphicsDevice.Viewport.Bounds;
        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, viewportBounds, OverlayColor);
        DrawPanel(MapModalBounds, ModalFillColor, ModalBorderColor);

        _renderContext.SpriteBatch.DrawString(
            font,
            "Map",
            new Vector2(MapModalBounds.X + 32, MapModalBounds.Y + 24),
            HeaderColor);

        _renderContext.SpriteBatch.DrawString(
            font,
            "The town sits at the center of the surrounding wilderness ring.",
            new Vector2(MapModalBounds.X + 32, MapModalBounds.Y + 58),
            new Color(82, 77, 72));

        DrawPanel(MapContentBounds, MapFrameFillColor, MapFrameBorderColor);
        DrawOverworldMap(model.MapSnapshot, font);

        _renderContext.SpriteBatch.DrawString(
            font,
            "Select / Tab / M / Esc to close",
            new Vector2(MapModalBounds.X + 32, MapModalBounds.Bottom - 34),
            new Color(82, 77, 72));
    }

    private void DrawInventoryPanel(GameplayPauseMenu model)
    {
        var font = _renderContext.Assets.DebugFont!;
        var viewportBounds = _renderContext.SpriteBatch.GraphicsDevice.Viewport.Bounds;
        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, viewportBounds, OverlayColor);
        DrawPanel(InventoryModalBounds, ModalFillColor, ModalBorderColor);

        _renderContext.SpriteBatch.DrawString(
            font,
            "Inventory",
            new Vector2(InventoryModalBounds.X + 32, InventoryModalBounds.Y + 28),
            HeaderColor);

        _renderContext.SpriteBatch.DrawString(
            font,
            "Browse your current equipment and progression tabs.",
            new Vector2(InventoryModalBounds.X + 32, InventoryModalBounds.Y + 62),
            new Color(82, 77, 72));

        DrawInventoryTabs(font, model.InventoryTab);
        DrawInventoryBody(font, model.InventoryTab);

        _renderContext.SpriteBatch.DrawString(
            font,
            "LB / RB or Q / R to switch tabs",
            new Vector2(InventoryModalBounds.X + 32, InventoryModalBounds.Bottom - 56),
            new Color(82, 77, 72));

        _renderContext.SpriteBatch.DrawString(
            font,
            "B / Esc to return",
            new Vector2(InventoryModalBounds.X + 32, InventoryModalBounds.Bottom - 30),
            new Color(82, 77, 72));
    }

    private void DrawFooter(string text)
    {
        var font = _renderContext.Assets.DebugFont!;
        var lines = WrappedTextLayout.WrapText(font, text, FooterWidth);

        for (var index = 0; index < lines.Count; index++)
        {
            _renderContext.SpriteBatch.DrawString(
                font,
                lines[index],
                new Vector2(FooterPosition.X, FooterPosition.Y + (index * FooterLineSpacing)),
                new Color(154, 178, 171));
        }
    }

    private void DrawInventoryTabs(Microsoft.Xna.Framework.Graphics.SpriteFont font, PlayerInventoryTab activeTab)
    {
        var tabWidth = InventoryTabBounds.Width / 4;
        DrawTab(font, new Rectangle(InventoryTabBounds.X, InventoryTabBounds.Y, tabWidth, InventoryTabBounds.Height), "Weapons", activeTab == PlayerInventoryTab.Weapons);
        DrawTab(font, new Rectangle(InventoryTabBounds.X + tabWidth, InventoryTabBounds.Y, tabWidth, InventoryTabBounds.Height), "Armor", activeTab == PlayerInventoryTab.Armor);
        DrawTab(font, new Rectangle(InventoryTabBounds.X + (tabWidth * 2), InventoryTabBounds.Y, tabWidth, InventoryTabBounds.Height), "Items", activeTab == PlayerInventoryTab.Items);
        DrawTab(font, new Rectangle(InventoryTabBounds.X + (tabWidth * 3), InventoryTabBounds.Y, tabWidth, InventoryTabBounds.Height), "Abilities", activeTab == PlayerInventoryTab.Abilities);
    }

    private void DrawInventoryBody(Microsoft.Xna.Framework.Graphics.SpriteFont font, PlayerInventoryTab activeTab)
    {
        DrawPanel(InventoryContentBounds, new Color(255, 252, 246), new Color(57, 53, 51));

        var heading = activeTab switch
        {
            PlayerInventoryTab.Weapons => "Weapons",
            PlayerInventoryTab.Armor => "Armor",
            PlayerInventoryTab.Items => "Items",
            _ => "Abilities"
        };

        var bodyLines = activeTab switch
        {
            PlayerInventoryTab.Weapons => new[] { "No weapons yet.", "Collected weapons will appear here once inventory data is hooked up." },
            PlayerInventoryTab.Armor => new[] { "No armor yet.", "Protective gear and equipment slots will appear here later." },
            PlayerInventoryTab.Items => new[] { "No items yet.", "Consumables, quest items, and materials will live here." },
            _ => new[] { "No abilities yet.", "Unlocked abilities and upgrades will appear here in a later pass." }
        };

        _renderContext.SpriteBatch.DrawString(
            font,
            heading,
            new Vector2(InventoryContentBounds.X + 18, InventoryContentBounds.Y + 18),
            HeaderColor);

        for (var index = 0; index < bodyLines.Length; index++)
        {
            _renderContext.SpriteBatch.DrawString(
                font,
                bodyLines[index],
                new Vector2(InventoryContentBounds.X + 18, InventoryContentBounds.Y + 56 + (index * 28)),
                new Color(68, 66, 62));
        }
    }

    private void DrawTab(Microsoft.Xna.Framework.Graphics.SpriteFont font, Rectangle bounds, string label, bool isActive)
    {
        DrawPanel(bounds, isActive ? ActiveTabFillColor : InactiveTabFillColor, ModalBorderColor);
        DrawCenteredText(font, label, bounds, isActive ? ActiveTabTextColor : InactiveTabTextColor);
    }

    private void DrawCenteredText(Microsoft.Xna.Framework.Graphics.SpriteFont font, string text, Rectangle bounds, Color color)
    {
        var textSize = font.MeasureString(text);
        var position = new Vector2(
            bounds.X + ((bounds.Width - textSize.X) * 0.5f),
            bounds.Y + ((bounds.Height - textSize.Y) * 0.5f));
        _renderContext.SpriteBatch.DrawString(font, text, position, color);
    }

    private void DrawPanel(Rectangle bounds, Color fillColor, Color borderColor)
    {
        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, bounds, fillColor);
        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, new Rectangle(bounds.X, bounds.Y, bounds.Width, 2), borderColor);
        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, new Rectangle(bounds.X, bounds.Bottom - 2, bounds.Width, 2), borderColor);
        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, new Rectangle(bounds.X, bounds.Y, 2, bounds.Height), borderColor);
        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, new Rectangle(bounds.Right - 2, bounds.Y, 2, bounds.Height), borderColor);
    }

    private void DrawOverworldMap(MyGame.Gameplay.World.OverworldMapSnapshot snapshot, Microsoft.Xna.Framework.Graphics.SpriteFont font)
    {
        foreach (var region in snapshot.Regions)
        {
            var bounds = ProjectMapBounds(region.Bounds, snapshot.MapBounds, MapContentBounds);
            var fillColor = region.SceneName == snapshot.CurrentSceneName
                ? MapActiveColor
                : region.IsTown
                    ? MapTownColor
                    : MapWildColor;

            DrawPanel(bounds, fillColor, MapFrameBorderColor);
            DrawCenteredText(font, region.Label, bounds, Color.White);
        }

        if (!snapshot.HasPlayerMarker)
        {
            return;
        }

        var markerPosition = ProjectMapPoint(snapshot.PlayerMapPosition, snapshot.MapBounds, MapContentBounds);
        var markerBounds = new Rectangle(
            (int)MathF.Round(markerPosition.X) - 4,
            (int)MathF.Round(markerPosition.Y) - 4,
            8,
            8);
        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, markerBounds, MapPlayerColor);
    }

    private static Rectangle ProjectMapBounds(Rectangle sourceBounds, Rectangle sourceArea, Rectangle targetArea)
    {
        var left = ProjectMapPoint(new Vector2(sourceBounds.Left, sourceBounds.Top), sourceArea, targetArea);
        var right = ProjectMapPoint(new Vector2(sourceBounds.Right, sourceBounds.Bottom), sourceArea, targetArea);
        return new Rectangle(
            (int)MathF.Round(left.X),
            (int)MathF.Round(left.Y),
            Math.Max(1, (int)MathF.Round(right.X - left.X)),
            Math.Max(1, (int)MathF.Round(right.Y - left.Y)));
    }

    private static Vector2 ProjectMapPoint(Vector2 point, Rectangle sourceArea, Rectangle targetArea)
    {
        var x = targetArea.Left + ((point.X - sourceArea.Left) / sourceArea.Width * targetArea.Width);
        var y = targetArea.Top + ((point.Y - sourceArea.Top) / sourceArea.Height * targetArea.Height);
        return new Vector2(x, y);
    }
}
