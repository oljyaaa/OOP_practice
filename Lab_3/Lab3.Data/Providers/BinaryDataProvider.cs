namespace Lab3.Data.Providers;

public sealed class BinaryDataProvider : DataProvider
{
    private const string Header = "LAB3-BINARY-1";

    public override SerializationFormat Format => SerializationFormat.Binary;

    public override void Serialize<T>(IReadOnlyCollection<T> entities, string filePath)
    {
        EnsureFilePath(filePath);
        var properties = SerializableProperties.For<T>();
        using var writer = new BinaryWriter(File.Create(filePath));
        writer.Write(Header);
        writer.Write(properties.Length);
        foreach (var property in properties) writer.Write(property.Name);
        writer.Write(entities.Count);
        foreach (var entity in entities)
        {
            foreach (var property in properties)
                WriteValue(writer, property.PropertyType, property.GetValue(entity));
        }
    }

    public override List<T> Deserialize<T>(string filePath)
    {
        using var reader = new BinaryReader(File.OpenRead(filePath));
        if (reader.ReadString() != Header)
        {
            throw new InvalidDataException("Файл не має формату двійкових даних Lab 3.");
        }

        var properties = SerializableProperties.For<T>();
        var propertyCount = reader.ReadInt32();
        if (propertyCount != properties.Length)
        {
            throw new InvalidDataException("Двійковий файл має іншу структуру сутності.");
        }

        foreach (var property in properties)
        {
            if (reader.ReadString() != property.Name)
                throw new InvalidDataException("Двійковий файл має іншу структуру властивостей.");
        }

        var entityCount = reader.ReadInt32();
        if (entityCount < 0) throw new InvalidDataException("Пошкоджена кількість об'єктів у двійковому файлі.");

        var result = new List<T>(entityCount);
        for (var entityIndex = 0; entityIndex < entityCount; entityIndex++)
        {
            var entity = new T();
            foreach (var property in properties)
                property.SetValue(entity, ReadValue(reader, property.PropertyType));
            result.Add(entity);
        }
        return result;
    }

    private static void WriteValue(BinaryWriter writer, Type type, object? value)
    {
        if (type == typeof(string)) writer.Write((string?)value ?? string.Empty);
        else if (type == typeof(int)) writer.Write((int)value!);
        else if (type == typeof(DateTime)) writer.Write(((DateTime)value!).ToBinary());
        else throw new NotSupportedException($"Двійковий формат не підтримує тип {type.Name}.");
    }

    private static object ReadValue(BinaryReader reader, Type type) => type == typeof(string) ? reader.ReadString()
        : type == typeof(int) ? reader.ReadInt32()
        : type == typeof(DateTime) ? DateTime.FromBinary(reader.ReadInt64())
        : throw new NotSupportedException($"Двійковий формат не підтримує тип {type.Name}.");
}
