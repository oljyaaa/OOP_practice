namespace Lab5.DAL;

/// <summary>Контракт доступу до даних студентів (дозволяє підставляти заглушку в тестах).</summary>
public interface IStudentRepository
{
    /// <summary>Читає всіх студентів зі сховища.</summary>
    List<StudentRecord> Load();

    /// <summary>Записує всіх студентів у сховище.</summary>
    void Save(List<StudentRecord> students);
}
