using System.Text;
using Lab5.BLL;
using Lab5.DAL;

namespace Lab5.PL;

internal class Program
{
    /// <summary>Точка входу: створює сервіс з файловим сховищем і запускає меню.</summary>
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        var service = new StudentService(new JsonStudentRepository("students.json"));

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("1 - Показати всіх студентів");
            Console.WriteLine("2 - Студенти 4-го курсу, народжені навесні");
            Console.WriteLine("3 - Додати студента");
            Console.WriteLine("4 - Перевірити день народження (збільшення віку)");
            Console.WriteLine("5 - Пекар і підприємець (тістечка, парашут)");
            Console.WriteLine("0 - Вихід");
            Console.Write("Вибір: ");

            string? choice = Console.ReadLine();
            if (choice == null || choice == "0")
            {
                return;
            }

            try
            {
                switch (choice)
                {
                    case "1": ShowAll(service); break;
                    case "2": ShowSpringStudents(service); break;
                    case "3": AddStudent(service); break;
                    case "4": CheckBirthday(service); break;
                    case "5": ShowOtherEntities(); break;
                    default: Console.WriteLine("Невідомий пункт."); break;
                }
            }
            catch (Exception ex) when (ex is ArgumentException or FormatException or InvalidOperationException)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }
    }

    /// <summary>Виводить усіх студентів з файлу.</summary>
    private static void ShowAll(StudentService service)
    {
        foreach (Student s in service.GetAll(DateTime.Today))
        {
            Console.WriteLine(s);
        }
    }

    /// <summary>Виводить кількість і дані студентів 4-го курсу, народжених навесні.</summary>
    private static void ShowSpringStudents(StudentService service)
    {
        List<Student> students = service.GetFourthCourseSpringStudents(DateTime.Today);
        Console.WriteLine($"Кількість: {students.Count}");
        foreach (Student s in students)
        {
            Console.WriteLine(s);
        }
    }

    /// <summary>Зчитує дані з клавіатури, створює студента і зберігає його у файл.</summary>
    private static void AddStudent(StudentService service)
    {
        string lastName = Ask("Прізвище");
        string firstName = Ask("Ім'я");
        if (!int.TryParse(Ask("Курс"), out int course))
        {
            throw new FormatException("Курс має бути числом.");
        }

        string card = Ask("Студентський квиток");
        string birthDate = Ask("Дата народження (ДД.ММ.РРРР)");

        service.Add(new Student(lastName, firstName, course, card, birthDate, DateTime.Today));
        Console.WriteLine("Студента додано.");
    }

    /// <summary>Для кожного студента перевіряє введену дату і збільшує вік, якщо це день народження.</summary>
    private static void CheckBirthday(StudentService service)
    {
        DateTime day = Student.ParseBirthDate(Ask("Дата (ДД.ММ.РРРР)"));
        foreach (Student s in service.GetAll(day.AddDays(-1)))
        {
            if (s.CelebrateBirthday(day))
            {
                Console.WriteLine($"З днем народження, {s.FirstName}! Тепер вік: {s.Age}");
            }
        }
    }

    /// <summary>Демонструє додаткові сутності: пекаря та підприємця.</summary>
    private static void ShowOtherEntities()
    {
        var baker = new Baker("Петренко", "Оксана");
        var entrepreneur = new Entrepreneur("Іваненко", "Дмитро", "Солодка лавка");

        Console.WriteLine(baker.MakeCake("борошно", "цукор", "вершки"));
        Console.WriteLine(entrepreneur.MakeCake("шоколад", "яйця", "масло"));

        // Усі вміють стрибати з парашутом через спільний інтерфейс ISkydiver.
        ISkydiver[] skydivers = { baker, entrepreneur, new Student("Коваль", "Андрій", 4, "КВ1002", "02.03.2004", DateTime.Today) };
        foreach (ISkydiver skydiver in skydivers)
        {
            Console.WriteLine(skydiver.JumpWithParachute());
        }
    }

    /// <summary>Виводить підказку і повертає введений рядок.</summary>
    private static string Ask(string prompt)
    {
        Console.Write($"{prompt}: ");
        return Console.ReadLine() ?? "";
    }
}
