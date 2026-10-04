namespace Lab3.Presentation.Models;

// PL-модель студента: дані у тому вигляді, як їх ввів користувач (усе - текст)
public class StudentInput
{
    public string Surname { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string Course { get; set; } = "";
    public string StudentCard { get; set; } = "";
    public string BirthDate { get; set; } = "";
}
