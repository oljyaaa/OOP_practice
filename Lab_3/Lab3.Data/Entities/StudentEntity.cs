namespace Lab3.Data.Entities;

// Entity exists only in DAL. BLL and PL use their own models.
public sealed class StudentEntity
{
    public string Surname { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public int Course { get; set; }
    public string StudentCardNumber { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
}
