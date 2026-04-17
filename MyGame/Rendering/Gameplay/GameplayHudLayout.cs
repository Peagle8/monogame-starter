using Microsoft.Xna.Framework;

namespace MyGame.Rendering.Gameplay;

public static class GameplayHudLayout
{
    private const int HealthPanelMargin = 12;
    private const int DebugOverlayGap = 12;

    public static Rectangle GetHealthPanelBounds(Point viewportSize)
    {
        var width = viewportSize.X <= 800
            ? 208
            : Math.Clamp((int)MathF.Round(viewportSize.X * 0.14f), 208, 320);
        var height = viewportSize.X <= 800
            ? 62
            : Math.Clamp((int)MathF.Round(viewportSize.Y * 0.09f), 62, 92);

        return new Rectangle(HealthPanelMargin, HealthPanelMargin, width, height);
    }

    public static Vector2 GetHealthTextPosition(Point viewportSize)
    {
        var panelBounds = GetHealthPanelBounds(viewportSize);
        return new Vector2(panelBounds.X + 12f, panelBounds.Y + 8f);
    }

    public static Rectangle GetHealthBarBounds(Point viewportSize)
    {
        var panelBounds = GetHealthPanelBounds(viewportSize);
        var barWidth = Math.Max(148, panelBounds.Width - 60);
        return new Rectangle(panelBounds.X + 44, panelBounds.Y + 12, barWidth, 10);
    }

    public static Vector2 GetAbilityPointTextPosition(Point viewportSize)
    {
        var panelBounds = GetHealthPanelBounds(viewportSize);
        return new Vector2(panelBounds.X + 12f, panelBounds.Y + 34f);
    }

    public static Rectangle GetAbilityPointBarBounds(Point viewportSize)
    {
        var panelBounds = GetHealthPanelBounds(viewportSize);
        var barWidth = Math.Max(148, panelBounds.Width - 60);
        return new Rectangle(panelBounds.X + 44, panelBounds.Y + 38, barWidth, 10);
    }

    public static Rectangle GetDeathPanelBounds(Point viewportSize)
    {
        var width = Math.Clamp((int)MathF.Round(viewportSize.X * 0.32f), 420, 680);
        var height = Math.Clamp((int)MathF.Round(viewportSize.Y * 0.2f), 132, 220);
        return new Rectangle(
            (viewportSize.X - width) / 2,
            (viewportSize.Y - height) / 2,
            width,
            height);
    }

    public static Vector2 GetDeathTitlePosition(Point viewportSize)
    {
        var panelBounds = GetDeathPanelBounds(viewportSize);
        return new Vector2(panelBounds.X + (panelBounds.Width * 0.27f), panelBounds.Y + 24f);
    }

    public static Vector2 GetDeathHintLineOnePosition(Point viewportSize)
    {
        var panelBounds = GetDeathPanelBounds(viewportSize);
        return new Vector2(panelBounds.X + (panelBounds.Width * 0.2f), panelBounds.Y + (panelBounds.Height * 0.47f));
    }

    public static Vector2 GetDeathHintLineTwoPosition(Point viewportSize)
    {
        var panelBounds = GetDeathPanelBounds(viewportSize);
        return new Vector2(panelBounds.X + (panelBounds.Width * 0.29f), panelBounds.Y + (panelBounds.Height * 0.67f));
    }

    public static Vector2 GetDebugOverlayPosition(Point viewportSize)
    {
        var panelBounds = GetHealthPanelBounds(viewportSize);
        return new Vector2(panelBounds.X, panelBounds.Bottom + DebugOverlayGap);
    }

    public static Vector2 GetScreenBannerPosition(Point viewportSize, Vector2 textSize)
    {
        return new Vector2(
            (viewportSize.X - textSize.X) * 0.5f,
            18f);
    }
}
