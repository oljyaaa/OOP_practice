class Student : Person, IParachuteJump
{
    public int Course { get; set; }
    public string StudentId { get; set; }
    public string BirthDate { get; set; }

    public Student(
        string firstName,
        string lastName,
        int course,
        string studentId,
        string birthDate)
        : base(firstName, lastName)
    {
        Course = course;
        StudentId = studentId;
        BirthDate = birthDate;
    }

    public void Study()
    {
    }

    public void JumpWithParachute()
    {
    }
}