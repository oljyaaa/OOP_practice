using Lab31.Domain;

namespace Lab31;

public interface IPersonInputHandler
{
    string MenuTitle { get; }

    Person CreatePerson();
}
