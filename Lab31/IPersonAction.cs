using Lab31.Domain;

namespace Lab31;

public interface IPersonAction
{
    string MenuTitle { get; }
    bool CanExecute(Person person);
    string Execute(Person person);
}
