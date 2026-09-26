using System.Globalization;
using Lab3.Business;
using Lab3.Business.Exceptions;
using Lab3.Business.Models;
using Lab3.Presentation.Models;

namespace Lab3.Presentation;

public sealed class Menu
{
    private readonly EntityService _service;

    private Menu(EntityService service) => _service = service;

    public static void MainMenu() => new Menu(EntityService.CreateDefault()).Run();

    private void Run()
    {
        while (true)
        {
            Console.WriteLine("\n=== Лабораторна робота 3.3, варіант 8 ===");
            Console.WriteLine("1 - Рядки: частина 1 (серіалізація)");
            Console.WriteLine("2 - Студенти");
            Console.WriteLine("3 - Пекарі");
            Console.WriteLine("4 - Підприємці");
            Console.WriteLine("5 - Операції з файлами");
            Console.WriteLine("0 - Вихід");
            Console.Write("Ваш вибір: ");
            switch (Console.ReadLine())
            {
                case "1": Execute(StringMenu); break;
                case "2": Execute(StudentMenu); break;
                case "3": Execute(BakerMenu); break;
                case "4": Execute(EntrepreneurMenu); break;
                case "5": Execute(FileMenu); break;
                case "0": return;
                case null: return;
                default: Console.WriteLine("Оберіть пункт від 0 до 5."); break;
            }
        }
    }

    private void StringMenu()
    {
        while (true)
        {
            Console.WriteLine("\n--- Рядок, варіант 8 ---");
            Console.WriteLine("1 - Показати рядки");
            Console.WriteLine("2 - Додати рядок");
            Console.WriteLine("3 - Знайти символ");
            Console.WriteLine("4 - Розвернути рядок");
            Console.WriteLine("5 - Додати текст до рядка");
            Console.WriteLine("6 - Зберегти масив рядків");
            Console.WriteLine("7 - Відновити новий масив рядків");
            Console.WriteLine("8 - Зберегти колекцію рядків");
            Console.WriteLine("9 - Відновити колекцію рядків");
            Console.WriteLine("0 - Назад");
            Console.Write("Ваш вибір: ");
            switch (Console.ReadLine())
            {
                case "1": PrintStrings(_service.Strings); break;
                case "2": _service.AddString(ReadString().Value); break;
                case "3": FindCharacter(); break;
                case "4": _service.ReverseString(ReadIndex()); break;
                case "5": _service.AppendToString(ReadIndex(), ReadRequired("Текст для додавання")); break;
                case "6": _service.SaveStringsAsArray(ReadStorageOptions()); Console.WriteLine("Масив збережено."); break;
                case "7": PrintStrings(_service.LoadStringsAsArray(ReadStorageOptions())); break;
                case "8": _service.SaveStringsAsCollection(ReadStorageOptions()); Console.WriteLine("Колекцію збережено."); break;
                case "9": _service.LoadStringsAsCollection(ReadStorageOptions()); Console.WriteLine("Колекцію відновлено."); break;
                case "0": return;
                case null: return;
                default: Console.WriteLine("Оберіть пункт від 0 до 9."); break;
            }
        }
    }

    private void StudentMenu()
    {
        while (true)
        {
            Console.WriteLine("\n--- Студенти ---");
            Console.WriteLine("1 - Показати всіх");
            Console.WriteLine("2 - Додати");
            Console.WriteLine("3 - Знайти за студентським квитком");
            Console.WriteLine("4 - Видалити за студентським квитком");
            Console.WriteLine("5 - Зберегти у файл");
            Console.WriteLine("6 - Прочитати з файлу");
            Console.WriteLine("7 - Порахувати студентів 4 курсу, народжених навесні");
            Console.WriteLine("0 - Назад");
            Console.Write("Ваш вибір: ");
            switch (Console.ReadLine())
            {
                case "1": PrintStudents(_service.Students); break;
                case "2": _service.AddStudent(ReadStudent()); Console.WriteLine("Студента додано."); break;
                case "3": PrintStudents([_service.FindStudent(ReadRequired("Номер квитка"))]); break;
                case "4": _service.RemoveStudent(ReadRequired("Номер квитка")); Console.WriteLine("Студента видалено."); break;
                case "5": _service.SaveStudents(ReadStorageOptions()); Console.WriteLine("Студентів збережено."); break;
                case "6": _service.LoadStudents(ReadStorageOptions()); Console.WriteLine("Студентів прочитано з файлу."); break;
                case "7": PrintSpringStudents(); break;
                case "0": return;
                case null: return;
                default: Console.WriteLine("Оберіть пункт від 0 до 7."); break;
            }
        }
    }

