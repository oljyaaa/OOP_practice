using System.Text.Encodings.Web;
using System.Text.Json;

namespace Lab5.DAL;

/// <summary>Зберігає студентів у JSON-файлі (серіалізація).</summary>
public class JsonStudentRepository : IStudentRepository
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    private readonly string _filePath;

    /// <summary>Створює репозиторій для вказаного файлу.</summary>
    public JsonStudentRepository(string filePath)
    {
        _filePath = filePath;
    }

    /// <summary>Читає список студентів з файлу; якщо файлу немає — повертає порожній список.</summary>
    public List<StudentRecord> Load()
    {
        if (!File.Exists(_filePath))
        {
            return new List<StudentRecord>();
        }

        string json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<StudentRecord>>(json, Options) ?? new List<StudentRecord>();
    }

    /// <summary>Записує список студентів у файл.</summary>
    public void Save(List<StudentRecord> students)
    {
        File.WriteAllText(_filePath, JsonSerializer.Serialize(students, Options));
    }
}
