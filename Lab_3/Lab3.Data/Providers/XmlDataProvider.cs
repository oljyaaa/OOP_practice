using System.Xml.Serialization;

namespace Lab3.Data.Providers;

public sealed class XmlDataProvider : DataProvider
{
    public override SerializationFormat Format => SerializationFormat.Xml;

    public override void Serialize<T>(IReadOnlyCollection<T> entities, string filePath)
    {
        EnsureFilePath(filePath);
        var serializer = new XmlSerializer(typeof(List<T>));
        using var stream = File.Create(filePath);
        serializer.Serialize(stream, entities.ToList());
    }

    public override List<T> Deserialize<T>(string filePath)
    {
        var serializer = new XmlSerializer(typeof(List<T>));
        using var stream = File.OpenRead(filePath);
        return (List<T>?)serializer.Deserialize(stream) ?? [];
    }
}
