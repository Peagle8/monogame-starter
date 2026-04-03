namespace MyGame.Core.Diagnostics;

// TODO: make usage of this configurable so that it is not recording by default and only for debug situations
public sealed class GameRecorder
{
    private readonly List<RecordedFrame> _frames = new();

    public IReadOnlyList<RecordedFrame> Frames => _frames;

    public void Capture(RecordedFrame frame)
    {
        _frames.Add(frame);
    }

    public void Clear()
    {
        _frames.Clear();
    }
}
