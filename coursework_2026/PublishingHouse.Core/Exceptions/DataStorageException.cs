namespace PublishingHouse.Core.Exceptions
{
    /// <summary>
    /// Помилка читання або запису файлу з даними.
    /// </summary>
    public class DataStorageException : PublishingException
    {
        public string FilePath { get; }

        public DataStorageException(string message, string filePath, Exception innerException)
            : base($"{message} Файл: {filePath}. {innerException.Message}", innerException)
        {
            FilePath = filePath;
        }
    }
}
