using System.Globalization;
using Lab3.Business.Exceptions;
using Lab3.Business.Models;
using Lab3.Data;
using Lab3.Data.Entities;

namespace Lab3.Business;

// Проміжний рівень (BLL): додавання, видалення, пошук та обробка даних.
// Клас не працює з консоллю та файлами напряму - для файлів використовується EntityContext.
public class EntityService
{
    private const string DateFormat = "dd.MM.yyyy";

    private readonly EntityContext context = new EntityContext();

    private List<Student> students = new List<Student>();
    private List<Baker> bakers = new List<Baker>();
    private List<Entrepreneur> entrepreneurs = new List<Entrepreneur>();

    // Частина 1: чотири об'єкти класу "Рядок" (кількість за варіантом 8)
    private List<MyString> strings = new List<MyString>
    {
        new MyString("Лабораторна робота"),
        new MyString("Варіант 8"),
        new MyString("C#"),
        new MyString("Серіалізація")
    };

    // ===================== Частина 1: рядки =====================

    // Повертає всі рядки
    public List<MyString> GetStrings()
    {
        return strings;
    }

    // Додає новий рядок до колекції
    public void AddString(string value)
    {
        RequireText(value, "Рядок");
        strings.Add(new MyString(value));
    }

    // Шукає символ у рядку з заданим індексом
    public int FindSymbol(int index, char symbol)
    {
        return GetString(index).FindSymbol(symbol);
    }

    // Розвертає рядок з заданим індексом
    public void ReverseString(int index)
    {
        GetString(index).Reverse();
    }

    // Додає текст у кінець рядка з заданим індексом
    public void AppendToString(int index, string addition)
    {
        RequireText(addition, "Текст для додавання");
        GetString(index).Append(addition);
    }

    // Серіалізує рядки як масив
    public void SaveStringsAsArray(StorageOptions options)
    {
        MyString[] array = strings.ToArray();
        StringEntity[] entities = new StringEntity[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            entities[i] = ToEntity(array[i]);
        }

        try
        {
            context.UpdateArray(entities, options.FilePath, ToDataFormat(options.Format));
        }
        catch (Exception exception)
        {
            throw new StorageOperationException("Не вдалося зберегти масив у файл.", exception);
        }
        finally
        {
            context.Close(options.FilePath);
        }
    }

    // Десеріалізує рядки з файлу в новий масив
    public MyString[] LoadStringsAsArray(StorageOptions options)
    {
        StringEntity[] entities;
        try
        {
            entities = context.OpenArray<StringEntity>(options.FilePath, ToDataFormat(options.Format));
        }
        catch (Exception exception)
        {
            throw new StorageOperationException("Не вдалося прочитати масив з файлу.", exception);
        }
        finally
        {
            context.Close(options.FilePath);
        }

        MyString[] array = new MyString[entities.Length];
        for (int i = 0; i < entities.Length; i++)
        {
            array[i] = ToModel(entities[i]);
        }
        return array;
    }

    // Серіалізує рядки як колекцію List<T>
    public void SaveStringsAsCollection(StorageOptions options)
    {
        SaveToFile(strings.ConvertAll(ToEntity), options);
    }

    // Десеріалізує колекцію рядків з файлу і замінює нею поточні рядки
    public void LoadStringsAsCollection(StorageOptions options)
    {
        strings = LoadFromFile<StringEntity>(options).ConvertAll(ToModel);
    }

    // Зберігає рядки як масив і як колекцію, відновлює обидва та порівнює їх поелементно
    public ComparisonResult CompareArrayWithCollection(StorageOptions arrayOptions, StorageOptions collectionOptions)
    {
        SaveStringsAsArray(arrayOptions);
        SaveStringsAsCollection(collectionOptions);

        MyString[] restoredArray = LoadStringsAsArray(arrayOptions);
        List<MyString> restoredCollection = LoadFromFile<StringEntity>(collectionOptions).ConvertAll(ToModel);

        bool areEqual = restoredArray.Length == restoredCollection.Count;
        for (int i = 0; areEqual && i < restoredArray.Length; i++)
        {
            if (restoredArray[i].Value != restoredCollection[i].Value)
            {
                areEqual = false;
            }
        }
        return new ComparisonResult(restoredArray, restoredCollection, areEqual);
    }

    // ===================== Частина 2: студенти =====================

    // Повертає всіх студентів
    public List<Student> GetStudents()
    {
        return students;
    }

    // Додає студента після перевірки даних
    public void AddStudent(Student student)
    {
        RequireText(student.Surname, "Прізвище");
        RequireText(student.FirstName, "Ім'я");
        RequireText(student.StudentCard, "Студентський квиток");
        if (student.Course < 1 || student.Course > 6)
        {
            throw new EntityValidationException("Курс має бути від 1 до 6.");
        }
        if (student.BirthDate > DateTime.Today)
        {
            throw new EntityValidationException("Дата народження не може бути в майбутньому.");
        }
        if (students.Exists(s => s.StudentCard == student.StudentCard))
        {
            throw new EntityValidationException("Студент з таким квитком вже існує.");
        }
        students.Add(student);
    }

