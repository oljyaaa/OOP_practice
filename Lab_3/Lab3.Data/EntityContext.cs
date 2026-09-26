using Lab3.Data.Providers;

namespace Lab3.Data;

// The only DAL class that performs direct file work.
public sealed class EntityContext
{
    private readonly IReadOnlyDictionary<SerializationFormat, DataProvider> _providers;

    public EntityContext()
    {
        DataProvider[] providers = [new BinaryDataProvider(), new XmlDataProvider(), new JsonDataProvider(), new CustomDataProvider()];
        _providers = providers.ToDictionary(provider => provider.Format);
    }

    public void Save<T>(IReadOnlyCollection<T> entities, string filePath, SerializationFormat format)
        where T : class, new() => GetProvider(format).Serialize(entities, filePath);

    public List<T> Load<T>(string filePath, SerializationFormat format)
        where T : class, new()
    {
        if (!File.Exists(filePath)) throw new FileNotFoundException("Вказаний файл не знайдено.", filePath);
        return GetProvider(format).Deserialize<T>(filePath);
    }

    private DataProvider GetProvider(SerializationFormat format) => _providers.TryGetValue(format, out var provider)
        ? provider : throw new ArgumentOutOfRangeException(nameof(format), format, "Непідтримуваний формат серіалізації.");
}
