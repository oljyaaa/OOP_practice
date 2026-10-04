using System.Globalization;

namespace Lab3.Data.Entities;

// Сутність "Студент" (частина 2, варіант 8).
// Дата народження зберігається у файлі у форматі ХХ.ХХ.ХХХХ (дд.ММ.рррр).
public class StudentEntity : IEntity
{
    public string Surname { get; set; } = "";
    public string FirstName { get; set; } = "";
    public int Course { get; set; }
    public string StudentCard { get; set; } = "";
    public string BirthDate { get; set; } = "";

    // Повертає поля студента для бінарного та користувацького формату
    public Dictionary<string, string> ToFields()
    {
        Dictionary<string, string> fields = new Dictionary<string, string>();
        fields["Surname"] = Surname;
        fields["FirstName"] = FirstName;
        fields["Course"] = Course.ToString();
        fields["StudentCard"] = StudentCard;
        fields["BirthDate"] = BirthDate;
        return fields;
    }

    // Відновлює поля студента зі словника
    public void FromFields(Dictionary<string, string> fields)
    {
        Surname = fields["Surname"];
        FirstName = fields["FirstName"];
        Course = int.Parse(fields["Course"]);
        StudentCard = fields["StudentCard"];
        BirthDate = fields["BirthDate"];
    }

    // Студент коректний, якщо заповнені ім'я, прізвище, квиток і дата має формат дд.ММ.рррр
    public bool IsValid()
    {
        DateTime date;
        bool isDateCorrect = DateTime.TryParseExact(BirthDate, "dd.MM.yyyy", CultureInfo.InvariantCulture,
            DateTimeStyles.None, out date);
        return Surname != "" && FirstName != "" && StudentCard != "" && isDateCorrect;
    }
}
