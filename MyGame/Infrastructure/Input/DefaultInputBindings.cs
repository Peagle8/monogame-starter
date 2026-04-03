using Microsoft.Xna.Framework.Input;
using MyGame.Core.Input;

namespace MyGame.Infrastructure.Input;

public sealed class DefaultInputBindings
{
    public IReadOnlyDictionary<GameAction, Keys[]> Create()
    {
        return new Dictionary<GameAction, Keys[]>
        {
            [GameAction.MoveUp] = [Keys.W, Keys.Up],
            [GameAction.MoveDown] = [Keys.S, Keys.Down],
            [GameAction.MoveLeft] = [Keys.A, Keys.Left],
            [GameAction.MoveRight] = [Keys.D, Keys.Right],
            [GameAction.Confirm] = [Keys.Enter, Keys.Space],
            [GameAction.Cancel] = [Keys.Escape, Keys.Back],
            [GameAction.Pause] = [Keys.P, Keys.Escape]
        };
    }
}
