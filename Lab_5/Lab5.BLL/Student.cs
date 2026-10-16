using System.Globalization;

namespace Lab5.BLL;

/// <summary>Студент: прізвище, ім'я, курс, студентський квиток, дата народження (ДД.ММ.РРРР).</summary>
public class Student : Person
{
    public const string DateFormat = "dd.MM.yyyy";

    public int Course { get; }
    public string StudentCard { get; }
    public DateTime BirthDate { get; }
    public int Age { get; private set; }

    /// <summary>Створює студента, перевіряє всі поля та обчислює вік на дату today.</summary>
    public Student(string lastName, string firstName, int course, string studentCard, string birthDate, DateTime today)
        : base(lastName, firstName)
    {
        if (course < 1 || course > 6)
        {
            throw new ArgumentException("Курс має бути від 1 до 6.");
        }

        if (string.IsNullOrWhiteSpace(studentCard))
        {
            throw new ArgumentException("Студентський квиток не може бути порожнім.");
        }

        Course = course;
        StudentCard = studentCard.Trim();
        BirthDate = ParseBirthDate(birthDate);

        if (BirthDate > today)
        {
            throw new ArgumentException("Дата народження не може бути в майбутньому.");
        }

        Age = CalculateAge(BirthDate, today);
    }

    /// <summary>Перетворює рядок формату ДД.ММ.РРРР у дату або кидає виняток.</summary>
    public static DateTime ParseBirthDate(string text)
    {
        if (!DateTime.TryParseExact(text, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
        {
            throw new FormatException("Дата народження має бути у форматі ДД.ММ.РРРР.");
        }

        return date;
    }

    /// <summary>Рахує кількість повних років на дату today.</summary>
    public static int CalculateAge(DateTime birthDate, DateTime today)
    {
        int age = today.Year - birthDate.Year;
        if (today.Month < birthDate.Month || (today.Month == birthDate.Month && today.Day < birthDate.Day))
        {
            age--;
        }

        return age;
    }

    /// <summary>Перевіряє, чи народився студент навесні (березень–травень).</summary>
    public bool IsBornInSpring() => BirthDate.Month >= 3 && BirthDate.Month <= 5;

    /// <summary>Перевіряє, чи дата day є днем народження студента.</summary>
    public bool IsBirthday(DateTime day) => day.Month == BirthDate.Month && day.Day == BirthDate.Day;

    /// <summary>Якщо настав день народження — збільшує вік на 1 і повертає true.</summary>
    public bool CelebrateBirthday(DateTime day)
    {
        if (!IsBirthday(day))
        {
            return false;
        }

        Age++;
        return true;
    }

    /// <summary>Повертає дані студента одним рядком.</summary>
    public override string ToString() =>
        $"{LastName} {FirstName}, {Course} курс, квиток {StudentCard}, нар. {BirthDate.ToString(DateFormat)}, вік {Age}";
}
