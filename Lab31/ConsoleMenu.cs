using System;
using System.Text.RegularExpressions;

class ConsoleMenu
{
    private FileManager fileManager;

    public ConsoleMenu(FileManager fileManager)
    {
        this.fileManager = fileManager;
    }

    public void Start()
    {
        int choice;

        do
        {
            Console.WriteLine();
            Console.WriteLine("1 - Додати студента");
            Console.WriteLine("2 - Показати всіх студентів");
            Console.WriteLine(
                "3 - Студенти 4 курсу, які народилися навесні");
            Console.WriteLine("0 - Вихід");

            Console.Write("Оберіть пункт: ");

            choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    AddStudent();
                    break;

                case 2:
                    ShowStudents();
                    break;

                case 3:
                    ShowFourthCourseSpringStudents();
                    break;
            }

        } while (choice != 0);
    }

    private void AddStudent()
    {
        Console.Write("Ім'я: ");
        string firstName = Console.ReadLine();

        Console.Write("Прізвище: ");
        string lastName = Console.ReadLine();

        Console.Write("Курс: ");
        int course = int.Parse(Console.ReadLine());

        Console.Write("Студентський квиток: ");
        string studentId = Console.ReadLine();

        Console.Write(
            "Дата народження (XX.XX.XXXX): ");

        string birthDate = Console.ReadLine();

        if (!Regex.IsMatch(
            birthDate,
            @"^\d{2}\.\d{2}\.\d{4}$"))
        {
            Console.WriteLine(
                "Неправильний формат дати.");

            return;
        }

        Student student = new Student(
            firstName,
            lastName,
            course,
            studentId,
            birthDate);

        fileManager.WriteStudent(student);

        Console.WriteLine("Дані записано у файл.");
    }

    private void ShowStudents()
    {
        Student[] students =
            fileManager.ReadStudents();

        for (int i = 0;
             i < students.Length;
             i++)
        {
            PrintStudent(students[i]);
        }
    }

    private void ShowFourthCourseSpringStudents()
    {
        Student[] students =
            fileManager.ReadStudents();

        int count = 0;

        Console.WriteLine();
        Console.WriteLine(
            "Студенти 4 курсу, народжені навесні:");

        for (int i = 0;
             i < students.Length;
             i++)
        {
            Student student = students[i];

            string[] date =
                student.BirthDate.Split('.');

            int month = int.Parse(date[1]);

            if (student.Course == 4 &&
                month >= 3 &&
                month <= 5)
            {
                PrintStudent(student);
                count++;
            }
        }

        Console.WriteLine();
        Console.WriteLine(
            "Кількість студентів: " + count);
    }

    private void PrintStudent(Student student)
    {
        Console.WriteLine();
        Console.WriteLine(
            "Прізвище: " + student.LastName);

        Console.WriteLine(
            "Ім'я: " + student.FirstName);

        Console.WriteLine(
            "Курс: " + student.Course);

        Console.WriteLine(
            "Студентський квиток: "
            + student.StudentId);

        Console.WriteLine(
            "Дата народження: "
            + student.BirthDate);
    }
}