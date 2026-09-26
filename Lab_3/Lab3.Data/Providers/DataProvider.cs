namespace Lab3.Data.Providers;

public abstract class DataProvider
{
    public abstract SerializationFormat Format { get; }

    public abstract void Serialize<T>(IReadOnlyCollection<T> entities, string filePath)
        where T : class, new();

    public abstract List<T> Deserialize<T>(string filePath)
        where T : class, new();

    protected static void EnsureFilePath(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("Ім'я файлу не може бути порожнім.", nameof(filePath));
        }

        var directory = Path.GetDirectoryName(Path.GetFullPath(filePath));
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}
