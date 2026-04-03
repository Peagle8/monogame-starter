namespace MyGame.Infrastructure.Logging;

public interface ILogger
{
    void Info(string message);

    void Warning(string message);

    void Error(string message);

    IReadOnlyList<LogEntry> Entries { get; }
}
