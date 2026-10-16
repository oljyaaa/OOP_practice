using Lab5.BLL;
using Lab5.DAL;

namespace Lab5.Tests;

[TestClass]
public class StudentServiceTests
{
    private static readonly DateTime Today = new(2026, 10, 16);

    /// <summary>Створює запис студента для заглушки.</summary>
    private static StudentRecord Record(string card, int course, string birthDate) => new()
    {
        LastName = "Коваль", FirstName = "Андрій", Course = course, StudentCard = card, BirthDate = birthDate
    };

    [TestMethod]
    public void Constructor_NullRepository_Throws()
    {
        // Arrange, Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => new StudentService(null!));
    }

    [TestMethod]
    public void CountFourthCourseSpringStudents_MixedData_CountsOnlyMatching()
    {
        // Arrange
        var repo = new StubStudentRepository();
        repo.Records.Add(Record("1", 4, "15.04.2004")); // підходить
        repo.Records.Add(Record("2", 4, "01.03.2004")); // підходить
        repo.Records.Add(Record("3", 4, "20.10.2003")); // осінь
        repo.Records.Add(Record("4", 3, "10.05.2005")); // 3 курс
        var service = new StudentService(repo);

        // Act
        int count = service.CountFourthCourseSpringStudents(Today);

        // Assert
        Assert.AreEqual(2, count);
    }

    [TestMethod]
    public void GetFourthCourseSpringStudents_EmptyRepository_ReturnsEmpty()
    {
        // Arrange
        var service = new StudentService(new StubStudentRepository());

        // Act
        List<Student> result = service.GetFourthCourseSpringStudents(Today);

        // Assert
        Assert.IsEmpty(result);
    }

    [TestMethod]
    public void GetAll_InvalidRecord_Throws()
    {
        // Arrange
        var repo = new StubStudentRepository();
        repo.Records.Add(Record("1", 9, "15.04.2004"));
        var service = new StudentService(repo);

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => service.GetAll(Today));
    }

    [TestMethod]
    public void Add_NewStudent_SavesToRepository()
    {
        // Arrange
        var repo = new StubStudentRepository();
        var service = new StudentService(repo);
        var student = new Student("Бондар", "Ірина", 4, "КВ7", "05.05.2004", Today);

        // Act
        service.Add(student);

        // Assert
        Assert.AreEqual(1, repo.SaveCalls);
        Assert.AreEqual("КВ7", repo.Records[0].StudentCard);
        Assert.AreEqual("05.05.2004", repo.Records[0].BirthDate);
    }

    [TestMethod]
    public void Add_DuplicateStudentCard_Throws()
    {
        // Arrange
        var repo = new StubStudentRepository();
        repo.Records.Add(Record("КВ7", 4, "15.04.2004"));
        var service = new StudentService(repo);
        var student = new Student("Бондар", "Ірина", 4, "КВ7", "05.05.2004", Today);

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => service.Add(student));
        Assert.AreEqual(0, repo.SaveCalls);
    }
}
