using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Core.Rendering;
using MyGame.Scenes.Gameplay;

namespace MyGame.Rendering.Gameplay;

public sealed class GameplayOverlayRenderer : IRenderer<GameplayScene>
{
    private static readonly Color PanelFillColor = new(9, 17, 24, 220);
    private static readonly Color PanelBorderColor = new(82, 121, 111);
    private static readonly Color HealthPipColor = new(211, 78, 68);
    private static readonly Color MissingHealthPipColor = new(73, 50, 52);
    private static readonly Color DeathPanelFillColor = new(36, 12, 16, 235);
    private static readonly Color DeathPanelBorderColor = new(190, 92, 82);
    private static readonly Color RecordingIndicatorColor = new(220, 82, 82);
    private static readonly Color ReplayIndicatorColor = new(106, 208, 126);
    private static readonly Color IndicatorPanelFillColor = new(9, 17, 24, 210);
    private static readonly Color IndicatorPanelBorderColor = new(70, 88, 96);
    private static readonly Point IndicatorPadding = new(10, 6);
    private static readonly Point IndicatorMargin = new(20, 20);

    private readonly IRenderContext _renderContext;
    private readonly IRenderer<GameplayPauseMenu> _pauseMenuRenderer;

    public GameplayOverlayRenderer(IRenderContext renderContext, IRenderer<GameplayPauseMenu> pauseMenuRenderer)
    {
        _renderContext = renderContext;
        _pauseMenuRenderer = pauseMenuRenderer;
    }

    public void Draw(GameplayScene model, FrameTime frameTime)
    {
        var viewport = _renderContext.SpriteBatch.GraphicsDevice.Viewport;
        var viewportSize = new Point(viewport.Width, viewport.Height);

        DrawHealthHud(model);

        if (_renderContext.Assets.DebugFont is not null)
        {
            DrawHudText(model);
            DrawDiagnosticsIndicator(model, viewportSize);

            if (model.IsPlayerDead)
            {
                DrawDeathPanel(viewportSize);
            }
        }

        _pauseMenuRenderer.Draw(model.PauseMenu, frameTime);
    }

    private void DrawHealthHud(GameplayScene model)
    {
        var panelBounds = GameplayHudLayout.GetHealthPanelBounds();
        DrawPanel(panelBounds, PanelFillColor, PanelBorderColor);

        for (var index = 0; index < model.World.Player.MaxHealth; index++)
        {
            var color = index < model.World.Player.CurrentHealth
                ? HealthPipColor
                : MissingHealthPipColor;

            _renderContext.SpriteBatch.Draw(
                _renderContext.Assets.Pixel,
                GameplayHudLayout.GetHealthPipBounds(index),
                color);
        }
    }

    private void DrawHudText(GameplayScene model)
    {
        var font = _renderContext.Assets.DebugFont;
        if (font is null)
        {
            return;
        }

        _renderContext.SpriteBatch.DrawString(
            font,
            $"Health: {model.World.Player.CurrentHealth}/{model.World.Player.MaxHealth}",
            GameplayHudLayout.GetHealthTextPosition(),
            Color.White);

        _renderContext.SpriteBatch.DrawString(
            font,
            $"Crabs: {model.World.DefeatedEnemyCount}",
            GameplayHudLayout.GetKillCountPosition(),
            new Color(255, 220, 196));
    }

    private void DrawDeathPanel(Point viewportSize)
    {
        var font = _renderContext.Assets.DebugFont;
        if (font is null)
        {
            return;
        }

        DrawPanel(
            GameplayHudLayout.GetDeathPanelBounds(viewportSize),
            DeathPanelFillColor,
            DeathPanelBorderColor);

        _renderContext.SpriteBatch.DrawString(
            font,
            "You got pinched.",
            GameplayHudLayout.GetDeathTitlePosition(viewportSize),
            new Color(255, 220, 220));

        _renderContext.SpriteBatch.DrawString(
            font,
            "Press Enter to restart",
            GameplayHudLayout.GetDeathHintLineOnePosition(viewportSize),
            Color.White);

        _renderContext.SpriteBatch.DrawString(
            font,
            "Press Esc for menu.",
            GameplayHudLayout.GetDeathHintLineTwoPosition(viewportSize),
            Color.White);
    }

    private void DrawDiagnosticsIndicator(GameplayScene model, Point viewportSize)
    {
        var font = _renderContext.Assets.DebugFont;
        if (font is null)
        {
            return;
        }

        string? label = null;
        Color labelColor = Color.White;

        if (model.IsRecording)
        {
            label = "RECORDING";
            labelColor = RecordingIndicatorColor;
        }
        else if (model.IsReplayPaused)
        {
            label = "REPLAY PAUSED";
            labelColor = ReplayIndicatorColor;
        }
        else if (model.IsReplaying)
        {
            label = "REPLAY";
            labelColor = ReplayIndicatorColor;
        }

        if (label is null)
        {
            return;
        }

        var textSize = font.MeasureString(label);
        var bounds = new Rectangle(
            viewportSize.X - IndicatorMargin.X - (int)MathF.Ceiling(textSize.X) - (IndicatorPadding.X * 2),
            IndicatorMargin.Y,
            (int)MathF.Ceiling(textSize.X) + (IndicatorPadding.X * 2),
            (int)MathF.Ceiling(textSize.Y) + (IndicatorPadding.Y * 2));

        DrawPanel(bounds, IndicatorPanelFillColor, IndicatorPanelBorderColor);
        _renderContext.SpriteBatch.DrawString(
            font,
            label,
            new Vector2(bounds.X + IndicatorPadding.X, bounds.Y + IndicatorPadding.Y),
            labelColor);
    }

    private void DrawPanel(Rectangle bounds, Color fillColor, Color borderColor)
    {
        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, bounds, fillColor);
        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, new Rectangle(bounds.X, bounds.Y, bounds.Width, 2), borderColor);
        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, new Rectangle(bounds.X, bounds.Bottom - 2, bounds.Width, 2), borderColor);
        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, new Rectangle(bounds.X, bounds.Y, 2, bounds.Height), borderColor);
        _renderContext.SpriteBatch.Draw(_renderContext.Assets.Pixel, new Rectangle(bounds.Right - 2, bounds.Y, 2, bounds.Height), borderColor);
    }
}
