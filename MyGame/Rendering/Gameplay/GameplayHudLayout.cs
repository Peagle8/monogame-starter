using Microsoft.Xna.Framework;

namespace MyGame.Rendering.Gameplay;

public static class GameplayHudLayout
{
    private const int HealthPanelMargin = 12;
    private const int HealthPanelWidth = 208;
    private const int HealthPanelHeight = 62;
    private const int DebugOverlayGap = 12;
    private const int DeathPanelWidth = 420;
    private const int DeathPanelHeight = 132;

    public static Rectangle GetHealthPanelBounds()
    {
        return new Rectangle(HealthPanelMargin, HealthPanelMargin, HealthPanelWidth, HealthPanelHeight);
    }

    public static Vector2 GetHealthTextPosition()
    {
        var panelBounds = GetHealthPanelBounds();
        return new Vector2(panelBounds.X + 12f, panelBounds.Y + 8f);
    }

    public static Rectangle GetHealthBarBounds()
    {
        var panelBounds = GetHealthPanelBounds();
        return new Rectangle(panelBounds.X + 44, panelBounds.Y + 12, 148, 10);
    }

    public static Vector2 GetAbilityPointTextPosition()
    {
        var panelBounds = GetHealthPanelBounds();
        return new Vector2(panelBounds.X + 12f, panelBounds.Y + 34f);
    }

    public static Rectangle GetAbilityPointBarBounds()
    {
        var panelBounds = GetHealthPanelBounds();
        return new Rectangle(panelBounds.X + 44, panelBounds.Y + 38, 148, 10);
    }

    public static Rectangle GetDeathPanelBounds(Point viewportSize)
    {
        return new Rectangle(
            (viewportSize.X - DeathPanelWidth) / 2,
            (viewportSize.Y - DeathPanelHeight) / 2,
            DeathPanelWidth,
            DeathPanelHeight);
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

    public static Vector2 GetScreenBannerPosition(Point viewportSize, Vector2 textSize)
    {
        return new Vector2(
            (viewportSize.X - textSize.X) * 0.5f,
            18f);
    }
}
