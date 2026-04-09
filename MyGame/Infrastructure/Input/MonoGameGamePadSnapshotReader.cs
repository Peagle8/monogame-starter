using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MyGame.Core.Input;

namespace MyGame.Infrastructure.Input;

public sealed class MonoGameGamePadSnapshotReader
{
    private const float StickThreshold = 0.4f;

    public GamePadSnapshot ReadCurrent()
    {
        var state = GamePad.GetState(PlayerIndex.One);
        if (!state.IsConnected)
        {
            return GamePadSnapshot.Empty;
        }

        var pressedControls = new HashSet<GamePadControl>();

        AddIfPressed(pressedControls, GamePadControl.DPadUp, state.DPad.Up == ButtonState.Pressed);
        AddIfPressed(pressedControls, GamePadControl.DPadDown, state.DPad.Down == ButtonState.Pressed);
        AddIfPressed(pressedControls, GamePadControl.DPadLeft, state.DPad.Left == ButtonState.Pressed);
        AddIfPressed(pressedControls, GamePadControl.DPadRight, state.DPad.Right == ButtonState.Pressed);

        AddIfPressed(pressedControls, GamePadControl.LeftStickUp, state.ThumbSticks.Left.Y >= StickThreshold);
        AddIfPressed(pressedControls, GamePadControl.LeftStickDown, state.ThumbSticks.Left.Y <= -StickThreshold);
        AddIfPressed(pressedControls, GamePadControl.LeftStickLeft, state.ThumbSticks.Left.X <= -StickThreshold);
        AddIfPressed(pressedControls, GamePadControl.LeftStickRight, state.ThumbSticks.Left.X >= StickThreshold);

        AddIfPressed(pressedControls, GamePadControl.FaceBottom, state.Buttons.A == ButtonState.Pressed);
        AddIfPressed(pressedControls, GamePadControl.FaceRight, state.Buttons.B == ButtonState.Pressed);
        AddIfPressed(pressedControls, GamePadControl.FaceLeft, state.Buttons.X == ButtonState.Pressed);
        AddIfPressed(pressedControls, GamePadControl.FaceTop, state.Buttons.Y == ButtonState.Pressed);
        AddIfPressed(pressedControls, GamePadControl.LeftTrigger, state.Triggers.Left >= StickThreshold);
        AddIfPressed(pressedControls, GamePadControl.RightTrigger, state.Triggers.Right >= StickThreshold);
        AddIfPressed(pressedControls, GamePadControl.LeftShoulder, state.Buttons.LeftShoulder == ButtonState.Pressed);
        AddIfPressed(pressedControls, GamePadControl.RightShoulder, state.Buttons.RightShoulder == ButtonState.Pressed);
        AddIfPressed(pressedControls, GamePadControl.Start, state.Buttons.Start == ButtonState.Pressed);
        AddIfPressed(pressedControls, GamePadControl.Back, state.Buttons.Back == ButtonState.Pressed);

        return new GamePadSnapshot(pressedControls);
    }

    private static void AddIfPressed(HashSet<GamePadControl> pressedControls, GamePadControl control, bool isPressed)
    {
        if (isPressed)
        {
            pressedControls.Add(control);
        }
    }
}
