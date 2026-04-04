using Microsoft.Xna.Framework.Input;

namespace MyGame.Core.Input;

public sealed class KeyboardInputSnapshotSource : IInputSnapshotSource
{
    private readonly IReadOnlyDictionary<GameAction, Keys[]> _bindings;

    public KeyboardInputSnapshotSource(IReadOnlyDictionary<GameAction, Keys[]> bindings)
    {
        _bindings = bindings;
    }

    public InputSnapshot ReadCurrent()
    {
        var keyboardState = Keyboard.GetState();
        var pressedActions = new HashSet<GameAction>();

        foreach (var binding in _bindings)
        {
            if (binding.Value.Any(keyboardState.IsKeyDown))
            {
                pressedActions.Add(binding.Key);
            }
        }

        return new InputSnapshot(pressedActions);
    }
}
