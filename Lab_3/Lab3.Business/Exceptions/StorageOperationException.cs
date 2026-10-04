namespace Lab3.Business.Exceptions;

// Виняток: не вдалося створити, прочитати, записати або видалити файл
public class StorageOperationException : Exception
{
    // Створює виняток з повідомленням і початковою (внутрішньою) помилкою
    public StorageOperationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
