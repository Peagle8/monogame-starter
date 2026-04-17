using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Inventory;
using MyGame.Rendering.Menus;
using MyGame.Scenes.Gameplay;

namespace MyGame.Rendering.Gameplay;

public sealed class GameplayPauseMenuRenderer : IRenderer<GameplayPauseMenu>
{
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
        var viewportSize = new Point(viewportBounds.Width, viewportBounds.Height);
        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, viewportBounds, Color.Black * 0.55f);

        if (model.IsShowingControls)
        {
            DrawControlsPanel(viewportSize);
            return;
        }

        if (model.IsShowingInventoryMenu)
        {
            DrawInventoryPanel(model, viewportBounds, viewportSize);
            return;
        }

        if (model.IsShowingMap)
        {
            DrawMapPanel(model, viewportBounds, viewportSize);
            return;
        }

        if (model.IsShowingReplayMenu)
        {
            DrawReplayPanel(model, viewportSize);
            return;
        }

        DrawMainPausePanel(model, viewportSize);
    }

    private void DrawMainPausePanel(GameplayPauseMenu model, Point viewportSize)
    {
        var panelBounds = GameplayPauseMenuLayout.GetMenuPanelBounds(viewportSize);
        var titlePosition = GameplayPauseMenuLayout.GetMenuTitlePosition(panelBounds);
        var itemsStartPosition = GameplayPauseMenuLayout.GetMenuItemsStartPosition(panelBounds);

        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, panelBounds, Color.Black * 0.85f);
        _renderContext.SpriteBatch.DrawString(_renderContext.Assets.DebugFont!, "Paused", titlePosition, Color.White);

        for (var index = 0; index < model.Items.Count; index++)
        {
            _renderContext.SpriteBatch.DrawString(
                _renderContext.Assets.DebugFont!,
                model.Items[index].Text,
                VerticalMenuLayout.GetItemPosition(itemsStartPosition, ItemSpacing, index),
                VerticalMenuLayout.GetItemColor(index, model.SelectedIndex, model.Items[index].IsEnabled));
        }

        DrawFooter(model.FooterText, panelBounds);
    }

    private void DrawControlsPanel(Point viewportSize)
    {
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

    private void DrawReplayPanel(GameplayPauseMenu model, Point viewportSize)
    {
        var panelBounds = GameplayPauseMenuLayout.GetMenuPanelBounds(viewportSize);
        var titlePosition = GameplayPauseMenuLayout.GetMenuTitlePosition(panelBounds);
        var itemsStartPosition = GameplayPauseMenuLayout.GetMenuItemsStartPosition(panelBounds);

        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, panelBounds, Color.Black * 0.88f);
        _renderContext.SpriteBatch.DrawString(_renderContext.Assets.DebugFont!, "Replay", titlePosition, Color.White);

        for (var index = 0; index < model.ReplayItems.Count; index++)
        {
            _renderContext.SpriteBatch.DrawString(
                _renderContext.Assets.DebugFont!,
                model.ReplayItems[index].Text,
                VerticalMenuLayout.GetItemPosition(itemsStartPosition, ItemSpacing, index),
                VerticalMenuLayout.GetItemColor(index, model.ReplaySelectedIndex, model.ReplayItems[index].IsEnabled));
        }

        DrawFooter(model.FooterText, panelBounds);
    }

    private void DrawMapPanel(GameplayPauseMenu model, Rectangle viewportBounds, Point viewportSize)
    {
        var font = _renderContext.Assets.DebugFont!;
        var modalBounds = GameplayPauseMenuLayout.GetMapModalBounds(viewportSize);
        var mapContentBounds = GameplayPauseMenuLayout.GetMapContentBounds(modalBounds);

        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, viewportBounds, OverlayColor);
        DrawPanel(modalBounds, ModalFillColor, ModalBorderColor);

        _renderContext.SpriteBatch.DrawString(
            font,
            "Map",
            new Vector2(modalBounds.X + 32, modalBounds.Y + 24),
            HeaderColor);

        _renderContext.SpriteBatch.DrawString(
            font,
            "The town sits at the center of the surrounding wilderness ring.",
            new Vector2(modalBounds.X + 32, modalBounds.Y + 58),
            new Color(82, 77, 72));

        DrawPanel(mapContentBounds, MapFrameFillColor, MapFrameBorderColor);
        DrawOverworldMap(model.MapSnapshot, font, mapContentBounds);

        _renderContext.SpriteBatch.DrawString(
            font,
            "Select / Tab / M / Esc to close",
            new Vector2(modalBounds.X + 32, modalBounds.Bottom - 34),
            new Color(82, 77, 72));
    }

    private void DrawInventoryPanel(GameplayPauseMenu model, Rectangle viewportBounds, Point viewportSize)
    {
        var font = _renderContext.Assets.DebugFont!;
        var modalBounds = GameplayPauseMenuLayout.GetInventoryModalBounds(viewportSize);
        var tabBounds = GameplayPauseMenuLayout.GetInventoryTabBounds(modalBounds);
        var contentBounds = GameplayPauseMenuLayout.GetInventoryContentBounds(modalBounds);

        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, viewportBounds, OverlayColor);
        DrawPanel(modalBounds, ModalFillColor, ModalBorderColor);

        _renderContext.SpriteBatch.DrawString(
            font,
            "Inventory",
            new Vector2(modalBounds.X + 32, modalBounds.Y + 28),
            HeaderColor);

        _renderContext.SpriteBatch.DrawString(
            font,
            "Browse your current equipment and progression tabs.",
            new Vector2(modalBounds.X + 32, modalBounds.Y + 62),
            new Color(82, 77, 72));

        DrawInventoryTabs(font, model.InventoryTab, tabBounds);
        DrawInventoryBody(font, model.InventoryTab, contentBounds);

        _renderContext.SpriteBatch.DrawString(
            font,
            "LB / RB or Q / R to switch tabs",
            new Vector2(modalBounds.X + 32, modalBounds.Bottom - 56),
            new Color(82, 77, 72));

        _renderContext.SpriteBatch.DrawString(
            font,
            "B / Esc to return",
            new Vector2(modalBounds.X + 32, modalBounds.Bottom - 30),
            new Color(82, 77, 72));
    }

    private void DrawFooter(string text, Rectangle panelBounds)
    {
        var font = _renderContext.Assets.DebugFont!;
        var lines = WrappedTextLayout.WrapText(font, text, GameplayPauseMenuLayout.GetFooterWidth(panelBounds));
        var footerPosition = GameplayPauseMenuLayout.GetFooterPosition(panelBounds);

        for (var index = 0; index < lines.Count; index++)
        {
            _renderContext.SpriteBatch.DrawString(
                font,
                lines[index],
                new Vector2(footerPosition.X, footerPosition.Y + (index * FooterLineSpacing)),
                new Color(154, 178, 171));
        }
    }

    private void DrawInventoryTabs(Microsoft.Xna.Framework.Graphics.SpriteFont font, PlayerInventoryTab activeTab, Rectangle tabBounds)
    {
        var tabWidth = tabBounds.Width / 4;
        DrawTab(font, new Rectangle(tabBounds.X, tabBounds.Y, tabWidth, tabBounds.Height), "Weapons", activeTab == PlayerInventoryTab.Weapons);
        DrawTab(font, new Rectangle(tabBounds.X + tabWidth, tabBounds.Y, tabWidth, tabBounds.Height), "Armor", activeTab == PlayerInventoryTab.Armor);
        DrawTab(font, new Rectangle(tabBounds.X + (tabWidth * 2), tabBounds.Y, tabWidth, tabBounds.Height), "Items", activeTab == PlayerInventoryTab.Items);
        DrawTab(font, new Rectangle(tabBounds.X + (tabWidth * 3), tabBounds.Y, tabWidth, tabBounds.Height), "Abilities", activeTab == PlayerInventoryTab.Abilities);
    }

    private void DrawInventoryBody(Microsoft.Xna.Framework.Graphics.SpriteFont font, PlayerInventoryTab activeTab, Rectangle contentBounds)
    {
        DrawPanel(contentBounds, new Color(255, 252, 246), new Color(57, 53, 51));

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
            new Vector2(contentBounds.X + 18, contentBounds.Y + 18),
            HeaderColor);

        for (var index = 0; index < bodyLines.Length; index++)
        {
            _renderContext.SpriteBatch.DrawString(
                font,
                bodyLines[index],
                new Vector2(contentBounds.X + 18, contentBounds.Y + 56 + (index * 28)),
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

    private void DrawOverworldMap(MyGame.Gameplay.World.OverworldMapSnapshot snapshot, Microsoft.Xna.Framework.Graphics.SpriteFont font, Rectangle mapContentBounds)
    {
        foreach (var region in snapshot.Regions)
        {
            var bounds = ProjectMapBounds(region.Bounds, snapshot.MapBounds, mapContentBounds);
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

        var markerPosition = ProjectMapPoint(snapshot.PlayerMapPosition, snapshot.MapBounds, mapContentBounds);
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