    private void BakerMenu()
    {
        while (true)
        {
            Console.WriteLine("\n--- Пекарі ---");
            Console.WriteLine("1 - Показати всіх; 2 - Додати; 3 - Знайти; 4 - Видалити");
            Console.WriteLine("5 - Стрибнути з парашутом; 6 - Зберегти; 7 - Прочитати; 0 - Назад");
            Console.Write("Ваш вибір: ");
            switch (Console.ReadLine())
            {
                case "1": PrintBakers(); break;
                case "2": _service.AddBaker(ReadBaker()); Console.WriteLine("Пекаря додано."); break;
                case "3": Console.WriteLine(_service.FindBaker(ReadRequired("Прізвище")).JumpWithParachute()); break;
                case "4": _service.RemoveBaker(ReadRequired("Прізвище")); Console.WriteLine("Пекаря видалено."); break;
                case "5": Console.WriteLine(_service.FindBaker(ReadRequired("Прізвище")).JumpWithParachute()); break;
                case "6": _service.SaveBakers(ReadStorageOptions()); Console.WriteLine("Пекарів збережено."); break;
                case "7": _service.LoadBakers(ReadStorageOptions()); Console.WriteLine("Пекарів прочитано з файлу."); break;
                case "0": return;
                case null: return;
                default: Console.WriteLine("Оберіть пункт від 0 до 7."); break;
            }
        }
    }

    private void EntrepreneurMenu()
    {
        while (true)
        {
            Console.WriteLine("\n--- Підприємці ---");
            Console.WriteLine("1 - Показати всіх; 2 - Додати; 3 - Знайти; 4 - Видалити");
            Console.WriteLine("5 - Стрибнути з парашутом; 6 - Зберегти; 7 - Прочитати; 0 - Назад");
            Console.Write("Ваш вибір: ");
            switch (Console.ReadLine())
            {
                case "1": PrintEntrepreneurs(); break;
                case "2": _service.AddEntrepreneur(ReadEntrepreneur()); Console.WriteLine("Підприємця додано."); break;
                case "3": Console.WriteLine(_service.FindEntrepreneur(ReadRequired("Прізвище")).JumpWithParachute()); break;
                case "4": _service.RemoveEntrepreneur(ReadRequired("Прізвище")); Console.WriteLine("Підприємця видалено."); break;
                case "5": Console.WriteLine(_service.FindEntrepreneur(ReadRequired("Прізвище")).JumpWithParachute()); break;
                case "6": _service.SaveEntrepreneurs(ReadStorageOptions()); Console.WriteLine("Підприємців збережено."); break;
                case "7": _service.LoadEntrepreneurs(ReadStorageOptions()); Console.WriteLine("Підприємців прочитано з файлу."); break;
                case "0": return;
                case null: return;
                default: Console.WriteLine("Оберіть пункт від 0 до 7."); break;
            }
        }
    }

    private void FileMenu()
    {
        while (true)
        {
            Console.WriteLine("\n--- Життєвий цикл файла ---");
            Console.WriteLine("1 - Створити порожній файл; 2 - Видалити файл; 0 - Назад");
            Console.Write("Ваш вибір: ");
            switch (Console.ReadLine())
            {
                case "1":
                    _service.CreateEmptyFile(ReadStoredEntityKind(), ReadStorageOptions());
                    Console.WriteLine("Порожній файл створено й закрито.");
                    break;
                case "2":
                    _service.DeleteFile(ReadRequired("Ім'я або повний шлях до файлу"));
                    Console.WriteLine("Файл видалено.");
                    break;
                case "0": return;
                case null: return;
                default: Console.WriteLine("Оберіть пункт 0, 1 або 2."); break;
            }
        }
    }

    private void FindCharacter()
    {
        var index = ReadIndex();
        var symbol = ReadRequired("Один символ");
        if (symbol.Length != 1) throw new EntityValidationException("Потрібно ввести рівно один символ.");
        var position = _service.FindCharacterInString(index, symbol[0]);
        Console.WriteLine(position >= 0 ? $"Символ знайдено за індексом {position}." : "Символ не знайдено.");
    }

    private void PrintSpringStudents()
    {
        var students = _service.GetFourthCourseSpringStudents();
        Console.WriteLine($"Знайдено: {students.Count}.");
        PrintStudents(students);
    }

    private static void PrintStrings(IEnumerable<LaboratoryString> strings)
    {
        foreach (var (item, index) in strings.Select((item, index) => (item, index)))
            Console.WriteLine($"[{index}] \"{item.Value}\" (довжина: {item.Length})");
    }

