using Lab3.Data.Entities;

namespace Lab3.Data.Providers;

// Провайдер користувацької серіалізації (власний текстовий формат).
// Кожне поле записується окремим рядком "Назва=Значення", об'єкти розділяються порожнім рядком:
//   Value=C#
//   Length=2
//
//   Value=Варіант 8
//   ...
public class CustomDataProvider : DataProvider
{
    // Записує список об'єктів у файл власного формату
    public override void Write<T>(List<T> items, string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (T item in items)
            {
                foreach (KeyValuePair<string, string> field in item.ToFields())
                {
                    writer.WriteLine(field.Key + "=" + field.Value);
                }
                writer.WriteLine();
            }
        }
    }

    // Читає список об'єктів з файлу власного формату
    public override List<T> Read<T>(string filePath)
    {
        List<T> items = new List<T>();
        Dictionary<string, string> fields = new Dictionary<string, string>();

        foreach (string line in File.ReadAllLines(filePath))
        {
            if (line == "")
            {
                // Порожній рядок - кінець одного об'єкта
                AddItem(items, fields);
                fields = new Dictionary<string, string>();
            }
            else
            {
                int separator = line.IndexOf('=');
                if (separator < 0)
                {
                    throw new InvalidDataException("Рядок файлу не має формату Назва=Значення.");
                }
                fields[line.Substring(0, separator)] = line.Substring(separator + 1);
            }
        }

        // Останній об'єкт, якщо після нього немає порожнього рядка
        AddItem(items, fields);
        return items;
    }

    // Створює об'єкт із зібраних полів і додає його до списку
    private static void AddItem<T>(List<T> items, Dictionary<string, string> fields) where T : IEntity, new()
    {
        if (fields.Count == 0)
        {
            return;
        }
        T item = new T();
        item.FromFields(fields);
        items.Add(item);
    }
}
