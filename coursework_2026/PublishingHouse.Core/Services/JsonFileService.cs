using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using PublishingHouse.Core.Exceptions;

namespace PublishingHouse.Core.Services
{
    /// <summary>
    /// Збереження та завантаження об'єкта у JSON-файл через System.Text.Json.
    /// </summary>
    public class JsonFileService<T> where T : class, new()
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
            // Кирилиця записується як є, а не у вигляді \uXXXX
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            Converters = { new JsonStringEnumConverter() }
        };

        private readonly string _filePath;

        public JsonFileService(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Шлях до файлу не може бути порожнім.", nameof(filePath));
            _filePath = filePath;
        }

        public string FilePath => _filePath;

        public bool FileExists => File.Exists(_filePath);

        /// <summary>
        /// Зберігає дані. Спочатку пише у тимчасовий файл, щоб збій не пошкодив попередню копію.
        /// </summary>
        public void Save(T data)
        {
            ArgumentNullException.ThrowIfNull(data);
            string tempPath = _filePath + ".tmp";
            try
            {
                string? directory = Path.GetDirectoryName(Path.GetFullPath(_filePath));
                if (!string.IsNullOrEmpty(directory))
                    Directory.CreateDirectory(directory);

                File.WriteAllText(tempPath, JsonSerializer.Serialize(data, Options));
                File.Move(tempPath, _filePath, overwrite: true);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
            {
                throw new DataStorageException("Не вдалося зберегти дані.", _filePath, ex);
            }
        }

        /// <summary>
        /// Завантажує дані; якщо файлу немає — повертає порожній об'єкт.
        /// </summary>
        public T Load()
        {
            if (!FileExists)
                return new T();

            try
            {
                string json = File.ReadAllText(_filePath);
                if (string.IsNullOrWhiteSpace(json))
                    return new T();
                return JsonSerializer.Deserialize<T>(json, Options) ?? new T();
            }
            catch (Exception ex) when (ex is JsonException or ArgumentException or IOException or UnauthorizedAccessException)
            {
                // ArgumentException — дані у файлі не пройшли валідацію властивостей моделей
                throw new DataStorageException("Не вдалося завантажити дані.", _filePath, ex);
            }
        }
    }
}