    // Шукає студента за номером студентського квитка
    public Student FindStudent(string studentCard)
    {
        Student? student = students.Find(s => s.StudentCard == studentCard);
        if (student == null)
        {
            throw new EntityNotFoundException("Студента з квитком " + studentCard + " не знайдено.");
        }
        return student;
    }

    // Видаляє студента за номером студентського квитка
    public void RemoveStudent(string studentCard)
    {
        students.Remove(FindStudent(studentCard));
    }

    // Зберігає студентів у файл
    public void SaveStudents(StorageOptions options)
    {
        SaveToFile(students.ConvertAll(ToEntity), options);
    }

    // Читає студентів з файлу
    public void LoadStudents(StorageOptions options)
    {
        students = LoadFromFile<StudentEntity>(options).ConvertAll(ToModel);
    }

    // Завдання варіанта 8: отримує з файлу студентів 4-го курсу, які народилися навесні.
    // Кількість таких студентів - це Count повернутого списку.
    public List<Student> GetSpringStudentsOfFourthCourse(StorageOptions options)
    {
        List<Student> fromFile = LoadFromFile<StudentEntity>(options).ConvertAll(ToModel);
        List<Student> result = new List<Student>();
        foreach (Student student in fromFile)
        {
            if (student.Course == 4 && student.IsBornInSpring())
            {
                result.Add(student);
            }
        }
        return result;
    }

    // ===================== Частина 2: пекарі =====================

    // Повертає всіх пекарів
    public List<Baker> GetBakers()
    {
        return bakers;
    }

    // Додає пекаря після перевірки даних
    public void AddBaker(Baker baker)
    {
        RequireText(baker.Surname, "Прізвище");
        RequireText(baker.FirstName, "Ім'я");
        RequireText(baker.Bakery, "Пекарня");
        bakers.Add(baker);
    }

    // Шукає пекаря за прізвищем
    public Baker FindBaker(string surname)
    {
        Baker? baker = bakers.Find(b => b.Surname == surname);
        if (baker == null)
        {
            throw new EntityNotFoundException("Пекаря " + surname + " не знайдено.");
        }
        return baker;
    }

    // Видаляє пекаря за прізвищем
    public void RemoveBaker(string surname)
    {
        bakers.Remove(FindBaker(surname));
    }

    // Зберігає пекарів у файл
    public void SaveBakers(StorageOptions options)
    {
        SaveToFile(bakers.ConvertAll(ToEntity), options);
    }

    // Читає пекарів з файлу
    public void LoadBakers(StorageOptions options)
    {
        bakers = LoadFromFile<BakerEntity>(options).ConvertAll(ToModel);
    }

    // ===================== Частина 2: підприємці =====================

    // Повертає всіх підприємців
    public List<Entrepreneur> GetEntrepreneurs()
    {
        return entrepreneurs;
    }

    // Додає підприємця після перевірки даних
    public void AddEntrepreneur(Entrepreneur entrepreneur)
    {
        RequireText(entrepreneur.Surname, "Прізвище");
        RequireText(entrepreneur.FirstName, "Ім'я");
        RequireText(entrepreneur.Business, "Бізнес");
        entrepreneurs.Add(entrepreneur);
    }

    // Шукає підприємця за прізвищем
    public Entrepreneur FindEntrepreneur(string surname)
    {
        Entrepreneur? entrepreneur = entrepreneurs.Find(e => e.Surname == surname);
        if (entrepreneur == null)
        {
            throw new EntityNotFoundException("Підприємця " + surname + " не знайдено.");
        }
        return entrepreneur;
    }

    // Видаляє підприємця за прізвищем
    public void RemoveEntrepreneur(string surname)
    {
        entrepreneurs.Remove(FindEntrepreneur(surname));
    }

    // Зберігає підприємців у файл
    public void SaveEntrepreneurs(StorageOptions options)
    {
        SaveToFile(entrepreneurs.ConvertAll(ToEntity), options);
    }

    // Читає підприємців з файлу
    public void LoadEntrepreneurs(StorageOptions options)
    {
        entrepreneurs = LoadFromFile<EntrepreneurEntity>(options).ConvertAll(ToModel);
    }

    // ===================== Операції з файлами =====================

