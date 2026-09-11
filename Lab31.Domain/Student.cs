using System.Globalization;

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

    public override PersonRecord ToRecord()
    {
        return new PersonRecord(
            "Student",
            $"{FirstName}{LastName}",
            ["firstname", "lastname", "course", "studentId", "birthDate"],
            [FirstName, LastName, Course.ToString(CultureInfo.InvariantCulture), StudentId, BirthDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)]);
    }
}

public sealed class StudentFactory : IPersonFactory
{
    public string RecordTypeName => "Student";

    public Person Create(PersonRecord record)
    {
        return new Student(
            record.RequireValue("firstname"),
            record.RequireValue("lastname"),
            int.Parse(record.RequireValue("course"), CultureInfo.InvariantCulture),
            record.RequireValue("studentId"),
            DateTime.ParseExact(record.RequireValue("birthDate"), "dd.MM.yyyy", CultureInfo.InvariantCulture));
    }
}
