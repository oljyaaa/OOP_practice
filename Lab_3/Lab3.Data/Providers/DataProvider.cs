using Lab3.Data.Entities;

namespace Lab3.Data.Providers;

// Абстракція провайдера даних: кожен спосіб серіалізації має власного нащадка.
// Методи узагальнені (generic), тому один провайдер працює з будь-якою сутністю.
public abstract class DataProvider
{
    // Серіалізує список об'єктів у файл
    public abstract void Write<T>(List<T> items, string filePath) where T : IEntity, new();

    // Десеріалізує список об'єктів з файлу
    public abstract List<T> Read<T>(string filePath) where T : IEntity, new();
}
