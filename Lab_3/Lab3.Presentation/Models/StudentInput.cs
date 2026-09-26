namespace Lab3.Presentation.Models;

// PL model: it is intentionally different from the BLL and DAL student models.
public sealed class StudentInput
{
    public string Surname { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string CourseText { get; init; } = string.Empty;
    public string StudentCardNumber { get; init; } = string.Empty;
    public string BirthDateText { get; init; } = string.Empty;
}
