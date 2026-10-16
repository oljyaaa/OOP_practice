using Lab5.BLL;

namespace Lab5.Tests;

[TestClass]
public class StudentTests
{
    private static readonly DateTime Today = new(2026, 10, 16);

    /// <summary>Створює коректного студента для тестів.</summary>
    private static Student Create(int course = 4, string birthDate = "15.04.2004") =>
        new("Шевченко", "Олена", course, "КВ1001", birthDate, Today);

    [TestMethod]
    public void Constructor_ValidData_SetsProperties()
    {
        // Arrange & Act
        Student student = Create();

        // Assert
        Assert.AreEqual("Шевченко", student.LastName);
        Assert.AreEqual("Олена", student.FirstName);
        Assert.AreEqual(4, student.Course);
        Assert.AreEqual("КВ1001", student.StudentCard);
        Assert.AreEqual(new DateTime(2004, 4, 15), student.BirthDate);
        Assert.AreEqual(22, student.Age);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(7)]
    public void Constructor_InvalidCourse_Throws(int course)
    {
        // Arrange, Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => Create(course: course));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("Олена1")]
    public void Constructor_InvalidName_Throws(string firstName)
    {
        // Arrange, Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => new Student("Шевченко", firstName, 4, "КВ1", "15.04.2004", Today));
    }

    [TestMethod]
    public void Constructor_EmptyStudentCard_Throws()
    {
        // Arrange, Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => new Student("Шевченко", "Олена", 4, " ", "15.04.2004", Today));
    }

    [TestMethod]
    [DataRow("2004-04-15")]
    [DataRow("31.02.2004")]
    [DataRow("abc")]
    public void Constructor_WrongDateFormat_Throws(string birthDate)
    {
        // Arrange, Act & Assert
        Assert.ThrowsExactly<FormatException>(() => Create(birthDate: birthDate));
    }

    [TestMethod]
    public void Constructor_FutureBirthDate_Throws()
    {
        // Arrange, Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => Create(birthDate: "01.01.2030"));
    }

    [TestMethod]
    public void CalculateAge_BeforeBirthdayThisYear_ReturnsOneLess()
    {
        // Arrange
        var birth = new DateTime(2004, 12, 1);

        // Act
        int age = Student.CalculateAge(birth, Today);

        // Assert
        Assert.AreEqual(21, age);
    }

    [TestMethod]
    [DataRow("01.03.2004", true)]
    [DataRow("31.05.2004", true)]
    [DataRow("28.02.2004", false)]
    [DataRow("01.06.2004", false)]
    public void IsBornInSpring_ChecksMonth(string birthDate, bool expected)
    {
        // Arrange
        Student student = Create(birthDate: birthDate);

        // Act
        bool result = student.IsBornInSpring();

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void CelebrateBirthday_OnBirthday_IncreasesAge()
    {
        // Arrange
        Student student = Create(birthDate: "15.04.2004");
        int ageBefore = student.Age;

        // Act
        bool celebrated = student.CelebrateBirthday(new DateTime(2027, 4, 15));

        // Assert
        Assert.IsTrue(celebrated);
        Assert.AreEqual(ageBefore + 1, student.Age);
    }

    [TestMethod]
    [DataRow(4, 16)]
    [DataRow(5, 15)]
    public void CelebrateBirthday_NotBirthday_AgeUnchanged(int month, int day)
    {
        // Arrange
        Student student = Create(birthDate: "15.04.2004");
        int ageBefore = student.Age;

        // Act
        bool celebrated = student.CelebrateBirthday(new DateTime(2027, month, day));

        // Assert
        Assert.IsFalse(celebrated);
        Assert.AreEqual(ageBefore, student.Age);
    }

    [TestMethod]
    public void JumpWithParachute_ReturnsMessage()
    {
        // Arrange
        ISkydiver skydiver = Create();

        // Act
        string result = skydiver.JumpWithParachute();

        // Assert
        Assert.AreEqual("Олена Шевченко стрибає з парашутом!", result);
    }

    [TestMethod]
    public void ToString_ContainsMainData()
    {
        // Arrange
        Student student = Create();

        // Act
        string text = student.ToString();

        // Assert
        StringAssert.Contains(text, "Шевченко Олена");
        StringAssert.Contains(text, "15.04.2004");
    }
}
