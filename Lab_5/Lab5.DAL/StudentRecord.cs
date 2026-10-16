namespace Lab5.DAL;

/// <summary>Сирі дані студента у тому вигляді, як вони зберігаються у файлі.</summary>
public class StudentRecord
{
    public string LastName { get; set; } = "";
    public string FirstName { get; set; } = "";
    public int Course { get; set; }
    public string StudentCard { get; set; } = "";
    public string BirthDate { get; set; } = "";
}
