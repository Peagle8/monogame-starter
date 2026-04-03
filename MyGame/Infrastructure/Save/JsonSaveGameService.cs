using System.Text.Json;
using MyGame.Infrastructure.Logging;

namespace MyGame.Infrastructure.Save;

public sealed class JsonSaveGameService : ISaveGameService
{
    private readonly ILogger _logger;
    private readonly string _savePath;
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true
    };

    public JsonSaveGameService(ILogger logger, string savePath)
    {
        _logger = logger;
        _savePath = savePath;
    }

    public bool SaveExists()
    {
        return File.Exists(_savePath);
    }

    public void Save(SaveGameData data)
    {
        try
        {
            var directoryPath = Path.GetDirectoryName(_savePath);
            if (!string.IsNullOrWhiteSpace(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var json = JsonSerializer.Serialize(data, _serializerOptions);
            File.WriteAllText(_savePath, json);
            _logger.Info($"Saved game to '{_savePath}'.");
        }
        catch (IOException exception)
        {
            _logger.Error($"Failed to save game to '{_savePath}': {exception.Message}");
        }
    }

    public SaveGameData? Load()
    {
        try
        {
            if (!File.Exists(_savePath))
            {
                _logger.Warning($"Save file '{_savePath}' was not found.");
                return null;
            }

            var json = File.ReadAllText(_savePath);
            var data = JsonSerializer.Deserialize<SaveGameData>(json, _serializerOptions);

            if (data is null)
            {
                _logger.Warning($"Save file '{_savePath}' was empty or invalid.");
                return null;
            }

            _logger.Info($"Loaded save game from '{_savePath}'.");
            return data;
        }
        catch (JsonException exception)
        {
            _logger.Error($"Failed to parse save file '{_savePath}': {exception.Message}");
            return null;
        }
        catch (IOException exception)
        {
            _logger.Error($"Failed to load save file '{_savePath}': {exception.Message}");
            return null;
        }
    }
}
