namespace Lab3.Business.Models;

// Студент (BLL-модель, окрема від StudentEntity з DAL)
public class Student : Person
{
    public int Course { get; set; }
    public string StudentCard { get; set; }
    public DateTime BirthDate { get; set; }

    // Створює студента з усіма даними
    public Student(string surname, string firstName, int course, string studentCard, DateTime birthDate)
        : base(surname, firstName)
    {
        Course = course;
        StudentCard = studentCard;
        BirthDate = birthDate;
    }

    // Перевіряє, чи народився студент навесні (березень, квітень, травень)
    public bool IsBornInSpring()
    {
        return BirthDate.Month >= 3 && BirthDate.Month <= 5;
    }

    // Повертає інформацію про студента
    public override string GetInfo()
    {
        return Surname + " " + FirstName + ", курс " + Course + ", квиток " + StudentCard
            + ", дата народження " + BirthDate.ToString("dd.MM.yyyy");
    }
}
