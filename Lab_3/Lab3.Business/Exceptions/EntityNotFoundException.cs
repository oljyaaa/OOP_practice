namespace Lab3.Business.Exceptions;

// Виняток: студента, пекаря, підприємця або рядок не знайдено
public class EntityNotFoundException : Exception
{
    // Створює виняток з повідомленням про помилку
    public EntityNotFoundException(string message) : base(message)
    {
    }
}
