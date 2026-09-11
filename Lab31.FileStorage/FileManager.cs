using Lab31.Domain;

namespace Lab31.FileStorage;

public sealed class FileManager : IPersonRepository
{
    private readonly string fileName;
    private readonly IPersonFactoryRegistry factoryRegistry;

    public FileManager(string fileName, IPersonFactoryRegistry factoryRegistry)
    {
        this.fileName = fileName;
        this.factoryRegistry = factoryRegistry;
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

            PersonRecord record = ReadRecord(header, reader);
            buffer[count++] = factoryRegistry.Create(record);
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

    private static PersonRecord ReadRecord(string header, StreamReader reader)
    {
        int separator = header.IndexOf(' ');
        string typeName = separator >= 0 ? header[..separator].Trim() : header.Trim();
        string objectName = separator >= 0 ? header[(separator + 1)..].Trim() : string.Empty;

        if (reader.ReadLine()?.Trim() != "{")
            throw new InvalidDataException("Очікується початок запису '{'.");

        string[] namesBuffer = new string[20];
        string[] valuesBuffer = new string[20];
        int count = 0;
        string? line;

        while ((line = reader.ReadLine()) is not null)
        {
            if (line.Trim() == "};")
            {
                string[] names = new string[count];
                string[] values = new string[count];
                Array.Copy(namesBuffer, names, count);
                Array.Copy(valuesBuffer, values, count);
                return new PersonRecord(typeName, objectName, names, values);
            }

            if (count == namesBuffer.Length)
                throw new InvalidDataException("У записі забагато атрибутів.");

            ReadAttribute(line, out namesBuffer[count], out valuesBuffer[count]);
            count++;
        }

        throw new InvalidDataException("Запис у файлі не завершено.");
    }

    private static void ReadAttribute(string line, out string key, out string value)
    {
        int colon = line.IndexOf(':');
        if (colon < 0)
            throw new InvalidDataException("Невірний формат атрибуту.");

        key = line[..colon].Trim().Trim('"');
        value = line[(colon + 1)..].Trim().TrimEnd(',').Trim().Trim('"');
    }

    private static void WritePerson(StreamWriter writer, Person person)
    {
        PersonRecord record = person.ToRecord();
        writer.WriteLine($"{record.TypeName} {record.ObjectName}");
        writer.WriteLine("{");

        for (int i = 0; i < record.AttributeNames.Length; i++)
        {
            string comma = i == record.AttributeNames.Length - 1 ? string.Empty : ",";
            writer.WriteLine($"\"{record.AttributeNames[i]}\": \"{record.AttributeValues[i]}\"{comma}");
        }

        writer.WriteLine("};");
    }
}
