namespace MyGame.Infrastructure.Logging;

public sealed class InMemoryLogger : ILogger
{
    private readonly List<LogEntry> _entries = new();

    public IReadOnlyList<LogEntry> Entries => _entries;

    public void Info(string message)
    {
        _entries.Add(new LogEntry(DateTimeOffset.UtcNow, "Info", message));
    }

    public void Warning(string message)
    {
        _entries.Add(new LogEntry(DateTimeOffset.UtcNow, "Warning", message));
    }

    public void Error(string message)
    {
        _entries.Add(new LogEntry(DateTimeOffset.UtcNow, "Error", message));
    }
}
