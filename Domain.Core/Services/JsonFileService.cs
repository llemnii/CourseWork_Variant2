using System.Text.Json;
using Domain.Core.Exceptions;
using Domain.Core.Interfaces;

namespace Domain.Core.Services;

public class JsonFileService<T> : IFileService<T>
{
    private readonly string _filePath;

    public JsonFileService(string filePath)
    {
        _filePath = filePath;
    }

    public void Save(IEnumerable<T> data)
    {
        try
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            throw new StorageException($"Не вдалося зберегти файл {_filePath}.", ex);
        }
    }

    public List<T> Load()
    {
        if (!File.Exists(_filePath))
            return new List<T>();

        try
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }
        catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException)
        {
            throw new StorageException($"Не вдалося прочитати файл {_filePath}.", ex);
        }
    }
}
