namespace Lab3.Business.Exceptions;

// Виняток: дані сутності некоректні (порожнє ім'я, неправильний курс, дата тощо)
public class EntityValidationException : Exception
{
    // Створює виняток з повідомленням про помилку
    public EntityValidationException(string message) : base(message)
    {
    }
}
