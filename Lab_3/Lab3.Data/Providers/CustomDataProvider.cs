using System.Globalization;
using System.Reflection;
using System.Text;

namespace Lab3.Data.Providers;

// Format: a header of property names and Base64-encoded fields in each next line.
public sealed class CustomDataProvider : DataProvider
{
    public override SerializationFormat Format => SerializationFormat.Custom;

    public override void Serialize<T>(IReadOnlyCollection<T> entities, string filePath)
    {
        EnsureFilePath(filePath);
        var properties = SerializableProperties<T>();
        using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
        writer.WriteLine(string.Join('|', properties.Select(property => property.Name)));
        foreach (var entity in entities)
        {
            var values = properties.Select(property => Convert.ToBase64String(
                Encoding.UTF8.GetBytes(ToText(property.GetValue(entity)))));
            writer.WriteLine(string.Join('|', values));
        }
    }

    public override List<T> Deserialize<T>(string filePath)
    {
        var lines = File.ReadAllLines(filePath, Encoding.UTF8);
        if (lines.Length == 0 || string.IsNullOrWhiteSpace(lines[0])) return [];

        var properties = SerializableProperties<T>();
        if (!lines[0].Split('|').SequenceEqual(properties.Select(property => property.Name)))
        {
            throw new InvalidDataException("Файл не відповідає очікуваній структурі сутності.");
        }

        var result = new List<T>();
        foreach (var line in lines.Skip(1).Where(line => !string.IsNullOrWhiteSpace(line)))
        {
            var fields = line.Split('|');
            if (fields.Length != properties.Length)
                throw new InvalidDataException("Некоректна кількість полів у користувацькому файлі.");

            var entity = new T();
            for (var index = 0; index < properties.Length; index++)
            {
                var text = Encoding.UTF8.GetString(Convert.FromBase64String(fields[index]));
                properties[index].SetValue(entity, FromText(text, properties[index].PropertyType));
            }
            result.Add(entity);
        }
        return result;
    }

    private static PropertyInfo[] SerializableProperties<T>() => typeof(T).GetProperties()
        .Where(property => property.CanRead && property.CanWrite)
        .OrderBy(property => property.Name, StringComparer.Ordinal)
        .ToArray();

    private static string ToText(object? value) => value switch
    {
        null => string.Empty,
        DateTime date => date.ToString("O", CultureInfo.InvariantCulture),
        _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty
    };

    private static object FromText(string value, Type type) => type == typeof(string) ? value
        : type == typeof(int) ? int.Parse(value, CultureInfo.InvariantCulture)
        : type == typeof(DateTime) ? DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
        : throw new NotSupportedException($"Користувацький формат не підтримує тип {type.Name}.");
}
