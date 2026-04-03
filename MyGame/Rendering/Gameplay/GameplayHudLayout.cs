using Microsoft.Xna.Framework;

namespace MyGame.Rendering.Gameplay;

public static class GameplayHudLayout
{
    private const int HealthPanelMargin = 12;
    private const int HealthPanelWidth = 160;
    private const int HealthPanelHeight = 68;
    private const int DebugOverlayGap = 12;
    private const int HealthPipSize = 16;
    private const int HealthPipSpacing = 6;
    private const int HealthPipStartX = 12;
    private const int HealthPipY = 42;
    private const int DeathPanelWidth = 420;
    private const int DeathPanelHeight = 132;

    public static Rectangle GetHealthPanelBounds()
    {
        return new Rectangle(HealthPanelMargin, HealthPanelMargin, HealthPanelWidth, HealthPanelHeight);
    }

    public static Rectangle GetHealthPipBounds(int index)
    {
        var panelBounds = GetHealthPanelBounds();
        return new Rectangle(
            panelBounds.X + HealthPipStartX + (index * (HealthPipSize + HealthPipSpacing)),
            panelBounds.Y + HealthPipY,
            HealthPipSize,
            HealthPipSize);
    }

    public static Rectangle GetDeathPanelBounds(Point viewportSize)
    {
        return new Rectangle(
            (viewportSize.X - DeathPanelWidth) / 2,
            (viewportSize.Y - DeathPanelHeight) / 2,
            DeathPanelWidth,
            DeathPanelHeight);
    }

    public static Vector2 GetHealthTextPosition()
    {
        var panelBounds = GetHealthPanelBounds();
        return new Vector2(panelBounds.X + 12f, panelBounds.Y + 2f);
    }

    public static Vector2 GetKillCountPosition()
    {
        var panelBounds = GetHealthPanelBounds();
        return new Vector2(panelBounds.X + 12f, panelBounds.Y + 22f);
    }

    public static Vector2 GetDeathTitlePosition(Point viewportSize)
    {
        var panelBounds = GetDeathPanelBounds(viewportSize);
        return new Vector2(panelBounds.X + 112f, panelBounds.Y + 24f);
    }

    public static Vector2 GetDeathHintLineOnePosition(Point viewportSize)
    {
        var panelBounds = GetDeathPanelBounds(viewportSize);
        return new Vector2(panelBounds.X + 84f, panelBounds.Y + 62f);
    }

    public static Vector2 GetDeathHintLineTwoPosition(Point viewportSize)
    {
        var panelBounds = GetDeathPanelBounds(viewportSize);
        return new Vector2(panelBounds.X + 122f, panelBounds.Y + 88f);
    }

    public static Vector2 GetDebugOverlayPosition()
    {
        var panelBounds = GetHealthPanelBounds();
        return new Vector2(
            panelBounds.X,
            panelBounds.Bottom + DebugOverlayGap);
    }
}
