using System.Globalization;
using System.Text.RegularExpressions;

namespace Lab31;

public static class ConsoleInput
{
    private const string NamePattern = @"^[\p{L}][\p{L}'-]{1,49}$";
    private const string StudentIdPattern = @"^[A-Z]{3}-\d{6,7}$";
    private const string PersonIdPattern = @"^[A-Z]{3}-\d{3,7}$";

    public static string ReadName(string prompt)
    {
        return ReadByPattern(prompt, NamePattern, "Використовуйте лише літери, апостроф або дефіс.");
    }

    public static string ReadStudentId()
    {
        return ReadByPattern("Студентський квиток (ABC-123456): ", StudentIdPattern, "Формат: три великі літери, дефіс і 6-7 цифр.");
    }

    public static string ReadPersonId(string prompt)
    {
        return ReadByPattern(prompt, PersonIdPattern, "Формат: три великі літери, дефіс і 3-7 цифр.");
    }

    public static int ReadCourse()
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

    public static DateTime ReadBirthDate()
    {
        while (true)
        {
            string value = ReadByPattern("Дата народження (ДД.ММ.РРРР): ", @"^\d{2}\.\d{2}\.\d{4}$", "Формат: ДД.ММ.РРРР.");

            if (DateTime.TryParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthDate))
                return birthDate;

            Console.WriteLine("Такої календарної дати не існує.");
        }
    }

    public static string ReadRequiredText(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string value = (Console.ReadLine() ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(value))
                return value;

            Console.WriteLine("Введіть значення.");
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
}
