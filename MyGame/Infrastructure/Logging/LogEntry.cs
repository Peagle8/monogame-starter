namespace MyGame.Infrastructure.Logging;

public sealed record LogEntry(DateTimeOffset Timestamp, string Level, string Message);
