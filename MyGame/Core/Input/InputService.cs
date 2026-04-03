using Microsoft.Xna.Framework.Input;

namespace MyGame.Core.Input;

public sealed class InputService : IInputService
{
    private readonly IReadOnlyDictionary<GameAction, Keys[]> _bindings;

    public InputService(IReadOnlyDictionary<GameAction, Keys[]> bindings)
    {
        _bindings = bindings;
        Current = InputSnapshot.Empty;
        Previous = InputSnapshot.Empty;
    }

    public InputSnapshot Current { get; private set; }

    public InputSnapshot Previous { get; private set; }

    public void Update()
    {
        Previous = Current;

        var keyboardState = Keyboard.GetState();
        var pressedActions = new HashSet<GameAction>();

        foreach (var binding in _bindings)
        {
            if (binding.Value.Any(keyboardState.IsKeyDown))
            {
                pressedActions.Add(binding.Key);
            }
        }

        Current = new InputSnapshot(pressedActions);
    }

    public bool IsPressed(GameAction action)
    {
        return Current.IsPressed(action);
    }

    public bool IsJustPressed(GameAction action)
    {
        return Current.IsPressed(action) && !Previous.IsPressed(action);
    }

    public bool IsJustReleased(GameAction action)
    {
        return !Current.IsPressed(action) && Previous.IsPressed(action);
    }
}
