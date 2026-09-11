using Lab31.Domain;

namespace Lab31;

public sealed class StudentInputHandler : IPersonInputHandler
{
    public string MenuTitle => "Додати студента";

    public Person CreatePerson()
    {
        return new Student(
            ConsoleInput.ReadName("Ім'я: "),
            ConsoleInput.ReadName("Прізвище: "),
            ConsoleInput.ReadCourse(),
            ConsoleInput.ReadStudentId(),
            ConsoleInput.ReadBirthDate());
    }
}

public sealed class BakerInputHandler : IPersonInputHandler
{
    public string MenuTitle => "Додати пекаря";

    public Person CreatePerson()
    {
        return new Baker(
            ConsoleInput.ReadName("Ім'я пекаря: "),
            ConsoleInput.ReadName("Прізвище пекаря: "),
            ConsoleInput.ReadPersonId("Ідентифікатор пекаря (ABC-123): "));
    }
}

public sealed class EntrepreneurInputHandler : IPersonInputHandler
{
    public string MenuTitle => "Додати підприємця";

    public Person CreatePerson()
    {
        return new Entrepreneur(
            ConsoleInput.ReadName("Ім'я підприємця: "),
            ConsoleInput.ReadName("Прізвище підприємця: "),
            ConsoleInput.ReadPersonId("Ідентифікатор підприємця (ABC-123): "));
    }
}
