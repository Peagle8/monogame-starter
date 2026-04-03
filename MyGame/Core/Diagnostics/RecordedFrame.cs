using MyGame.Core.Input;

namespace MyGame.Core.Diagnostics;

public sealed record RecordedFrame(
    TimeSpan Timestamp,
    string SceneName,
    InputSnapshot Input,
    IReadOnlyDictionary<string, string> State);
