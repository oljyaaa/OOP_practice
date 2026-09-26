using Lab3.Business.Exceptions;
using Lab3.Business.Models;
using Lab3.Data;
using Lab3.Data.Entities;

namespace Lab3.Business;

public sealed class EntityService
{
    private readonly EntityContext _context;
    private List<Student> _students = [];
    private List<Baker> _bakers = [];
    private List<Entrepreneur> _entrepreneurs = [];
    private List<LaboratoryString> _strings = [
        new("Лабораторна робота"), new("Варіант 8"), new("C#"), new("Серіалізація")
    ];

    public EntityService(EntityContext context) => _context = context;
    public static EntityService CreateDefault() => new(new EntityContext());

    public IReadOnlyList<Student> Students => _students.AsReadOnly();
    public IReadOnlyList<Baker> Bakers => _bakers.AsReadOnly();
    public IReadOnlyList<Entrepreneur> Entrepreneurs => _entrepreneurs.AsReadOnly();
    public IReadOnlyList<LaboratoryString> Strings => _strings.AsReadOnly();

    public void AddStudent(Student student)
    {
        ValidateStudent(student);
        if (_students.Any(item => item.StudentCardNumber == student.StudentCardNumber))
            throw new EntityValidationException("Студент з таким номером квитка вже існує.");
        _students.Add(student);
    }

    public void RemoveStudent(string studentCardNumber) => Remove(_students, studentCardNumber, item => item.StudentCardNumber, "Студента не знайдено.");
    public Student FindStudent(string studentCardNumber) => Find(_students, studentCardNumber, item => item.StudentCardNumber, "Студента не знайдено.");

    public void AddBaker(Baker baker)
    {
        RequireName(baker.Surname, baker.FirstName);
        _bakers.Add(baker);
    }

    public void AddEntrepreneur(Entrepreneur entrepreneur)
    {
        RequireName(entrepreneur.Surname, entrepreneur.FirstName);
        _entrepreneurs.Add(entrepreneur);
    }

    public void RemoveBaker(string surname) => Remove(_bakers, surname, item => item.Surname, "Пекаря не знайдено.");
    public void RemoveEntrepreneur(string surname) => Remove(_entrepreneurs, surname, item => item.Surname, "Підприємця не знайдено.");
    public Baker FindBaker(string surname) => Find(_bakers, surname, item => item.Surname, "Пекаря не знайдено.");
    public Entrepreneur FindEntrepreneur(string surname) => Find(_entrepreneurs, surname, item => item.Surname, "Підприємця не знайдено.");

    public int FindCharacterInString(int index, char character) => GetString(index).FindCharacter(character);
    public void ReverseString(int index) => GetString(index).Reverse();
    public void AppendToString(int index, string suffix) => GetString(index).Append(suffix);
    public void AddString(string value) => _strings.Add(new LaboratoryString(value));

    public IReadOnlyList<Student> GetFourthCourseSpringStudents() => _students
        .Where(student => student.Course == 4 && student.WasBornInSpring).ToList();

    public void SaveStudents(StorageOptions options) => Save(_students.Select(ToEntity).ToList(), options);
    public void SaveBakers(StorageOptions options) => Save(_bakers.Select(ToEntity).ToList(), options);
    public void SaveEntrepreneurs(StorageOptions options) => Save(_entrepreneurs.Select(ToEntity).ToList(), options);
    public void SaveStringsAsCollection(StorageOptions options) => Save(_strings.Select(ToEntity).ToList(), options);
    public void SaveStringsAsArray(StorageOptions options) => Save(_strings.Select(ToEntity).ToArray(), options);

    public void LoadStudents(StorageOptions options) => _students = Load<StudentEntity>(options).Select(ToModel).ToList();
    public void LoadBakers(StorageOptions options) => _bakers = Load<BakerEntity>(options).Select(ToModel).ToList();
    public void LoadEntrepreneurs(StorageOptions options) => _entrepreneurs = Load<EntrepreneurEntity>(options).Select(ToModel).ToList();
    public void LoadStringsAsCollection(StorageOptions options) => _strings = Load<StringEntity>(options).Select(ToModel).ToList();
    public LaboratoryString[] LoadStringsAsArray(StorageOptions options) => Load<StringEntity>(options).Select(ToModel).ToArray();

