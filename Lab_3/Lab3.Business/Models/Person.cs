namespace Lab3.Business.Models;

// Базовий клас для студента, пекаря та підприємця
public abstract class Person : ISkydiver
{
    public string Surname { get; set; }
    public string FirstName { get; set; }

    // Задає прізвище та ім'я
    protected Person(string surname, string firstName)
    {
        Surname = surname;
        FirstName = firstName;
    }

    // Повертає інформацію про людину одним рядком
    public abstract string GetInfo();

    // Людина стрибає з парашутом
    public string JumpWithParachute()
    {
        return Surname + " " + FirstName + " стрибає з парашутом!";
    }
}
