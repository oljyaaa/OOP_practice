using Lab31.Domain;

namespace Lab31;

public sealed class ConsoleMenu
{
    private readonly PersonService service;
    private readonly IPersonInputHandler[] inputHandlers;
    private readonly IPersonAction[] actions;

    public ConsoleMenu(
        PersonService service,
        PersonInputHandlerRegistry inputHandlerRegistry,
        PersonActionRegistry actionRegistry)
    {
        this.service = service;
        inputHandlers = inputHandlerRegistry.GetAll();
        actions = actionRegistry.GetAll();
    }

    public void Start()
    {
        int choice;

        do
        {
            PrintMenu();
            choice = ReadMenuChoice();

            try
            {
                ExecuteChoice(choice);
            }
            catch (Exception exception) when (exception is IOException or InvalidDataException or FormatException or InvalidOperationException)
            {
                Console.WriteLine($"Помилка: {exception.Message}");
            }
        } while (choice != 0);
    }

    private void PrintMenu()
    {
        Console.WriteLine();

        for (int i = 0; i < inputHandlers.Length; i++)
            Console.WriteLine($"{i + 1} - {inputHandlers[i].MenuTitle}");

        int operationsStart = inputHandlers.Length + 1;
        Console.WriteLine($"{operationsStart} - Показати всі записи");
        Console.WriteLine($"{operationsStart + 1} - Студенти 4 курсу, які народилися навесні");
        Console.WriteLine($"{operationsStart + 2} - Пошук за прізвищем");
        Console.WriteLine($"{operationsStart + 3} - Пошук за унікальним ідентифікатором");
        Console.WriteLine($"{operationsStart + 4} - Видалити запис за унікальним ідентифікатором");

        for (int i = 0; i < actions.Length; i++)
            Console.WriteLine($"{operationsStart + 5 + i} - {actions[i].MenuTitle}");

        Console.WriteLine("0 - Вихід");
    }

    private static int ReadMenuChoice()
    {
        Console.Write("Оберіть пункт: ");
        return int.TryParse(Console.ReadLine(), out int choice) ? choice : -1;
    }

    private void ExecuteChoice(int choice)
    {
        if (choice == 0)
            return;

        if (choice >= 1 && choice <= inputHandlers.Length)
        {
            AddPerson(inputHandlers[choice - 1]);
            return;
        }

        int operationsStart = inputHandlers.Length + 1;

        switch (choice)
        {
            case int value when value == operationsStart:
                ShowPeople(service.GetAll());
                return;
            case int value when value == operationsStart + 1:
                ShowFourthCourseSpringStudents();
                return;
            case int value when value == operationsStart + 2:
                SearchByLastName();
                return;
            case int value when value == operationsStart + 3:
                SearchById();
                return;
            case int value when value == operationsStart + 4:
                DeleteById();
                return;
        }

        int actionIndex = choice - (operationsStart + 5);
        if (actionIndex >= 0 && actionIndex < actions.Length)
        {
            ExecutePersonAction(actions[actionIndex]);
            return;
        }

        Console.WriteLine("Такого пункту немає.");
    }

    private void AddPerson(IPersonInputHandler inputHandler)
    {
        Person person = inputHandler.CreatePerson();
        service.Add(person);
        Console.WriteLine("Запис додано.");
    }

    private void ShowFourthCourseSpringStudents()
    {
        Student[] students = service.GetFourthCourseSpringStudents();
        Console.WriteLine("\nСтуденти 4 курсу, народжені навесні:");
        ShowPeople(students);
        Console.WriteLine($"Кількість студентів: {students.Length}");
    }

    private void SearchByLastName()
    {
        string lastName = ConsoleInput.ReadName("Прізвище для пошуку: ");
        Person[] people = service.FindByLastName(lastName);
        PrintSearchResult(people);
    }

    private void SearchById()
    {
        Person? person = service.FindById(ConsoleInput.ReadRequiredText("Унікальний ідентифікатор: "));

        if (person is null)
            Console.WriteLine("Запис не знайдено.");
        else
            PrintPerson(person);
    }

    private void DeleteById()
    {
        string uniqueId = ConsoleInput.ReadRequiredText("Унікальний ідентифікатор для видалення: ");
        Console.WriteLine(service.DeleteById(uniqueId) ? "Запис видалено." : "Запис не знайдено.");
    }

    private void ExecutePersonAction(IPersonAction action)
    {
        Person? person = service.FindById(ConsoleInput.ReadRequiredText("Унікальний ідентифікатор особи: "));

        if (person is null)
        {
            Console.WriteLine("Особа не знайдена.");
            return;
        }

        if (!action.CanExecute(person))
        {
            Console.WriteLine("Ця дія недоступна для обраної особи.");
            return;
        }

        Console.WriteLine(action.Execute(person));
    }

    private static void PrintSearchResult(Person[] people)
    {
        if (people.Length == 0)
        {
            Console.WriteLine("Записів не знайдено.");
            return;
        }

        ShowPeople(people);
    }

    private static void ShowPeople(Person[] people)
    {
        if (people.Length == 0)
        {
            Console.WriteLine("База даних порожня.");
            return;
        }

        for (int i = 0; i < people.Length; i++)
            PrintPerson(people[i]);
    }

    private static void PrintPerson(Person person)
    {
        PersonRecord record = person.ToRecord();
        Console.WriteLine();
        Console.WriteLine($"Тип: {record.TypeName}");

        for (int i = 0; i < record.AttributeNames.Length; i++)
            Console.WriteLine($"{record.AttributeNames[i]}: {record.AttributeValues[i]}");
    }
}
