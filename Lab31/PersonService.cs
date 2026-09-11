using Lab31.Domain;
using Lab31.FileStorage;

namespace Lab31;

public sealed class PersonService
{
    private readonly IPersonRepository repository;

    public PersonService(IPersonRepository repository)
    {
        this.repository = repository;
    }

    public Person[] GetAll() => repository.ReadAll();

    public void Add(Person person)
    {
        if (FindById(person.UniqueId) is not null)
            throw new InvalidOperationException("Особа з таким унікальним ідентифікатором уже існує.");

        repository.Append(person);
    }

    public Person[] FindByLastName(string lastName)
    {
        Person[] people = repository.ReadAll();
        Person[] matches = new Person[people.Length];
        int count = 0;

        for (int i = 0; i < people.Length; i++)
        {
            if (string.Equals(people[i].LastName, lastName, StringComparison.OrdinalIgnoreCase))
                matches[count++] = people[i];
        }

        return Trim(matches, count);
    }

    public Person? FindById(string uniqueId)
    {
        Person[] people = repository.ReadAll();

        for (int i = 0; i < people.Length; i++)
        {
            if (string.Equals(people[i].UniqueId, uniqueId, StringComparison.OrdinalIgnoreCase))
                return people[i];
        }

        return null;
    }

    public bool DeleteById(string uniqueId)
    {
        Person[] people = repository.ReadAll();
        Person[] remaining = new Person[people.Length];
        int count = 0;
        bool deleted = false;

        for (int i = 0; i < people.Length; i++)
        {
            if (string.Equals(people[i].UniqueId, uniqueId, StringComparison.OrdinalIgnoreCase))
            {
                deleted = true;
                continue;
            }

            remaining[count++] = people[i];
        }

        if (deleted)
            repository.OverwriteAll(Trim(remaining, count));

        return deleted;
    }

    public Student[] GetFourthCourseSpringStudents()
    {
        Person[] people = repository.ReadAll();
        Student[] matches = new Student[people.Length];
        int count = 0;

        for (int i = 0; i < people.Length; i++)
        {
            if (people[i] is Student student && student.Course == 4 && student.BirthDate.Month is >= 3 and <= 5)
                matches[count++] = student;
        }

        Student[] result = new Student[count];
        Array.Copy(matches, result, count);
        return result;
    }

    private static Person[] Trim(Person[] source, int count)
    {
        Person[] result = new Person[count];
        Array.Copy(source, result, count);
        return result;
    }
}