    private void Save<T>(IReadOnlyCollection<T> entities, StorageOptions options) where T : class, new()
    {
        try { _context.Save(entities, options.FilePath, ToDataFormat(options.Format)); }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or ArgumentException or InvalidOperationException)
        { throw new StorageOperationException("Не вдалося зберегти дані у файл.", exception); }
    }

    private List<T> Load<T>(StorageOptions options) where T : class, new()
    {
        try { return _context.Load<T>(options.FilePath, ToDataFormat(options.Format)); }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or ArgumentException or InvalidOperationException)
        { throw new StorageOperationException("Не вдалося прочитати дані з файлу.", exception); }
    }

    private static SerializationFormat ToDataFormat(FileFormat format) => format switch
    {
        FileFormat.Binary => SerializationFormat.Binary,
        FileFormat.Xml => SerializationFormat.Xml,
        FileFormat.Json => SerializationFormat.Json,
        FileFormat.Custom => SerializationFormat.Custom,
        _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Непідтримуваний формат.")
    };
    private LaboratoryString GetString(int index) => index >= 0 && index < _strings.Count
        ? _strings[index] : throw new EntityNotFoundException("Рядок з таким індексом не знайдено.");

    private static void ValidateStudent(Student student)
    {
        RequireName(student.Surname, student.FirstName);
        if (student.Course is < 1 or > 6) throw new EntityValidationException("Курс має бути в межах від 1 до 6.");
        if (string.IsNullOrWhiteSpace(student.StudentCardNumber)) throw new EntityValidationException("Номер студентського квитка обов'язковий.");
        if (student.BirthDate > DateTime.Today) throw new EntityValidationException("Дата народження не може бути в майбутньому.");
    }

    private static void RequireName(string surname, string firstName)
    {
        if (string.IsNullOrWhiteSpace(surname) || string.IsNullOrWhiteSpace(firstName))
            throw new EntityValidationException("Прізвище та ім'я обов'язкові.");
    }

    private static void Remove<T>(List<T> items, string key, Func<T, string> selector, string error)
    {
        var item = Find(items, key, selector, error);
        items.Remove(item);
    }

    private static T Find<T>(IEnumerable<T> items, string key, Func<T, string> selector, string error) => items
        .FirstOrDefault(item => string.Equals(selector(item), key, StringComparison.OrdinalIgnoreCase))
        ?? throw new EntityNotFoundException(error);

    private static StudentEntity ToEntity(Student item) => new() { Surname = item.Surname, FirstName = item.FirstName, Course = item.Course, StudentCardNumber = item.StudentCardNumber, BirthDate = item.BirthDate };
    private static Student ToModel(StudentEntity item) => new() { Surname = item.Surname, FirstName = item.FirstName, Course = item.Course, StudentCardNumber = item.StudentCardNumber, BirthDate = item.BirthDate };
    private static BakerEntity ToEntity(Baker item) => new() { Surname = item.Surname, FirstName = item.FirstName, BakeryName = item.BakeryName };
    private static Baker ToModel(BakerEntity item) => new() { Surname = item.Surname, FirstName = item.FirstName, BakeryName = item.BakeryName };
    private static EntrepreneurEntity ToEntity(Entrepreneur item) => new() { Surname = item.Surname, FirstName = item.FirstName, BusinessName = item.BusinessName };
    private static Entrepreneur ToModel(EntrepreneurEntity item) => new() { Surname = item.Surname, FirstName = item.FirstName, BusinessName = item.BusinessName };
    private static StringEntity ToEntity(LaboratoryString item) => new() { Value = item.Value };
    private static LaboratoryString ToModel(StringEntity item) => new(item.Value);
}
