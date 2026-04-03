using System.Text.Json;
using MyGame.Infrastructure.Logging;

namespace MyGame.Infrastructure.Configuration;

public sealed class JsonFileLoader<T> where T : class
{
    private readonly ILogger _logger;
    private readonly JsonSerializerOptions _serializerOptions;

    public JsonFileLoader(ILogger logger)
    {
        _logger = logger;
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public T LoadOrDefault(string path, T defaultValue)
    {
        try
        {
            if (!File.Exists(path))
            {
                _logger.Warning($"Configuration file '{path}' was not found. Using defaults.");
                return defaultValue;
            }

            var json = File.ReadAllText(path);
            var value = JsonSerializer.Deserialize<T>(json, _serializerOptions);

            if (value is null)
            {
                _logger.Warning($"Configuration file '{path}' was empty or invalid. Using defaults.");
                return defaultValue;
            }

            _logger.Info($"Loaded configuration from '{path}'.");
            return value;
        }
        catch (JsonException exception)
        {
            _logger.Warning($"Configuration file '{path}' could not be parsed: {exception.Message}. Using defaults.");
            return defaultValue;
        }
        catch (IOException exception)
        {
            _logger.Warning($"Configuration file '{path}' could not be read: {exception.Message}. Using defaults.");
            return defaultValue;
        }
    }
}
