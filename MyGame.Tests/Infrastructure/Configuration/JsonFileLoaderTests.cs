using MyGame.Configuration;
using MyGame.Infrastructure.Configuration;
using MyGame.Infrastructure.Logging;

namespace MyGame.Tests.Infrastructure.Configuration;

public sealed class JsonFileLoaderTests : IDisposable
{
    private readonly string _directoryPath;

    public JsonFileLoaderTests()
    {
        _directoryPath = Path.Combine(Path.GetTempPath(), $"MyGame.Tests.{Guid.NewGuid():N}");
        Directory.CreateDirectory(_directoryPath);
    }

    [Fact]
    public void LoadOrDefault_LoadsSettingsFromJson()
    {
        var logger = new InMemoryLogger();
        var loader = new JsonFileLoader<PlayerMovementSettings>(logger);
        var path = Path.Combine(_directoryPath, "PlayerMovementSettings.json");
        File.WriteAllText(path, """
            {
              "MoveSpeed": 240
            }
            """);

        var settings = loader.LoadOrDefault(path, new PlayerMovementSettings());

        Assert.Equal(240f, settings.MoveSpeed);
        Assert.Contains(logger.Entries, entry => entry.Level == "Info");
    }

    [Fact]
    public void LoadOrDefault_LoadsEnemySettingsFromJson()
    {
        var logger = new InMemoryLogger();
        var loader = new JsonFileLoader<EnemySettings>(logger);
        var path = Path.Combine(_directoryPath, "EnemySettings.json");
        File.WriteAllText(path, """
            {
              "MaxHealth": 5,
              "MoveSpeed": 150,
              "ChaseRange": 220
            }
            """);

        var settings = loader.LoadOrDefault(path, new EnemySettings());

        Assert.Equal(5, settings.MaxHealth);
        Assert.Equal(150f, settings.MoveSpeed);
        Assert.Equal(220f, settings.ChaseRange);
        Assert.Contains(logger.Entries, entry => entry.Level == "Info");
    }

    [Fact]
    public void LoadOrDefault_LoadsDiagnosticsSettingsFromJson()
    {
        var logger = new InMemoryLogger();
        var loader = new JsonFileLoader<DiagnosticsSettings>(logger);
        var path = Path.Combine(_directoryPath, "DiagnosticsSettings.json");
        File.WriteAllText(path, """
            {
              "EnableReplayMenu": false
            }
            """);

        var settings = loader.LoadOrDefault(path, new DiagnosticsSettings());

        Assert.False(settings.EnableReplayMenu);
        Assert.Contains(logger.Entries, entry => entry.Level == "Info");
    }

    [Fact]
    public void LoadOrDefault_WhenFileIsMissing_ReturnsDefaultValue()
    {
        var logger = new InMemoryLogger();
        var loader = new JsonFileLoader<PlayerMovementSettings>(logger);
        var path = Path.Combine(_directoryPath, "missing.json");
        var defaultValue = new PlayerMovementSettings { MoveSpeed = 180f };

        var settings = loader.LoadOrDefault(path, defaultValue);

        Assert.Same(defaultValue, settings);
        Assert.Contains(logger.Entries, entry => entry.Level == "Warning");
    }

    [Fact]
    public void LoadOrDefault_WhenJsonIsInvalid_ReturnsDefaultValue()
    {
        var logger = new InMemoryLogger();
        var loader = new JsonFileLoader<PlayerMovementSettings>(logger);
        var path = Path.Combine(_directoryPath, "PlayerMovementSettings.json");
        var defaultValue = new PlayerMovementSettings { MoveSpeed = 180f };
        File.WriteAllText(path, "{");

        var settings = loader.LoadOrDefault(path, defaultValue);

        Assert.Same(defaultValue, settings);
        Assert.Contains(logger.Entries, entry => entry.Level == "Warning");
    }

    public void Dispose()
    {
        if (Directory.Exists(_directoryPath))
        {
            Directory.Delete(_directoryPath, recursive: true);
        }
    }
}