    private static void PrintStudents(IEnumerable<Student> students)
    {
        foreach (var student in students)
            Console.WriteLine($"{student.FullName}; курс {student.Course}; квиток {student.StudentCardNumber}; дата: {student.BirthDate:dd.MM.yyyy}");
    }

    private void PrintBakers()
    {
        foreach (var baker in _service.Bakers)
            Console.WriteLine($"{baker.Surname} {baker.FirstName}; пекарня: {baker.BakeryName}");
    }

    private void PrintEntrepreneurs()
    {
        foreach (var entrepreneur in _service.Entrepreneurs)
            Console.WriteLine($"{entrepreneur.Surname} {entrepreneur.FirstName}; бізнес: {entrepreneur.BusinessName}");
    }

    private static Student ReadStudent()
    {
        var input = new StudentInput
        {
            Surname = ReadRequired("Прізвище"), FirstName = ReadRequired("Ім'я"),
            CourseText = ReadRequired("Курс"), StudentCardNumber = ReadRequired("Номер студентського квитка"),
            BirthDateText = ReadRequired("Дата народження (дд.ММ.рррр)")
        };
        return new Student
        {
            Surname = input.Surname, FirstName = input.FirstName, StudentCardNumber = input.StudentCardNumber,
            Course = int.Parse(input.CourseText, CultureInfo.InvariantCulture),
            BirthDate = DateTime.ParseExact(input.BirthDateText, "dd.MM.yyyy", CultureInfo.InvariantCulture)
        };
    }

    private static Baker ReadBaker()
    {
        var input = new BakerInput { Surname = ReadRequired("Прізвище"), FirstName = ReadRequired("Ім'я"), BakeryName = ReadRequired("Назва пекарні") };
        return new Baker { Surname = input.Surname, FirstName = input.FirstName, BakeryName = input.BakeryName };
    }

    private static StringInput ReadString() => new() { Value = ReadRequired("Новий рядок") };

    private static Entrepreneur ReadEntrepreneur()
    {
        var input = new EntrepreneurInput { Surname = ReadRequired("Прізвище"), FirstName = ReadRequired("Ім'я"), BusinessName = ReadRequired("Назва бізнесу") };
        return new Entrepreneur { Surname = input.Surname, FirstName = input.FirstName, BusinessName = input.BusinessName };
    }

    private static StorageOptions ReadStorageOptions()
    {
        Console.WriteLine("Формат: 1 - binary, 2 - XML, 3 - JSON, 4 - custom.");
        var format = ReadRequired("Номер формату") switch
        {
            "1" => FileFormat.Binary, "2" => FileFormat.Xml, "3" => FileFormat.Json, "4" => FileFormat.Custom,
            _ => throw new EntityValidationException("Формат має бути від 1 до 4.")
        };
        return new StorageOptions(ReadRequired("Ім'я або повний шлях до файлу"), format);
    }

    private static StoredEntityKind ReadStoredEntityKind()
    {
        Console.WriteLine("Сутність: 1 - студент, 2 - пекар, 3 - підприємець, 4 - рядок.");
        return ReadRequired("Номер сутності") switch
        {
            "1" => StoredEntityKind.Student,
            "2" => StoredEntityKind.Baker,
            "3" => StoredEntityKind.Entrepreneur,
            "4" => StoredEntityKind.LaboratoryString,
            _ => throw new EntityValidationException("Тип сутності має бути від 1 до 4.")
        };
    }

    private static int ReadIndex() => int.Parse(ReadRequired("Індекс рядка (від 0)"), CultureInfo.InvariantCulture);

    private static string ReadRequired(string caption)
    {
        Console.Write($"{caption}: ");
        var value = Console.ReadLine()?.Trim();
        return !string.IsNullOrWhiteSpace(value) ? value : throw new EntityValidationException($"Поле «{caption}» обов'язкове.");
    }

    private static void Execute(Action action)
    {
        try { action(); }
        catch (EntityValidationException exception) { Console.WriteLine($"Помилка введення: {exception.Message}"); }
        catch (EntityNotFoundException exception) { Console.WriteLine($"Не знайдено: {exception.Message}"); }
        catch (StorageOperationException exception) { Console.WriteLine($"Помилка файлу: {exception.InnerException?.Message}"); }
        catch (FormatException) { Console.WriteLine("Перевірте формат введених чисел або дати."); }
        catch (Exception exception) { Console.WriteLine($"Непередбачена помилка: {exception.Message}"); }
    }
}
