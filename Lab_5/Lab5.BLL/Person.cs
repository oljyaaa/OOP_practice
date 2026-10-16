namespace Lab5.BLL;

/// <summary>Базовий клас людини з прізвищем та ім'ям.</summary>
public abstract class Person : ISkydiver
{
    public string LastName { get; }
    public string FirstName { get; }

    /// <summary>Створює людину та перевіряє, що прізвище й ім'я задані.</summary>
    protected Person(string lastName, string firstName)
    {
        LastName = Validator.RequireName(lastName, "Прізвище");
        FirstName = Validator.RequireName(firstName, "Ім'я");
    }

    /// <summary>Стрибок з парашутом (спільне вміння для всіх).</summary>
    public virtual string JumpWithParachute()
    {
        return $"{FirstName} {LastName} стрибає з парашутом!";
    }

    /// <summary>Повертає ім'я та прізвище.</summary>
    public override string ToString() => $"{LastName} {FirstName}";
}
