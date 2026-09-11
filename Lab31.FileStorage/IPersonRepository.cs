using Lab31.Domain;

namespace Lab31.FileStorage;

public interface IPersonRepository
{
    Person[] ReadAll();
    void Append(Person person);
    void OverwriteAll(Person[] people);
}
