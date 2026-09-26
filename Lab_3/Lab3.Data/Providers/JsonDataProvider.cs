using System.Text.Json;

namespace Lab3.Data.Providers;

public sealed class JsonDataProvider : DataProvider
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public override SerializationFormat Format => SerializationFormat.Json;

    public override void Serialize<T>(IReadOnlyCollection<T> entities, string filePath)
    {
        EnsureFilePath(filePath);
        File.WriteAllText(filePath, JsonSerializer.Serialize(entities, Options));
    }

    public override List<T> Deserialize<T>(string filePath) =>
        JsonSerializer.Deserialize<List<T>>(File.ReadAllText(filePath), Options) ?? [];
}
