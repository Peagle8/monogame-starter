namespace MyGame.Core.Input;

public interface IInputService
{
    InputSnapshot Current { get; }

    InputSnapshot Previous { get; }

    void Update();

    bool IsPressed(GameAction action);

    bool IsJustPressed(GameAction action);

    bool IsJustReleased(GameAction action);
}
