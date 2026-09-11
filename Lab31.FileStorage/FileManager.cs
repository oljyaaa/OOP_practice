using System.Globalization;
using Lab31.Domain;

namespace Lab31.FileStorage;

public sealed class FileManager : IPersonRepository
{
    private readonly string fileName;

    public FileManager(string fileName)
    {
        this.fileName = fileName;
    }

    public Person[] ReadAll()
    {
        Person[] buffer = new Person[100];
        int count = 0;

        using StreamReader reader = new(fileName);
        string? header;

        while ((header = reader.ReadLine()) is not null)
        {
            if (string.IsNullOrWhiteSpace(header))
                continue;

            if (count == buffer.Length)
                throw new InvalidDataException("Файл містить більше 100 записів.");

            string type = header.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
            PersonData data = ReadPersonData(reader);
            buffer[count++] = CreatePerson(type, data);
        }

        Person[] people = new Person[count];
        Array.Copy(buffer, people, count);
        return people;
    }

    public void Append(Person person)
    {
        using StreamWriter writer = new(fileName, append: true);
        WritePerson(writer, person);
    }

    public void OverwriteAll(Person[] people)
    {
        using StreamWriter writer = new(fileName, append: false);

        for (int i = 0; i < people.Length; i++)
            WritePerson(writer, people[i]);
    }

    private static PersonData ReadPersonData(StreamReader reader)
    {
        if (reader.ReadLine()?.Trim() != "{")
            throw new InvalidDataException("Очікується початок запису '{'.");

        PersonData data = new();
        string? line;

        while ((line = reader.ReadLine()) is not null)
        {
            if (line.Trim() == "};")
                return data;

            ReadAttribute(line, data);
        }

        throw new InvalidDataException("Запис у файлі не завершено.");
    }

    private static void ReadAttribute(string line, PersonData data)
    {
        int colon = line.IndexOf(':');
        if (colon < 0)
            throw new InvalidDataException("Невірний формат атрибуту.");

        string key = line[..colon].Trim().Trim('"');
        string value = line[(colon + 1)..].Trim().TrimEnd(',').Trim().Trim('"');

        switch (key)
        {
            case "firstname": data.FirstName = value; break;
            case "lastname": data.LastName = value; break;
            case "course": data.Course = value; break;
            case "studentId": data.StudentId = value; break;
            case "birthDate": data.BirthDate = value; break;
            case "personId": data.PersonId = value; break;
        }
    }

    private static Person CreatePerson(string type, PersonData data)
    {
        return type switch
        {
            "Student" => new Student(
                Require(data.FirstName, "firstname"),
                Require(data.LastName, "lastname"),
                int.Parse(Require(data.Course, "course"), CultureInfo.InvariantCulture),
                Require(data.StudentId, "studentId"),
                DateTime.ParseExact(Require(data.BirthDate, "birthDate"), "dd.MM.yyyy", CultureInfo.InvariantCulture)),
            "Baker" => new Baker(Require(data.FirstName, "firstname"), Require(data.LastName, "lastname"), Require(data.PersonId, "personId")),
            "Entrepreneur" => new Entrepreneur(Require(data.FirstName, "firstname"), Require(data.LastName, "lastname"), Require(data.PersonId, "personId")),
            _ => throw new InvalidDataException($"Невідомий тип запису: {type}.")
        };
    }

    private static string Require(string? value, string propertyName)
    {
        return !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new InvalidDataException($"Відсутній обов'язковий атрибут {propertyName}.");
    }

    private static void WritePerson(StreamWriter writer, Person person)
    {
        switch (person)
        {
            case Student student:
                writer.WriteLine($"Student {student.FirstName}{student.LastName}");
                writer.WriteLine("{");
                writer.WriteLine($"\"firstname\": \"{student.FirstName}\",");
                writer.WriteLine($"\"lastname\": \"{student.LastName}\",");
                writer.WriteLine($"\"course\": \"{student.Course}\",");
                writer.WriteLine($"\"studentId\": \"{student.StudentId}\",");
                writer.WriteLine($"\"birthDate\": \"{student.BirthDate:dd.MM.yyyy}\"");
                break;
            case Baker baker:
                WriteWorker(writer, "Baker", baker);
                return;
            case Entrepreneur entrepreneur:
                WriteWorker(writer, "Entrepreneur", entrepreneur);
                return;
            default:
                throw new InvalidDataException("Непідтримуваний тип особи.");
        }

        writer.WriteLine("};");
    }

    private static void WriteWorker(StreamWriter writer, string type, Person person)
    {
        writer.WriteLine($"{type} {person.FirstName}{person.LastName}");
        writer.WriteLine("{");
        writer.WriteLine($"\"firstname\": \"{person.FirstName}\",");
        writer.WriteLine($"\"lastname\": \"{person.LastName}\",");
        writer.WriteLine($"\"personId\": \"{person.UniqueId}\"");
        writer.WriteLine("};");
    }

    private sealed class PersonData
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Course { get; set; }
        public string? StudentId { get; set; }
        public string? BirthDate { get; set; }
        public string? PersonId { get; set; }
    }
}
