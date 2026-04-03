using Microsoft.Xna.Framework;
using MyGame.Rendering.Menus;

namespace MyGame.Tests.Rendering.Menus;

public sealed class ControlsOverlayLayoutTests
{
    [Fact]
    public void GetPanelBounds_CentersPanelInViewport()
    {
        var bounds = ControlsOverlayLayout.GetPanelBounds(new Point(800, 480));

        Assert.Equal(new Rectangle(170, 106, 460, 268), bounds);
    }

    [Fact]
    public void GetHintLineTwoPosition_StaysInsidePanel()
    {
        var panel = ControlsOverlayLayout.GetPanelBounds(new Point(800, 480));
        var hintPosition = ControlsOverlayLayout.GetHintLineTwoPosition(new Point(800, 480));

        Assert.True(hintPosition.X > panel.X);
        Assert.True(hintPosition.X < panel.Right);
        Assert.True(hintPosition.Y > panel.Y);
        Assert.True(hintPosition.Y < panel.Bottom);
    }

    [Fact]
    public void GetHintLineOnePosition_LeavesBreathingRoomAfterControlsList()
    {
        var linesStart = ControlsOverlayLayout.GetLinesStartPosition(new Point(800, 480));
        var hintStart = ControlsOverlayLayout.GetHintLineOnePosition(new Point(800, 480));

        Assert.True(hintStart.Y - linesStart.Y >= 130f);
    }
}
