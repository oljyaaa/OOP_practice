using Lab5.DAL;

namespace Lab5.BLL;

/// <summary>Бізнес-логіка роботи зі студентами. Сховище передається через конструктор (DI).</summary>
public class StudentService
{
    private readonly IStudentRepository _repository;

    /// <summary>Створює сервіс з будь-якою реалізацією сховища (файл або заглушка).</summary>
    public StudentService(IStudentRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <summary>Читає студентів зі сховища та перетворює їх на перевірені об'єкти Student.</summary>
    public List<Student> GetAll(DateTime today)
    {
        return _repository.Load().Select(r => ToStudent(r, today)).ToList();
    }

    /// <summary>Повертає студентів 4-го курсу, які народилися навесні.</summary>
    public List<Student> GetFourthCourseSpringStudents(DateTime today)
    {
        return GetAll(today).Where(s => s.Course == 4 && s.IsBornInSpring()).ToList();
    }

    /// <summary>Рахує студентів 4-го курсу, які народилися навесні.</summary>
    public int CountFourthCourseSpringStudents(DateTime today)
    {
        return GetFourthCourseSpringStudents(today).Count;
    }

    /// <summary>Додає студента у сховище (студентський квиток має бути унікальним).</summary>
    public void Add(Student student)
    {
        List<StudentRecord> records = _repository.Load();
        if (records.Any(r => r.StudentCard == student.StudentCard))
        {
            throw new InvalidOperationException("Студент з таким квитком уже існує.");
        }

        records.Add(ToRecord(student));
        _repository.Save(records);
    }

    /// <summary>Перетворює запис з файлу на Student (з перевіркою даних).</summary>
    public static Student ToStudent(StudentRecord r, DateTime today) =>
        new(r.LastName, r.FirstName, r.Course, r.StudentCard, r.BirthDate, today);

    /// <summary>Перетворює Student на запис для збереження у файл.</summary>
    public static StudentRecord ToRecord(Student s) => new()
    {
        LastName = s.LastName,
        FirstName = s.FirstName,
        Course = s.Course,
        StudentCard = s.StudentCard,
        BirthDate = s.BirthDate.ToString(Student.DateFormat)
    };
}
