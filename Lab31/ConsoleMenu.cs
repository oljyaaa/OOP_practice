using System.Globalization;
using System.Text.RegularExpressions;
using Lab31.Domain;

namespace Lab31;

public sealed class ConsoleMenu
{
    private const string NamePattern = @"^[\p{L}][\p{L}'-]{1,49}$";
    private const string StudentIdPattern = @"^[A-Z]{3}-\d{6,7}$";
    private const string PersonIdPattern = @"^[A-Z]{3}-\d{3,7}$";
    private readonly PersonService service;

    public ConsoleMenu(PersonService service)
    {
        this.service = service;
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
                switch (choice)
                {
                    case 1: AddStudent(); break;
                    case 2: AddBaker(); break;
                    case 3: AddEntrepreneur(); break;
                    case 4: ShowPeople(service.GetAll()); break;
                    case 5: ShowFourthCourseSpringStudents(); break;
                    case 6: SearchByLastName(); break;
                    case 7: SearchById(); break;
                    case 8: DeleteById(); break;
                    case 9: DemonstrateParachuteJump(); break;
                    case 0: break;
                    default: Console.WriteLine("Такого пункту немає."); break;
                }
            }
            catch (Exception exception) when (exception is IOException or InvalidDataException or FormatException or InvalidOperationException)
            {
                Console.WriteLine($"Помилка: {exception.Message}");
            }
        } while (choice != 0);
    }

    private static void PrintMenu()
    {
        Console.WriteLine("\n1 - Додати студента");
        Console.WriteLine("2 - Додати пекаря");
        Console.WriteLine("3 - Додати підприємця");
        Console.WriteLine("4 - Показати всі записи");
        Console.WriteLine("5 - Студенти 4 курсу, які народилися навесні");
        Console.WriteLine("6 - Пошук за прізвищем");
        Console.WriteLine("7 - Пошук за унікальним ідентифікатором");
        Console.WriteLine("8 - Видалити запис за унікальним ідентифікатором");
        Console.WriteLine("9 - Продемонструвати стрибок із парашутом");
        Console.WriteLine("0 - Вихід");
    }

    private static int ReadMenuChoice()
    {
        Console.Write("Оберіть пункт: ");
        return int.TryParse(Console.ReadLine(), out int choice) ? choice : -1;
    }

    private void AddStudent()
    {
        string firstName = ReadName("Ім'я: ");
        string lastName = ReadName("Прізвище: ");
        int course = ReadCourse();
        string studentId = ReadByPattern("Студентський квиток (ABC-123456): ", StudentIdPattern, "Формат: три великі літери, дефіс і 6-7 цифр.");
        DateTime birthDate = ReadBirthDate();

        service.Add(new Student(firstName, lastName, course, studentId, birthDate));
        Console.WriteLine("Студента додано.");
    }

    private void AddBaker()
    {
        string firstName = ReadName("Ім'я пекаря: ");
        string lastName = ReadName("Прізвище пекаря: ");
        string personId = ReadByPattern("Ідентифікатор пекаря (ABC-123): ", PersonIdPattern, "Формат: три великі літери, дефіс і 3-7 цифр.");

        service.Add(new Baker(firstName, lastName, personId));
        Console.WriteLine("Пекаря додано.");
    }

    private void AddEntrepreneur()
    {
        string firstName = ReadName("Ім'я підприємця: ");
        string lastName = ReadName("Прізвище підприємця: ");
        string personId = ReadByPattern("Ідентифікатор підприємця (ABC-123): ", PersonIdPattern, "Формат: три великі літери, дефіс і 3-7 цифр.");

        service.Add(new Entrepreneur(firstName, lastName, personId));
        Console.WriteLine("Підприємця додано.");
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
        string lastName = ReadName("Прізвище для пошуку: ");
        Person[] people = service.FindByLastName(lastName);
        PrintSearchResult(people);
    }

    private void SearchById()
    {
        Console.Write("Унікальний ідентифікатор: ");
        string uniqueId = (Console.ReadLine() ?? string.Empty).Trim();
        Person? person = service.FindById(uniqueId);

        if (person is null)
            Console.WriteLine("Запис не знайдено.");
        else
            PrintPerson(person);
    }

    private void DeleteById()
    {
        Console.Write("Унікальний ідентифікатор для видалення: ");
        string uniqueId = (Console.ReadLine() ?? string.Empty).Trim();
        Console.WriteLine(service.DeleteById(uniqueId) ? "Запис видалено." : "Запис не знайдено.");
    }

    private void DemonstrateParachuteJump()
    {
        Console.Write("Унікальний ідентифікатор особи: ");
        string uniqueId = (Console.ReadLine() ?? string.Empty).Trim();
        Person? person = service.FindById(uniqueId);

        if (person is not IParachuteJump parachutist)
        {
            Console.WriteLine("Особа не знайдена або не має цієї навички.");
            return;
        }

        parachutist.JumpWithParachute();
        Console.WriteLine($"{person.FirstName} {person.LastName} виконав(ла) стрибок із парашутом.");
    }

    private static string ReadName(string prompt)
    {
        return ReadByPattern(prompt, NamePattern, "Використовуйте лише літери, апостроф або дефіс.");
    }

    private static int ReadCourse()
    {
        while (true)
        {
            Console.Write("Курс (1-6): ");
            string value = (Console.ReadLine() ?? string.Empty).Trim();

            if (Regex.IsMatch(value, "^[1-6]$") && int.TryParse(value, out int course))
                return course;

            Console.WriteLine("Курс має бути числом від 1 до 6.");
        }
    }

    private static DateTime ReadBirthDate()
    {
        while (true)
        {
            string value = ReadByPattern("Дата народження (ДД.ММ.РРРР): ", @"^\d{2}\.\d{2}\.\d{4}$", "Формат: ДД.ММ.РРРР.");

            if (DateTime.TryParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthDate))
                return birthDate;

            Console.WriteLine("Такої календарної дати не існує.");
        }
    }

    private static string ReadByPattern(string prompt, string pattern, string errorMessage)
    {
        while (true)
        {
            Console.Write(prompt);
            string value = (Console.ReadLine() ?? string.Empty).Trim();

            if (Regex.IsMatch(value, pattern))
                return value;

            Console.WriteLine(errorMessage);
        }
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
        Console.WriteLine();
        Console.WriteLine($"Тип: {person.GetType().Name}");
        Console.WriteLine($"Прізвище: {person.LastName}");
        Console.WriteLine($"Ім'я: {person.FirstName}");
        Console.WriteLine($"Ідентифікатор: {person.UniqueId}");

        if (person is Student student)
        {
            Console.WriteLine($"Курс: {student.Course}");
            Console.WriteLine($"Дата народження: {student.BirthDate:dd.MM.yyyy}");
        }
    }
}
