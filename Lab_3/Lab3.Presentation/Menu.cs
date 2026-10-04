using System.Globalization;
using System.Text;
using Lab3.Business;
using Lab3.Business.Exceptions;
using Lab3.Business.Models;
using Lab3.Presentation.Models;

namespace Lab3.Presentation;

// Найвищий рівень (PL): введення і виведення даних через консоль.
// Menu викликає методи EntityService і передає туди дані користувача.
public static class Menu
{
    private static readonly EntityService service = new EntityService();

    // Головне меню програми
    public static void MainMenu()
    {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=== Лабораторна робота 3.3, варіант 8 ===");
            Console.WriteLine("1 - Частина 1: рядки і серіалізація");
            Console.WriteLine("2 - Студенти");
            Console.WriteLine("3 - Пекарі");
            Console.WriteLine("4 - Підприємці");
            Console.WriteLine("5 - Створення та видалення файлів");
            Console.WriteLine("0 - Вихід");
            string choice = ReadText("Ваш вибір");

            if (choice == "0")
            {
                return;
            }

            // Усі винятки обробляються тут, а не в місці їх виникнення
            try
            {
                switch (choice)
                {
                    case "1": StringMenu(); break;
                    case "2": StudentMenu(); break;
                    case "3": BakerMenu(); break;
                    case "4": EntrepreneurMenu(); break;
                    case "5": FileMenu(); break;
                    default: Console.WriteLine("Немає такого пункту."); break;
                }
            }
            catch (EntityValidationException exception)
            {
                Console.WriteLine("Помилка введення: " + exception.Message);
            }
            catch (EntityNotFoundException exception)
            {
                Console.WriteLine("Не знайдено: " + exception.Message);
            }
            catch (StorageOperationException exception)
            {
                Console.WriteLine("Помилка файлу: " + exception.Message + " " + exception.InnerException?.Message);
            }
        }
    }

    // ===================== Частина 1: рядки =====================

    // Меню роботи з рядками та серіалізацією масиву і колекції
    private static void StringMenu()
    {
        Console.WriteLine();
        Console.WriteLine("--- Рядки ---");
        Console.WriteLine("1 - Вивести рядки");
        Console.WriteLine("2 - Додати рядок");
        Console.WriteLine("3 - Знайти символ у рядку");
        Console.WriteLine("4 - Змінити порядок символів на протилежний");
        Console.WriteLine("5 - Додати новий рядок до існуючого");
        Console.WriteLine("6 - Серіалізувати масив рядків у файл");
        Console.WriteLine("7 - Відновити рядки з файлу в новий масив");
        Console.WriteLine("8 - Серіалізувати колекцію рядків у файл");
        Console.WriteLine("9 - Відновити колекцію рядків з файлу");
        Console.WriteLine("10 - Порівняти серіалізацію масиву і колекції");
        string choice = ReadText("Ваш вибір");

        switch (choice)
        {
            case "1":
                PrintStrings(service.GetStrings().ToArray());
                break;
            case "2":
                service.AddString(ReadText("Новий рядок"));
                Console.WriteLine("Рядок додано.");
                break;
            case "3":
                int index = ReadNumber("Номер рядка (з 0)");
                char symbol = ReadSymbol();
                int position = service.FindSymbol(index, symbol);
                if (position >= 0)
                {
                    Console.WriteLine("Символ '" + symbol + "' знайдено на позиції " + position + ".");
                }
                else
                {
                    Console.WriteLine("Символ '" + symbol + "' не знайдено.");
                }
                break;
            case "4":
                service.ReverseString(ReadNumber("Номер рядка (з 0)"));
                Console.WriteLine("Рядок розвернуто.");
                break;
            case "5":
                int stringIndex = ReadNumber("Номер рядка (з 0)");
                service.AppendToString(stringIndex, ReadText("Текст, який треба додати"));
                Console.WriteLine("Текст додано.");
                break;
            case "6":
                service.SaveStringsAsArray(ReadStorageOptions());
                Console.WriteLine("Масив серіалізовано.");
                break;
            case "7":
                MyString[] newArray = service.LoadStringsAsArray(ReadStorageOptions());
                Console.WriteLine("Новий масив, відновлений з файлу:");
                PrintStrings(newArray);
                break;
            case "8":
                service.SaveStringsAsCollection(ReadStorageOptions());
                Console.WriteLine("Колекцію серіалізовано.");
                break;
            case "9":
                service.LoadStringsAsCollection(ReadStorageOptions());
                Console.WriteLine("Колекцію відновлено з файлу:");
                PrintStrings(service.GetStrings().ToArray());
                break;
            case "10":
                CompareArrayWithCollection();
                break;
            default:
                Console.WriteLine("Немає такого пункту.");
                break;
        }
    }

    // Порівнює масив і колекцію після серіалізації та десеріалізації
    private static void CompareArrayWithCollection()
    {
        FileFormat format = ReadFormat();
        string arrayFile = ReadText("Файл для масиву");
        string collectionFile = ReadText("Файл для колекції");

        ComparisonResult result = service.CompareArrayWithCollection(
            new StorageOptions(arrayFile, format), new StorageOptions(collectionFile, format));

        Console.WriteLine("Масив (MyString[]), відновлений з файлу:");
        PrintStrings(result.RestoredArray);
        Console.WriteLine("Колекція (List<MyString>), відновлена з файлу:");
        PrintStrings(result.RestoredCollection.ToArray());

        if (result.AreEqual)
        {
            Console.WriteLine("Результат: дані однакові. Масив має фіксований розмір, а колекцію можна змінювати (Add/Remove).");
        }
        else
        {
            Console.WriteLine("Результат: дані відрізняються.");
        }
    }

    // Виводить рядки з їхніми номерами
    private static void PrintStrings(MyString[] items)
    {
        for (int i = 0; i < items.Length; i++)
        {
            Console.WriteLine("[" + i + "] " + items[i].GetInfo());
        }
    }

    // ===================== Частина 2: студенти =====================

    // Меню роботи зі студентами
    private static void StudentMenu()
    {
        Console.WriteLine();
        Console.WriteLine("--- Студенти ---");
        Console.WriteLine("1 - Вивести всіх");
        Console.WriteLine("2 - Додати студента");
        Console.WriteLine("3 - Знайти за студентським квитком");
        Console.WriteLine("4 - Видалити за студентським квитком");
        Console.WriteLine("5 - Зберегти у файл");
        Console.WriteLine("6 - Прочитати з файлу");
        Console.WriteLine("7 - Кількість студентів 4-го курсу, народжених навесні (з файлу)");
        Console.WriteLine("8 - Стрибнути з парашутом");
        string choice = ReadText("Ваш вибір");

        switch (choice)
        {
            case "1":
                PrintPeople(service.GetStudents().ToArray());
                break;
            case "2":
                service.AddStudent(ToStudent(ReadStudentInput()));
                Console.WriteLine("Студента додано.");
                break;
            case "3":
                Console.WriteLine(service.FindStudent(ReadText("Студентський квиток")).GetInfo());
                break;
            case "4":
                service.RemoveStudent(ReadText("Студентський квиток"));
                Console.WriteLine("Студента видалено.");
                break;
            case "5":
                service.SaveStudents(ReadStorageOptions());
                Console.WriteLine("Студентів збережено.");
                break;
            case "6":
                service.LoadStudents(ReadStorageOptions());
                Console.WriteLine("Студентів прочитано з файлу.");
                break;
            case "7":
                List<Student> springStudents = service.GetSpringStudentsOfFourthCourse(ReadStorageOptions());
                Console.WriteLine("Кількість студентів 4-го курсу, які народилися навесні: " + springStudents.Count);
                PrintPeople(springStudents.ToArray());
                break;
            case "8":
                Console.WriteLine(service.FindStudent(ReadText("Студентський квиток")).JumpWithParachute());
                break;
            default:
                Console.WriteLine("Немає такого пункту.");
                break;
        }
    }

    // Зчитує дані студента з консолі
    private static StudentInput ReadStudentInput()
    {
        StudentInput input = new StudentInput();
        input.Surname = ReadText("Прізвище");
        input.FirstName = ReadText("Ім'я");
        input.Course = ReadText("Курс");
        input.StudentCard = ReadText("Студентський квиток");
        input.BirthDate = ReadText("Дата народження (ХХ.ХХ.ХХХХ)");
        return input;
    }

    // Перетворює введені дані на BLL-модель студента (перевіряє числа та дату)
    private static Student ToStudent(StudentInput input)
    {
        int course;
        if (!int.TryParse(input.Course, out course))
        {
            throw new EntityValidationException("Курс має бути числом.");
        }

        DateTime birthDate;
        if (!DateTime.TryParseExact(input.BirthDate, "dd.MM.yyyy", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out birthDate))
        {
            throw new EntityValidationException("Дата має бути у форматі ХХ.ХХ.ХХХХ, наприклад 15.04.2004.");
        }

        return new Student(input.Surname, input.FirstName, course, input.StudentCard, birthDate);
    }

    // ===================== Частина 2: пекарі =====================

    // Меню роботи з пекарями
    private static void BakerMenu()
    {
        Console.WriteLine();
        Console.WriteLine("--- Пекарі ---");
        Console.WriteLine("1 - Вивести всіх");
        Console.WriteLine("2 - Додати пекаря");
        Console.WriteLine("3 - Знайти за прізвищем");
        Console.WriteLine("4 - Видалити за прізвищем");
        Console.WriteLine("5 - Зберегти у файл");
        Console.WriteLine("6 - Прочитати з файлу");
        Console.WriteLine("7 - Стрибнути з парашутом");
        string choice = ReadText("Ваш вибір");

        switch (choice)
        {
            case "1":
                PrintPeople(service.GetBakers().ToArray());
                break;
            case "2":
                BakerInput input = new BakerInput();
                input.Surname = ReadText("Прізвище");
                input.FirstName = ReadText("Ім'я");
                input.Bakery = ReadText("Пекарня");
                service.AddBaker(new Baker(input.Surname, input.FirstName, input.Bakery));
                Console.WriteLine("Пекаря додано.");
                break;
            case "3":
                Console.WriteLine(service.FindBaker(ReadText("Прізвище")).GetInfo());
                break;
            case "4":
                service.RemoveBaker(ReadText("Прізвище"));
                Console.WriteLine("Пекаря видалено.");
                break;
            case "5":
                service.SaveBakers(ReadStorageOptions());
                Console.WriteLine("Пекарів збережено.");
                break;
            case "6":
                service.LoadBakers(ReadStorageOptions());
                Console.WriteLine("Пекарів прочитано з файлу.");
                break;
            case "7":
                Console.WriteLine(service.FindBaker(ReadText("Прізвище")).JumpWithParachute());
                break;
            default:
                Console.WriteLine("Немає такого пункту.");
                break;
        }
    }

    // ===================== Частина 2: підприємці =====================

    // Меню роботи з підприємцями
    private static void EntrepreneurMenu()
    {
        Console.WriteLine();
        Console.WriteLine("--- Підприємці ---");
        Console.WriteLine("1 - Вивести всіх");
        Console.WriteLine("2 - Додати підприємця");
        Console.WriteLine("3 - Знайти за прізвищем");
        Console.WriteLine("4 - Видалити за прізвищем");
        Console.WriteLine("5 - Зберегти у файл");
        Console.WriteLine("6 - Прочитати з файлу");
        Console.WriteLine("7 - Стрибнути з парашутом");
        string choice = ReadText("Ваш вибір");

        switch (choice)
        {
            case "1":
                PrintPeople(service.GetEntrepreneurs().ToArray());
                break;
            case "2":
                EntrepreneurInput input = new EntrepreneurInput();
                input.Surname = ReadText("Прізвище");
                input.FirstName = ReadText("Ім'я");
                input.Business = ReadText("Бізнес");
                service.AddEntrepreneur(new Entrepreneur(input.Surname, input.FirstName, input.Business));
                Console.WriteLine("Підприємця додано.");
                break;
            case "3":
                Console.WriteLine(service.FindEntrepreneur(ReadText("Прізвище")).GetInfo());
                break;
            case "4":
                service.RemoveEntrepreneur(ReadText("Прізвище"));
                Console.WriteLine("Підприємця видалено.");
                break;
            case "5":
                service.SaveEntrepreneurs(ReadStorageOptions());
                Console.WriteLine("Підприємців збережено.");
                break;
            case "6":
                service.LoadEntrepreneurs(ReadStorageOptions());
                Console.WriteLine("Підприємців прочитано з файлу.");
                break;
            case "7":
                Console.WriteLine(service.FindEntrepreneur(ReadText("Прізвище")).JumpWithParachute());
                break;
            default:
                Console.WriteLine("Немає такого пункту.");
                break;
        }
    }

    // ===================== Файли =====================

    // Меню створення порожнього файлу та видалення файлу
    private static void FileMenu()
    {
        Console.WriteLine();
        Console.WriteLine("--- Файли ---");
        Console.WriteLine("1 - Створити порожній файл");
        Console.WriteLine("2 - Видалити файл");
        string choice = ReadText("Ваш вибір");

        switch (choice)
        {
            case "1":
                Console.WriteLine("Сутність: 1 - студент, 2 - пекар, 3 - підприємець, 4 - рядок");
                int kind = ReadNumber("Номер сутності");
                if (kind < 1 || kind > 4)
                {
                    throw new EntityValidationException("Номер сутності має бути від 1 до 4.");
                }
                service.CreateEmptyFile((EntityKind)kind, ReadStorageOptions());
                Console.WriteLine("Порожній файл створено.");
                break;
            case "2":
                service.DeleteFile(ReadText("Ім'я файлу"));
                Console.WriteLine("Файл видалено.");
                break;
            default:
                Console.WriteLine("Немає такого пункту.");
                break;
        }
    }

    // ===================== Допоміжні методи введення/виведення =====================

    // Виводить інформацію про список людей (студентів, пекарів або підприємців)
    private static void PrintPeople(Person[] people)
    {
        if (people.Length == 0)
        {
            Console.WriteLine("Список порожній.");
        }
        foreach (Person person in people)
        {
            Console.WriteLine(person.GetInfo());
        }
    }

    // Зчитує формат та ім'я файлу, які користувач обирає для серіалізації
    private static StorageOptions ReadStorageOptions()
    {
        FileFormat format = ReadFormat();
        string filePath = ReadText("Ім'я файлу");
        return new StorageOptions(filePath, format);
    }

    // Зчитує формат серіалізації
    private static FileFormat ReadFormat()
    {
        Console.WriteLine("Формат: 1 - Binary, 2 - XML, 3 - JSON, 4 - Custom");
        int format = ReadNumber("Номер формату");
        if (format < 1 || format > 4)
        {
            throw new EntityValidationException("Номер формату має бути від 1 до 4.");
        }
        return (FileFormat)format;
    }

    // Зчитує один символ
    private static char ReadSymbol()
    {
        string text = ReadText("Символ");
        if (text.Length != 1)
        {
            throw new EntityValidationException("Потрібно ввести рівно один символ.");
        }
        return text[0];
    }

    // Зчитує ціле число
    private static int ReadNumber(string caption)
    {
        int number;
        if (!int.TryParse(ReadText(caption), out number))
        {
            throw new EntityValidationException("Потрібно ввести ціле число.");
        }
        return number;
    }

    // Виводить підказку і зчитує рядок тексту з консолі
    private static string ReadText(string caption)
    {
        Console.Write(caption + ": ");
        string? text = Console.ReadLine();
        if (text == null)
        {
            return "0";
        }
        return text.Trim();
    }
}
