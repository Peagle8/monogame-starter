using MyGame.Core.Input;

namespace MyGame.Infrastructure.Input;

public sealed class GamePadInputSnapshotSource : IInputSnapshotSource
{
    private readonly MonoGameGamePadSnapshotReader _reader;
    private readonly GamePadSnapshotMapper _mapper;

    public GamePadInputSnapshotSource(
        MonoGameGamePadSnapshotReader reader,
        IReadOnlyDictionary<GameAction, GamePadControl[]> bindings)
    {
        _reader = reader;
        _mapper = new GamePadSnapshotMapper(bindings);
    }

    public InputSnapshot ReadCurrent()
    {
        return _mapper.Map(_reader.ReadCurrent());
    }
}
