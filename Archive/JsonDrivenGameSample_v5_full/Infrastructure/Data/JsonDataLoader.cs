using System.IO;
using System.Text.Json;

namespace JsonDrivenGameSample.Infrastructure.Data
{
    public sealed class JsonDataLoader
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public T Load<T>(string path)
        {
            string json = File.ReadAllText(path);
            T? result = JsonSerializer.Deserialize<T>(json, Options);

            if (result is null)
            {
                throw new InvalidDataException($"Could not deserialize JSON file at path: {path}");
            }

            return result;
        }
    }
}
