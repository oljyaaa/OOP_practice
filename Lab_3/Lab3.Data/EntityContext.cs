using Lab3.Data.Entities;
using Lab3.Data.Providers;

namespace Lab3.Data;

// Найнижчий рівень (DAL): безпосередня робота з файлами бази даних.
// Містить узагальнені методи створення, відкриття, оновлення, закриття та видалення файлу.
public class EntityContext
{
    // Файли, з якими зараз працює контекст (відкриті й ще не закриті)
    private readonly List<string> openedFiles = new List<string>();

    // Створює новий порожній файл бази даних
    public void Create<T>(string filePath, SerializationFormat format) where T : IEntity, new()
    {
        if (File.Exists(filePath))
        {
            throw new IOException("Файл вже існує.");
        }
        GetProvider(format).Write(new List<T>(), filePath);
        openedFiles.Add(filePath);
    }

    // Відкриває файл і повертає всі об'єкти з нього у вигляді колекції
    public List<T> Open<T>(string filePath, SerializationFormat format) where T : IEntity, new()
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Файл не знайдено: " + filePath);
        }
        List<T> items = GetProvider(format).Read<T>(filePath);
        openedFiles.Add(filePath);
        return items;
    }

    // Відкриває файл і повертає всі об'єкти з нього у вигляді масиву
    public T[] OpenArray<T>(string filePath, SerializationFormat format) where T : IEntity, new()
    {
        return Open<T>(filePath, format).ToArray();
    }

    // Оновлює файл: перезаписує його вмістом колекції
    public void Update<T>(List<T> items, string filePath, SerializationFormat format) where T : IEntity, new()
    {
        GetProvider(format).Write(items, filePath);
        openedFiles.Add(filePath);
    }

    // Оновлює файл: перезаписує його вмістом масиву
    public void UpdateArray<T>(T[] items, string filePath, SerializationFormat format) where T : IEntity, new()
    {
        Update(new List<T>(items), filePath, format);
    }

    // Закриває файл (контекст більше не працює з ним)
    public void Close(string filePath)
    {
        openedFiles.Remove(filePath);
    }

    // Видаляє файл бази даних
    public void Delete(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Файл не знайдено: " + filePath);
        }
        File.Delete(filePath);
        openedFiles.Remove(filePath);
    }

    // Повертає провайдер, який відповідає обраному способу серіалізації
    private static DataProvider GetProvider(SerializationFormat format)
    {
        switch (format)
        {
            case SerializationFormat.Binary:
                return new BinaryDataProvider();
            case SerializationFormat.Xml:
                return new XmlDataProvider();
            case SerializationFormat.Json:
                return new JsonDataProvider();
            default:
                return new CustomDataProvider();
        }
    }
}