    // Створює порожній файл бази даних для обраної сутності
    public void CreateEmptyFile(EntityKind kind, StorageOptions options)
    {
        SerializationFormat format = ToDataFormat(options.Format);
        try
        {
            switch (kind)
            {
                case EntityKind.Student:
                    context.Create<StudentEntity>(options.FilePath, format);
                    break;
                case EntityKind.Baker:
                    context.Create<BakerEntity>(options.FilePath, format);
                    break;
                case EntityKind.Entrepreneur:
                    context.Create<EntrepreneurEntity>(options.FilePath, format);
                    break;
                default:
                    context.Create<StringEntity>(options.FilePath, format);
                    break;
            }
        }
        catch (Exception exception)
        {
            throw new StorageOperationException("Не вдалося створити файл.", exception);
        }
        finally
        {
            context.Close(options.FilePath);
        }
    }

    // Видаляє файл бази даних
    public void DeleteFile(string filePath)
    {
        try
        {
            context.Delete(filePath);
        }
        catch (Exception exception)
        {
            throw new StorageOperationException("Не вдалося видалити файл.", exception);
        }
    }

    // ===================== Допоміжні методи =====================

    // Узагальнений запис будь-яких сутностей у файл через EntityContext
    private void SaveToFile<T>(List<T> entities, StorageOptions options) where T : IEntity, new()
    {
        try
        {
            context.Update(entities, options.FilePath, ToDataFormat(options.Format));
        }
        catch (Exception exception)
        {
            throw new StorageOperationException("Не вдалося записати дані у файл.", exception);
        }
        finally
        {
            context.Close(options.FilePath);
        }
    }

    // Узагальнене читання будь-яких сутностей з файлу через EntityContext
    private List<T> LoadFromFile<T>(StorageOptions options) where T : IEntity, new()
    {
        try
        {
            return context.Open<T>(options.FilePath, ToDataFormat(options.Format));
        }
        catch (Exception exception)
        {
            throw new StorageOperationException("Не вдалося прочитати дані з файлу.", exception);
        }
        finally
        {
            context.Close(options.FilePath);
        }
    }

    // Повертає рядок за індексом або генерує виняток, якщо індекс неправильний
    private MyString GetString(int index)
    {
        if (index < 0 || index >= strings.Count)
        {
            throw new EntityNotFoundException("Рядка з індексом " + index + " немає.");
        }
        return strings[index];
    }

    // Перевіряє, що текстове поле не порожнє
    private static void RequireText(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new EntityValidationException("Поле \"" + fieldName + "\" не може бути порожнім.");
        }
    }

    // Перетворює формат файлу з BLL у формат серіалізації DAL
    private static SerializationFormat ToDataFormat(FileFormat format)
    {
        switch (format)
        {
            case FileFormat.Binary:
                return SerializationFormat.Binary;
            case FileFormat.Xml:
                return SerializationFormat.Xml;
            case FileFormat.Json:
                return SerializationFormat.Json;
            default:
                return SerializationFormat.Custom;
        }
    }

    // ----- Перетворення BLL-моделей у DAL-сутності і навпаки -----

    // Рядок -> сутність DAL
    private static StringEntity ToEntity(MyString item)
    {
        return new StringEntity { Value = item.Value, Length = item.Length };
    }

    // Сутність DAL -> рядок
    private static MyString ToModel(StringEntity entity)
    {
        return new MyString(entity.Value);
    }

    // Студент -> сутність DAL (дата у форматі дд.ММ.рррр)
    private static StudentEntity ToEntity(Student item)
    {
        return new StudentEntity
        {
            Surname = item.Surname,
            FirstName = item.FirstName,
            Course = item.Course,
            StudentCard = item.StudentCard,
            BirthDate = item.BirthDate.ToString(DateFormat, CultureInfo.InvariantCulture)
        };
    }

    // Сутність DAL -> студент
    private static Student ToModel(StudentEntity entity)
    {
        DateTime birthDate = DateTime.ParseExact(entity.BirthDate, DateFormat, CultureInfo.InvariantCulture);
        return new Student(entity.Surname, entity.FirstName, entity.Course, entity.StudentCard, birthDate);
    }

    // Пекар -> сутність DAL
    private static BakerEntity ToEntity(Baker item)
    {
        return new BakerEntity { Surname = item.Surname, FirstName = item.FirstName, Bakery = item.Bakery };
    }

    // Сутність DAL -> пекар
    private static Baker ToModel(BakerEntity entity)
    {
        return new Baker(entity.Surname, entity.FirstName, entity.Bakery);
    }

    // Підприємець -> сутність DAL
    private static EntrepreneurEntity ToEntity(Entrepreneur item)
    {
        return new EntrepreneurEntity { Surname = item.Surname, FirstName = item.FirstName, Business = item.Business };
    }

    // Сутність DAL -> підприємець
    private static Entrepreneur ToModel(EntrepreneurEntity entity)
    {
        return new Entrepreneur(entity.Surname, entity.FirstName, entity.Business);
    }
}
