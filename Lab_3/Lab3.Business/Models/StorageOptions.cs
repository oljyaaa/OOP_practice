namespace Lab3.Business.Models;

// Ім'я (шлях) і формат файлу, які передаються з рівня представлення на нижчі рівні
public class StorageOptions
{
    public string FilePath { get; }
    public FileFormat Format { get; }

    // Створює параметри файлу
    public StorageOptions(string filePath, FileFormat format)
    {
        FilePath = filePath;
        Format = format;
    }
}
