using Lab5.DAL;

namespace Lab5.Tests;

/// <summary>Заглушка сховища: тримає студентів у пам'яті замість файлу.</summary>
public class StubStudentRepository : IStudentRepository
{
    public List<StudentRecord> Records { get; } = new();
    public int SaveCalls { get; private set; }

    /// <summary>Повертає копію списку записів.</summary>
    public List<StudentRecord> Load() => new(Records);

    /// <summary>Запам'ятовує збережений список.</summary>
    public void Save(List<StudentRecord> students)
    {
        SaveCalls++;
        Records.Clear();
        Records.AddRange(students);
    }
}
