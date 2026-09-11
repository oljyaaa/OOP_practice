namespace Lab31.Domain;

public sealed class Student : Person, IParachuteJump
{
    public int Course { get; }
    public string StudentId { get; }
    public DateTime BirthDate { get; }
    public bool IsStudying { get; private set; }
    public bool HasJumpedWithParachute { get; private set; }

    public override string UniqueId => StudentId;

    public Student(string firstName, string lastName, int course, string studentId, DateTime birthDate)
        : base(firstName, lastName)
    {
        Course = course;
        StudentId = studentId;
        BirthDate = birthDate;
    }

    public void Study()
    {
        IsStudying = true;
    }

    public void JumpWithParachute()
    {
        HasJumpedWithParachute = true;
    }
}
