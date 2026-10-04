using Lab3.Data.Entities;

namespace Lab3.Data.Providers;

// Провайдер бінарної серіалізації.
// BinaryFormatter у .NET 9+ видалено з міркувань безпеки, тому дані записуються
// у двійковому вигляді через BinaryWriter і читаються через BinaryReader.
// Структура файлу: заголовок, кількість об'єктів, для кожного об'єкта - кількість полів і пари "назва-значення".
public class BinaryDataProvider : DataProvider
{
    private const string Header = "LAB3-BINARY";

    // Записує список об'єктів у двійковий файл
    public override void Write<T>(List<T> items, string filePath)
    {
        using (BinaryWriter writer = new BinaryWriter(new FileStream(filePath, FileMode.Create)))
        {
            writer.Write(Header);
            writer.Write(items.Count);
            foreach (T item in items)
            {
                Dictionary<string, string> fields = item.ToFields();
                writer.Write(fields.Count);
                foreach (KeyValuePair<string, string> field in fields)
                {
                    writer.Write(field.Key);
                    writer.Write(field.Value);
                }
            }
        }
    }

    // Читає список об'єктів з двійкового файлу
    public override List<T> Read<T>(string filePath)
    {
        List<T> items = new List<T>();
        using (BinaryReader reader = new BinaryReader(new FileStream(filePath, FileMode.Open)))
        {
            if (reader.ReadString() != Header)
            {
                throw new InvalidDataException("Файл не є двійковим файлом цієї програми.");
            }

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                Dictionary<string, string> fields = new Dictionary<string, string>();
                int fieldCount = reader.ReadInt32();
                for (int j = 0; j < fieldCount; j++)
                {
                    string name = reader.ReadString();
                    string value = reader.ReadString();
                    fields[name] = value;
                }

                T item = new T();
                item.FromFields(fields);
                items.Add(item);
            }
        }
        return items;
    }
}
