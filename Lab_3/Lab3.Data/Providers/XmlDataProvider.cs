using System.Xml.Serialization;
using Lab3.Data.Entities;

namespace Lab3.Data.Providers;

// Провайдер XML-серіалізації (XmlSerializer)
public class XmlDataProvider : DataProvider
{
    // Записує список об'єктів у XML-файл
    public override void Write<T>(List<T> items, string filePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        {
            serializer.Serialize(stream, items);
        }
    }

    // Читає список об'єктів з XML-файлу
    public override List<T> Read<T>(string filePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
        using (FileStream stream = new FileStream(filePath, FileMode.Open))
        {
            List<T>? items;
            try
            {
                items = (List<T>?)serializer.Deserialize(stream);
            }
            catch (InvalidOperationException)
            {
                // XmlSerializer генерує цей виняток, якщо XML пошкоджений або містить іншу сутність
                throw new InvalidDataException("Файл не є XML-файлом з потрібними даними.");
            }

            if (items == null)
            {
                return new List<T>();
            }
            return items;
        }
    }
}
