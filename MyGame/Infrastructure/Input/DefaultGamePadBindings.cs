using MyGame.Core.Input;

namespace MyGame.Infrastructure.Input;

public sealed class DefaultGamePadBindings
{
    public IReadOnlyDictionary<GameAction, GamePadControl[]> Create()
    {
        return new Dictionary<GameAction, GamePadControl[]>
        {
            [GameAction.MoveUp] = [GamePadControl.DPadUp, GamePadControl.LeftStickUp],
            [GameAction.MoveDown] = [GamePadControl.DPadDown, GamePadControl.LeftStickDown],
            [GameAction.MoveLeft] = [GamePadControl.DPadLeft, GamePadControl.LeftStickLeft],
            [GameAction.MoveRight] = [GamePadControl.DPadRight, GamePadControl.LeftStickRight],
            [GameAction.Interact] = [GamePadControl.FaceRight],
            [GameAction.Attack] = [GamePadControl.FaceLeft],
            [GameAction.RangedAttack] = [GamePadControl.RightTrigger],
            [GameAction.DefenseAbility] = [GamePadControl.FaceTop],
            [GameAction.Dash] = [GamePadControl.RightShoulder],
            [GameAction.Confirm] = [GamePadControl.FaceBottom],
            [GameAction.Cancel] = [GamePadControl.FaceRight, GamePadControl.Back],
            [GameAction.Pause] = [GamePadControl.Start],
            [GameAction.PreviousTab] = [GamePadControl.LeftShoulder],
            [GameAction.NextTab] = [GamePadControl.RightShoulder]
        };
    }
}
