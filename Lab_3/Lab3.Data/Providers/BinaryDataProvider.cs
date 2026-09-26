using System.Text.Json;

namespace Lab3.Data.Providers;

public sealed class BinaryDataProvider : DataProvider
{
    private const string Header = "LAB3-BINARY-1";

    public override SerializationFormat Format => SerializationFormat.Binary;

    public override void Serialize<T>(IReadOnlyCollection<T> entities, string filePath)
    {
        EnsureFilePath(filePath);
        var payload = JsonSerializer.SerializeToUtf8Bytes(entities);
        using var writer = new BinaryWriter(File.Create(filePath));
        writer.Write(Header);
        writer.Write(payload.Length);
        writer.Write(payload);
    }

    public override List<T> Deserialize<T>(string filePath)
    {
        using var reader = new BinaryReader(File.OpenRead(filePath));
        if (reader.ReadString() != Header)
        {
            throw new InvalidDataException("Файл не має формату двійкових даних Lab 3.");
        }

        var length = reader.ReadInt32();
        if (length < 0 || length > reader.BaseStream.Length - reader.BaseStream.Position)
        {
            throw new InvalidDataException("Пошкоджена довжина двійкових даних.");
        }

        return JsonSerializer.Deserialize<List<T>>(reader.ReadBytes(length)) ?? [];
    }
}
