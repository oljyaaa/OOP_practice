using Lab5.DAL;

namespace Lab5.Tests;

[TestClass]
public class JsonStudentRepositoryTests
{
    [TestMethod]
    public void SaveThenLoad_ReturnsSameData()
    {
        // Arrange
        string path = Path.GetTempFileName();
        var repo = new JsonStudentRepository(path);
        var records = new List<StudentRecord> { new() { LastName = "Коваль", FirstName = "Андрій", Course = 4, StudentCard = "К1", BirthDate = "02.03.2004" } };

        // Act
        repo.Save(records);
        List<StudentRecord> loaded = repo.Load();
        File.Delete(path);

        // Assert
        Assert.HasCount(1, loaded);
        Assert.AreEqual("Коваль", loaded[0].LastName);
    }

    [TestMethod]
    public void Load_MissingFile_ReturnsEmpty()
    {
        // Arrange
        var repo = new JsonStudentRepository(Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json"));

        // Act
        List<StudentRecord> loaded = repo.Load();

        // Assert
        Assert.IsEmpty(loaded);
    }

    [TestMethod]
    public void Load_FileWithNull_ReturnsEmpty()
    {
        // Arrange
        string path = Path.GetTempFileName();
        File.WriteAllText(path, "null");
        var repo = new JsonStudentRepository(path);

        // Act
        List<StudentRecord> loaded = repo.Load();
        File.Delete(path);

        // Assert
        Assert.IsEmpty(loaded);
    }
}
