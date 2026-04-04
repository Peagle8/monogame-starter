namespace MyGame.Core.Input;

public interface IInputSnapshotSource
{
    InputSnapshot ReadCurrent();
}
