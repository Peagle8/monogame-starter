using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    private const float InventoryLineSpacing = 24f;
    private const int UpgradeNodesPerPage = 4;
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
    private static readonly Color SummaryFillColor = new(246, 243, 235);
    private static readonly Color SummaryAccentColor = new(77, 103, 129);
    private static readonly Color DisabledTextColor = new(142, 135, 126);
    private static readonly Color EquippedTextColor = new(39, 113, 79);
    private static readonly Color TreeLineColor = new(108, 117, 127);
    private static readonly Color TreeNodeFillColor = new(229, 221, 207);
    private static readonly Color TreeNodeBorderColor = new(70, 72, 78);

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
        DrawInventoryBody(font, model, contentBounds);

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

    private void DrawInventoryTabs(SpriteFont font, PlayerInventoryTab activeTab, Rectangle tabBounds)
    {
        var tabWidth = tabBounds.Width / 4;
        DrawTab(font, new Rectangle(tabBounds.X, tabBounds.Y, tabWidth, tabBounds.Height), "Weapons", activeTab == PlayerInventoryTab.Weapons);
        DrawTab(font, new Rectangle(tabBounds.X + tabWidth, tabBounds.Y, tabWidth, tabBounds.Height), "Armor", activeTab == PlayerInventoryTab.Armor);
        DrawTab(font, new Rectangle(tabBounds.X + (tabWidth * 2), tabBounds.Y, tabWidth, tabBounds.Height), "Items", activeTab == PlayerInventoryTab.Items);
        DrawTab(font, new Rectangle(tabBounds.X + (tabWidth * 3), tabBounds.Y, tabWidth, tabBounds.Height), AbilityLoadoutCatalog.GetTabLabel(), activeTab == PlayerInventoryTab.Abilities);
    }

    private void DrawInventoryBody(SpriteFont font, GameplayPauseMenu model, Rectangle contentBounds)
    {
        var activeTab = model.InventoryTab;
        DrawPanel(contentBounds, new Color(255, 252, 246), new Color(57, 53, 51));
        if (activeTab == PlayerInventoryTab.Abilities)
        {
            DrawAbilitiesBody(font, model, contentBounds);
            return;
        }

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

    private void DrawAbilitiesBody(SpriteFont font, GameplayPauseMenu model, Rectangle contentBounds)
    {
        var summaryBounds = new Rectangle(contentBounds.X + 16, contentBounds.Y + 16, contentBounds.Width - 32, 122);
        var lowerTop = summaryBounds.Bottom + 16;
        var lowerHeight = contentBounds.Bottom - lowerTop - 16;
        var leftColumnWidth = Math.Min(244, contentBounds.Width / 3);
        var leftColumnBounds = new Rectangle(contentBounds.X + 16, lowerTop, leftColumnWidth, lowerHeight);
        var detailBounds = new Rectangle(leftColumnBounds.Right + 16, lowerTop, contentBounds.Right - leftColumnBounds.Right - 32, lowerHeight);

        DrawAbilitySummary(font, model.AbilitySummary, summaryBounds);
        DrawAbilityNavigation(font, model, leftColumnBounds);
        DrawAbilityDetails(font, model, detailBounds);
    }

    private void DrawAbilitySummary(SpriteFont font, IReadOnlyList<AbilitySummaryEntry> summary, Rectangle bounds)
    {
        DrawPanel(bounds, SummaryFillColor, ModalBorderColor);
        _renderContext.SpriteBatch.DrawString(font, "Current Loadout", new Vector2(bounds.X + 16, bounds.Y + 14), HeaderColor);

        var columnWidth = bounds.Width / 4;
        for (var index = 0; index < summary.Count; index++)
        {
            var entryBounds = new Rectangle(bounds.X + (columnWidth * index), bounds.Y + 40, columnWidth, bounds.Height - 48);
            _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, new Rectangle(entryBounds.X, entryBounds.Y, 2, entryBounds.Height), SummaryAccentColor);
            _renderContext.SpriteBatch.DrawString(font, summary[index].SlotLabel, new Vector2(entryBounds.X + 12, entryBounds.Y + 4), SummaryAccentColor);
            DrawWrappedText(
                font,
                summary[index].EquippedAbilityName,
                new Vector2(entryBounds.X + 12, entryBounds.Y + 30),
                entryBounds.Width - 24,
                HeaderColor,
                InventoryLineSpacing);
        }
    }

    private void DrawAbilityNavigation(SpriteFont font, GameplayPauseMenu model, Rectangle bounds)
    {
        var slotsHeight = 194;
        var actionsTop = bounds.Y + slotsHeight + 12;
        var actionsHeight = Math.Max(104, bounds.Bottom - actionsTop);
        var slotsBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, slotsHeight);
        var actionsBounds = new Rectangle(bounds.X, actionsTop, bounds.Width, actionsHeight);

        DrawPanel(slotsBounds, new Color(250, 247, 238), ModalBorderColor);
        _renderContext.SpriteBatch.DrawString(font, "Slots", new Vector2(slotsBounds.X + 16, slotsBounds.Y + 14), HeaderColor);

        var startY = slotsBounds.Y + 48;
        for (var index = 0; index < AbilityLoadoutCatalog.OrderedSlots.Count; index++)
        {
            var slot = AbilityLoadoutCatalog.OrderedSlots[index];
            var isSelected = model.SelectedAbilitySlotIndex == index;
            var color = model.AbilityMenuView == AbilityMenuView.SlotList && isSelected ? ActiveTabFillColor : HeaderColor;
            var prefix = isSelected ? "> " : "  ";
            _renderContext.SpriteBatch.DrawString(
                font,
                prefix + AbilityLoadoutCatalog.GetSlotLabel(slot),
                new Vector2(slotsBounds.X + 18, startY + (index * 28)),
                color);
        }

        DrawPanel(actionsBounds, new Color(250, 247, 238), ModalBorderColor);
        _renderContext.SpriteBatch.DrawString(font, "Actions", new Vector2(actionsBounds.X + 16, actionsBounds.Y + 14), HeaderColor);
        for (var index = 0; index < AbilityLoadoutCatalog.MenuActions.Count; index++)
        {
            var isSelected = model.AbilityMenuView == AbilityMenuView.ActionList && model.SelectedAbilityActionIndex == index;
            var prefix = isSelected ? "> " : "  ";
            var color = isSelected ? ActiveTabFillColor : new Color(77, 73, 68);
            _renderContext.SpriteBatch.DrawString(
                font,
                prefix + AbilityLoadoutCatalog.GetActionLabel(AbilityLoadoutCatalog.MenuActions[index]),
                new Vector2(actionsBounds.X + 18, actionsBounds.Y + 48 + (index * 28)),
                color);
        }
    }

    private void DrawAbilityDetails(SpriteFont font, GameplayPauseMenu model, Rectangle bounds)
    {
        DrawPanel(bounds, new Color(255, 252, 246), ModalBorderColor);
        var slotLabel = AbilityLoadoutCatalog.GetSlotLabel(model.SelectedAbilitySlot);
        _renderContext.SpriteBatch.DrawString(font, slotLabel, new Vector2(bounds.X + 18, bounds.Y + 14), HeaderColor);
        _renderContext.SpriteBatch.DrawString(font, $"Equipped: {AbilityLoadoutCatalog.GetEquippedAbilityName(model.Player, model.SelectedAbilitySlot)}", new Vector2(bounds.X + 18, bounds.Y + 44), SummaryAccentColor);
        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, new Rectangle(bounds.X + 18, bounds.Y + 76, bounds.Width - 36, 2), new Color(211, 203, 191));

        switch (model.AbilityMenuView)
        {
            case AbilityMenuView.UpgradeView:
                DrawUpgradeTree(font, model, bounds);
                return;
            case AbilityMenuView.EquipList:
                DrawAbilityOptionList(font, model.AbilityOptions, model.SelectedAbilityOptionIndex, bounds);
                return;
            default:
                DrawAbilitySlotOverview(font, model, bounds);
                return;
        }
    }

    private void DrawAbilitySlotOverview(SpriteFont font, GameplayPauseMenu model, Rectangle bounds)
    {
        var descriptionPosition = new Vector2(bounds.X + 18, bounds.Y + 88);
        var descriptionLines = WrappedTextLayout.WrapText(
            font,
            "Choose a slot, then equip an implemented ability or preview the planned upgrade branch.",
            bounds.Width - 36);
        DrawWrappedText(
            font,
            descriptionLines,
            descriptionPosition,
            new Color(68, 66, 62),
            InventoryLineSpacing);

        var listStartY = descriptionPosition.Y + (descriptionLines.Count * InventoryLineSpacing) + 18f;
        var entries = AbilityLoadoutCatalog.GetEntries(model.SelectedAbilitySlot);
        var visibleWindow = CalculateVisibleWindow(
            entries.Count,
            selectedIndex: 0,
            GetVisibleRowCount(bounds.Bottom - listStartY - 18f, 28f));
        var currentY = listStartY;

        if (visibleWindow.HasHiddenRowsAbove)
        {
            _renderContext.SpriteBatch.DrawString(font, "...", new Vector2(bounds.X + 18, currentY), DisabledTextColor);
            currentY += 28f;
        }

        for (var index = visibleWindow.StartIndex; index < visibleWindow.EndIndexExclusive; index++)
        {
            var textColor = entries[index].IsImplemented ? HeaderColor : DisabledTextColor;
            _renderContext.SpriteBatch.DrawString(
                font,
                entries[index].DisplayName,
                new Vector2(bounds.X + 18, currentY),
                textColor);
            currentY += 28f;
        }

        if (visibleWindow.HasHiddenRowsBelow)
        {
            _renderContext.SpriteBatch.DrawString(font, "...", new Vector2(bounds.X + 18, currentY), DisabledTextColor);
        }
    }

    private void DrawAbilityOptionList(SpriteFont font, IReadOnlyList<AbilityMenuOptionViewModel> options, int selectedIndex, Rectangle bounds)
    {
        _renderContext.SpriteBatch.DrawString(font, "Equip", new Vector2(bounds.X + 18, bounds.Y + 88), SummaryAccentColor);
        var listStartY = bounds.Y + 122f;
        var visibleWindow = CalculateVisibleWindow(
            options.Count,
            selectedIndex,
            GetVisibleRowCount(bounds.Bottom - listStartY - 18f, 28f));
        var currentY = listStartY;
        DrawTopRightTag(font, bounds, GetListPageLabel(visibleWindow, options.Count), new Color(108, 117, 127));

        if (visibleWindow.HasHiddenRowsAbove)
        {
            _renderContext.SpriteBatch.DrawString(font, "...", new Vector2(bounds.X + 18, currentY), DisabledTextColor);
            currentY += 28f;
        }

        for (var index = visibleWindow.StartIndex; index < visibleWindow.EndIndexExclusive; index++)
        {
            var color = ResolveAbilityOptionColor(options[index], index == selectedIndex);
            var prefix = index == selectedIndex ? "> " : "  ";
            var suffix = options[index].IsEquipped ? " [Equipped]" : options[index].IsEnabled ? string.Empty : " [Placeholder]";
            _renderContext.SpriteBatch.DrawString(
                font,
                prefix + options[index].DisplayName + suffix,
                new Vector2(bounds.X + 18, currentY),
                color);
            currentY += 28f;
        }

        if (visibleWindow.HasHiddenRowsBelow)
        {
            _renderContext.SpriteBatch.DrawString(font, "...", new Vector2(bounds.X + 18, currentY), DisabledTextColor);
        }
    }

    private void DrawUpgradeTree(SpriteFont font, GameplayPauseMenu model, Rectangle bounds)
    {
        var graphBounds = new Rectangle(bounds.X + 18, bounds.Y + 96, bounds.Width - 36, bounds.Height - 114);
        DrawPanel(graphBounds, new Color(248, 244, 236), new Color(197, 188, 175));
        DrawTopRightTag(font, graphBounds, $"Page {model.UpgradePageIndex + 1}/{model.UpgradePageCount}", new Color(108, 117, 127));

        var entries = AbilityLoadoutCatalog.GetEntries(model.SelectedAbilitySlot);
        var startIndex = model.UpgradePageIndex * UpgradeNodesPerPage;
        var pageEntries = entries.Skip(startIndex).Take(UpgradeNodesPerPage).ToArray();

        var rootBounds = new Rectangle(graphBounds.X + 28, graphBounds.Y + (graphBounds.Height / 2) - 20, 150, 40);
        var nodeWidth = 168;
        var nodeHeight = 42;
        var columns = 2;
        var rows = 2;
        var gridLeft = rootBounds.Right + 80;
        var gridRight = graphBounds.Right - 28;
        var gridTop = graphBounds.Y + 44;
        var gridBottom = graphBounds.Bottom - 26;
        var horizontalGap = columns > 1
            ? (gridRight - gridLeft - (columns * nodeWidth)) / (columns - 1)
            : 0;
        var verticalGap = rows > 1
            ? (gridBottom - gridTop - (rows * nodeHeight)) / (rows - 1)
            : 0;
        var nodeBoundsByIndex = new Rectangle[pageEntries.Length];

        for (var index = 0; index < pageEntries.Length; index++)
        {
            var row = index / columns;
            var column = index % columns;
            nodeBoundsByIndex[index] = new Rectangle(
                gridLeft + (column * (nodeWidth + horizontalGap)),
                gridTop + (row * (nodeHeight + verticalGap)),
                nodeWidth,
                nodeHeight);
        }

        for (var index = 0; index < pageEntries.Length; index++)
        {
            DrawTreeConnection(GetNodeAnchor(rootBounds, true), GetNodeAnchor(nodeBoundsByIndex[index], false));
        }

        DrawTreeNode(font, rootBounds, AbilityLoadoutCatalog.GetSlotLabel(model.SelectedAbilitySlot), SummaryAccentColor);

        for (var index = 0; index < pageEntries.Length; index++)
        {
            DrawTreeNode(
                font,
                nodeBoundsByIndex[index],
                pageEntries[index].DisplayName,
                pageEntries[index].IsImplemented ? HeaderColor : DisabledTextColor);
        }
    }

    private void DrawTreeConnection(Vector2 start, Vector2 end)
    {
        var midX = start.X + ((end.X - start.X) * 0.45f);
        DrawLine(new Vector2(start.X, start.Y), new Vector2(midX, start.Y));
        DrawLine(new Vector2(midX, start.Y), new Vector2(midX, end.Y));
        DrawLine(new Vector2(midX, end.Y), new Vector2(end.X, end.Y));
    }

    private void DrawTreeNode(SpriteFont font, Rectangle bounds, string label, Color textColor)
    {
        DrawPanel(bounds, TreeNodeFillColor, TreeNodeBorderColor);
        DrawCenteredText(font, label, bounds, textColor);
    }

    private static Rectangle CreateNodeBounds(Vector2 center, int width, int height)
    {
        return new Rectangle((int)center.X - (width / 2), (int)center.Y - (height / 2), width, height);
    }

    private Vector2 GetNodeAnchor(Rectangle bounds, bool useRightEdge)
    {
        return useRightEdge
            ? new Vector2(bounds.Right, bounds.Center.Y)
            : new Vector2(bounds.Left, bounds.Center.Y);
    }

    private static Color ResolveAbilityOptionColor(AbilityMenuOptionViewModel option, bool isSelected)
    {
        if (!option.IsEnabled)
        {
            return DisabledTextColor;
        }

        if (option.IsEquipped)
        {
            return EquippedTextColor;
        }

        return isSelected ? ActiveTabFillColor : HeaderColor;
    }

    private void DrawTab(SpriteFont font, Rectangle bounds, string label, bool isActive)
    {
        DrawPanel(bounds, isActive ? ActiveTabFillColor : InactiveTabFillColor, ModalBorderColor);
        DrawWrappedCenteredText(font, label, bounds, isActive ? ActiveTabTextColor : InactiveTabTextColor, 20f);
    }

    private void DrawCenteredText(SpriteFont font, string text, Rectangle bounds, Color color)
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

    private void DrawWrappedCenteredText(SpriteFont font, string text, Rectangle bounds, Color color, float lineSpacing)
    {
        var lines = WrappedTextLayout.WrapText(font, text, bounds.Width - 16);
        var totalHeight = Math.Max(font.LineSpacing, lines.Count * lineSpacing);
        var startY = bounds.Y + ((bounds.Height - totalHeight) * 0.5f);

        for (var index = 0; index < lines.Count; index++)
        {
            var lineSize = font.MeasureString(lines[index]);
            var position = new Vector2(
                bounds.X + ((bounds.Width - lineSize.X) * 0.5f),
                startY + (index * lineSpacing));
            _renderContext.SpriteBatch.DrawString(font, lines[index], position, color);
        }
    }

    private void DrawWrappedText(
        SpriteFont font,
        string text,
        Vector2 position,
        float maxWidth,
        Color color,
        float lineSpacing)
    {
        var lines = WrappedTextLayout.WrapText(font, text, maxWidth);
        DrawWrappedText(font, lines, position, color, lineSpacing);
    }

    private void DrawWrappedText(
        SpriteFont font,
        IReadOnlyList<string> lines,
        Vector2 position,
        Color color,
        float lineSpacing)
    {
        for (var index = 0; index < lines.Count; index++)
        {
            _renderContext.SpriteBatch.DrawString(
                font,
                lines[index],
                new Vector2(position.X, position.Y + (index * lineSpacing)),
                color);
        }
    }

    private void DrawTopRightTag(SpriteFont font, Rectangle bounds, string text, Color color)
    {
        var textSize = font.MeasureString(text);
        var position = new Vector2(bounds.Right - textSize.X - 14, bounds.Y + 14);
        _renderContext.SpriteBatch.DrawString(font, text, position, color);
    }

    private string GetListPageLabel(VisibleWindow visibleWindow, int totalCount)
    {
        if (totalCount <= 0)
        {
            return "0/0";
        }

        var visibleCount = Math.Max(1, visibleWindow.EndIndexExclusive - visibleWindow.StartIndex);
        var currentPage = (visibleWindow.StartIndex / visibleCount) + 1;
        var totalPages = (int)Math.Ceiling(totalCount / (double)visibleCount);
        return $"{currentPage}/{totalPages}";
    }

    private void DrawLine(Vector2 start, Vector2 end)
    {
        if (Math.Abs(start.X - end.X) >= Math.Abs(start.Y - end.Y))
        {
            var x = (int)MathF.Round(Math.Min(start.X, end.X));
            var y = (int)MathF.Round(start.Y);
            var width = Math.Max(2, (int)MathF.Round(Math.Abs(end.X - start.X)));
            _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, new Rectangle(x, y, width, 2), TreeLineColor);
            return;
        }

        var verticalX = (int)MathF.Round(start.X);
        var verticalY = (int)MathF.Round(Math.Min(start.Y, end.Y));
        var height = Math.Max(2, (int)MathF.Round(Math.Abs(end.Y - start.Y)));
        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, new Rectangle(verticalX, verticalY, 2, height), TreeLineColor);
    }

    private static int GetVisibleRowCount(float availableHeight, float rowHeight)
    {
        return Math.Max(1, (int)MathF.Floor(availableHeight / rowHeight));
    }

    private static VisibleWindow CalculateVisibleWindow(int totalCount, int selectedIndex, int maxVisibleRows)
    {
        if (totalCount <= 0)
        {
            return new VisibleWindow(0, 0, false, false);
        }

        var visibleOptionRows = Math.Min(totalCount, Math.Max(1, maxVisibleRows));
        var startIndex = 0;
        var hasHiddenRowsAbove = false;
        var hasHiddenRowsBelow = false;

        for (var iteration = 0; iteration < 3; iteration++)
        {
            var maxStartIndex = Math.Max(0, totalCount - visibleOptionRows);
            startIndex = Math.Clamp(selectedIndex - (visibleOptionRows / 2), 0, maxStartIndex);
            hasHiddenRowsAbove = startIndex > 0;
            hasHiddenRowsBelow = startIndex + visibleOptionRows < totalCount;
            var reservedIndicatorRows = (hasHiddenRowsAbove ? 1 : 0) + (hasHiddenRowsBelow ? 1 : 0);
            visibleOptionRows = Math.Max(1, Math.Min(totalCount, maxVisibleRows - reservedIndicatorRows));
        }

        return new VisibleWindow(
            startIndex,
            Math.Min(totalCount, startIndex + visibleOptionRows),
            hasHiddenRowsAbove,
            hasHiddenRowsBelow);
    }

    private readonly record struct VisibleWindow(
        int StartIndex,
        int EndIndexExclusive,
        bool HasHiddenRowsAbove,
        bool HasHiddenRowsBelow);

    private void DrawOverworldMap(MyGame.Gameplay.World.OverworldMapSnapshot snapshot, SpriteFont font, Rectangle mapContentBounds)
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
