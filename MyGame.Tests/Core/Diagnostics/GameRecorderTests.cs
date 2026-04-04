using MyGame.Core.Diagnostics;
using MyGame.Core.Input;

namespace MyGame.Tests.Core.Diagnostics;

public sealed class GameRecorderTests
{
    [Fact]
    public void TryDequeueReplayInput_ReturnsRecordedInputsInOrder()
    {
        var recorder = new GameRecorder();
        var first = CreateFrame(TimeSpan.Zero, GameAction.MoveRight);
        var second = CreateFrame(TimeSpan.FromSeconds(1), GameAction.Attack);

        recorder.StartReplay(new[] { first, second });

        var firstResult = recorder.TryDequeueReplayInput(out var firstInput);
        var secondResult = recorder.TryDequeueReplayInput(out var secondInput);
        var thirdResult = recorder.TryDequeueReplayInput(out var finalInput);

        Assert.True(firstResult);
        Assert.Equal(first.Input, firstInput);
        Assert.True(secondResult);
        Assert.Equal(second.Input, secondInput);
        Assert.False(thirdResult);
        Assert.Equal(InputSnapshot.Empty, finalInput);
        Assert.False(recorder.IsReplaying);
    }

    [Fact]
    public void Capture_DoesNotAppendFrames_WhenRecordingIsDisabled()
    {
        var recorder = new GameRecorder();

        recorder.Capture(CreateFrame(TimeSpan.Zero, GameAction.MoveLeft));

        Assert.Empty(recorder.Frames);
    }

    [Fact]
    public void StartRecording_ClearsExistingFrames_AndEnablesCapture()
    {
        var recorder = new GameRecorder();
        recorder.StartRecording();
        recorder.Capture(CreateFrame(TimeSpan.Zero, GameAction.MoveLeft));
        recorder.StopRecording();

        recorder.StartRecording();
        recorder.Capture(CreateFrame(TimeSpan.FromSeconds(1), GameAction.Attack));

        Assert.Single(recorder.Frames);
        Assert.True(recorder.IsRecording);
        Assert.Equal(GameAction.Attack, recorder.Frames[0].Input.PressedActions.Single());
    }

    [Fact]
    public void Capture_DoesNotAppendFramesWhileReplayIsActive()
    {
        var recorder = new GameRecorder();
        var existing = CreateFrame(TimeSpan.Zero, GameAction.MoveLeft);
        recorder.StartRecording();
        recorder.Capture(existing);
        recorder.StopRecording();
        recorder.StartReplayFromBeginning();

        recorder.Capture(CreateFrame(TimeSpan.FromSeconds(1), GameAction.Attack));

        Assert.Single(recorder.Frames);
        Assert.Equal(existing, recorder.Frames[0]);
    }

    [Fact]
    public void PauseReplay_PreventsFramesFromAdvancing_UntilResumed()
    {
        var recorder = new GameRecorder();
        recorder.StartReplay(new[]
        {
            CreateFrame(TimeSpan.Zero, GameAction.MoveLeft),
            CreateFrame(TimeSpan.FromSeconds(1), GameAction.Attack)
        });
        recorder.PauseReplay();

        var pausedResult = recorder.TryDequeueReplayInput(out var pausedInput);

        recorder.ResumeReplay();
        var resumedResult = recorder.TryDequeueReplayInput(out var resumedInput);

        Assert.False(pausedResult);
        Assert.Equal(InputSnapshot.Empty, pausedInput);
        Assert.True(resumedResult);
        Assert.True(resumedInput.IsPressed(GameAction.MoveLeft));
    }

    private static RecordedFrame CreateFrame(TimeSpan timestamp, params GameAction[] actions)
    {
        return new RecordedFrame(
            timestamp,
            "Gameplay",
            new InputSnapshot(actions.ToHashSet()),
            new Dictionary<string, string>());
    }
}
