using MyGame.Core.Input;

namespace MyGame.Core.Diagnostics;

public sealed class GameRecorder
{
    private readonly List<RecordedFrame> _frames = new();
    private IReadOnlyList<RecordedFrame> _replayFrames = Array.Empty<RecordedFrame>();
    private int _replayIndex;

    public IReadOnlyList<RecordedFrame> Frames => _frames;

    public bool IsRecording { get; private set; }

    public bool IsReplaying => _replayIndex < _replayFrames.Count;

    public bool IsReplayPaused { get; private set; }

    public void Capture(RecordedFrame frame)
    {
        if (!IsRecording || IsReplaying)
        {
            return;
        }

        _frames.Add(frame);
    }

    public void Clear()
    {
        _frames.Clear();
        IsRecording = false;
        StopReplay();
    }

    public void StartRecording()
    {
        _frames.Clear();
        StopReplay();
        IsRecording = true;
    }

    public void StopRecording()
    {
        IsRecording = false;
    }

    public void StartReplayFromBeginning()
    {
        StartReplay(_frames);
    }

    public void StartReplay(IEnumerable<RecordedFrame> frames)
    {
        StopRecording();
        _replayFrames = frames.ToArray();
        _replayIndex = 0;
        IsReplayPaused = false;
    }

    public void PauseReplay()
    {
        if (!IsReplaying)
        {
            return;
        }

        IsReplayPaused = true;
    }

    public void ResumeReplay()
    {
        if (!IsReplaying)
        {
            return;
        }

        IsReplayPaused = false;
    }

    public void StopReplay()
    {
        _replayFrames = Array.Empty<RecordedFrame>();
        _replayIndex = 0;
        IsReplayPaused = false;
    }

    public bool TryDequeueReplayInput(out InputSnapshot snapshot)
    {
        if (!IsReplaying || IsReplayPaused)
        {
            snapshot = InputSnapshot.Empty;
            return false;
        }

        snapshot = _replayFrames[_replayIndex].Input;
        _replayIndex++;

        if (!IsReplaying)
        {
            StopReplay();
        }

        return true;
    }
}
