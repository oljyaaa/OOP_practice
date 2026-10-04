using System.Text.Encodings.Web;
using System.Text.Json;
using Lab3.Data.Entities;

namespace Lab3.Data.Providers;

// Провайдер JSON-серіалізації (System.Text.Json)
public class JsonDataProvider : DataProvider
{
    // WriteIndented - гарні відступи у файлі; Encoder - щоб кирилиця не перетворювалась на \uXXXX
    private readonly JsonSerializerOptions options = new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    // Записує список об'єктів у JSON-файл
    public override void Write<T>(List<T> items, string filePath)
    {
        string json = JsonSerializer.Serialize(items, options);
        File.WriteAllText(filePath, json);
    }

    // Читає список об'єктів з JSON-файлу
    public override List<T> Read<T>(string filePath)
    {
        string json = File.ReadAllText(filePath);
        List<T>? items = JsonSerializer.Deserialize<List<T>>(json, options);
        if (items == null)
        {
            return new List<T>();
        }
        return items;
    }
}
