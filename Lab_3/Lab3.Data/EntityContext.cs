using Lab3.Data.Providers;

namespace Lab3.Data;

// The only DAL class that performs direct file work.
public sealed class EntityContext
{
    private readonly IReadOnlyDictionary<SerializationFormat, DataProvider> _providers;
    private readonly HashSet<string> _openFiles = new(StringComparer.OrdinalIgnoreCase);

    public EntityContext()
    {
        DataProvider[] providers = [new BinaryDataProvider(), new XmlDataProvider(), new JsonDataProvider(), new CustomDataProvider()];
        _providers = providers.ToDictionary(provider => provider.Format);
    }

    // Generic file lifecycle operations required by the assignment.
    public void Create<T>(string filePath, SerializationFormat format) where T : class, new()
    {
        var fullPath = NormalizePath(filePath);
        if (File.Exists(fullPath)) throw new IOException("Файл уже існує. Для перезапису використайте оновлення.");
        GetProvider(format).Serialize(Array.Empty<T>(), fullPath);
        _openFiles.Add(fullPath);
    }

    public List<T> Open<T>(string filePath, SerializationFormat format)
        where T : class, new()
    {
        var fullPath = NormalizePath(filePath);
        if (!File.Exists(fullPath)) throw new FileNotFoundException("Вказаний файл не знайдено.", fullPath);
        var entities = GetProvider(format).Deserialize<T>(fullPath);
        _openFiles.Add(fullPath);
        return entities;
    }

    public void Update<T>(IReadOnlyCollection<T> entities, string filePath, SerializationFormat format)
        where T : class, new()
    {
        var fullPath = NormalizePath(filePath);
        GetProvider(format).Serialize(entities, fullPath);
        _openFiles.Add(fullPath);
    }

    public void Close(string filePath) => _openFiles.Remove(NormalizePath(filePath));

    public void Delete(string filePath)
    {
        var fullPath = NormalizePath(filePath);
        if (!File.Exists(fullPath)) throw new FileNotFoundException("Вказаний файл не знайдено.", fullPath);
        File.Delete(fullPath);
        _openFiles.Remove(fullPath);
    }

    // Backward-compatible names clarify the two primary operations in the service layer.
    public void Save<T>(IReadOnlyCollection<T> entities, string filePath, SerializationFormat format)
        where T : class, new() => Update(entities, filePath, format);

    public List<T> Load<T>(string filePath, SerializationFormat format)
        where T : class, new() => Open<T>(filePath, format);

    private DataProvider GetProvider(SerializationFormat format) => _providers.TryGetValue(format, out var provider)
        ? provider : throw new ArgumentOutOfRangeException(nameof(format), format, "Непідтримуваний формат серіалізації.");

    private static string NormalizePath(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("Ім'я файлу не може бути порожнім.", nameof(filePath));
        return Path.GetFullPath(filePath);
    }
}
