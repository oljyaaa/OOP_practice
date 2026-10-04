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
            List<T>? items = (List<T>?)serializer.Deserialize(stream);
            if (items == null)
            {
                return new List<T>();
            }
            return items;
        }
    }
}
