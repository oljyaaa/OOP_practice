namespace Lab3.Business.Models;

// BLL model: distinct from Lab3.Data.Entities.StudentEntity.
public sealed class Student
{
    public string Surname { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public int Course { get; init; }
    public string StudentCardNumber { get; init; } = string.Empty;
    public DateTime BirthDate { get; init; }

    public string FullName => $"{Surname} {FirstName}";
    public bool WasBornInSpring => BirthDate.Month is 3 or 4 or 5;
}
