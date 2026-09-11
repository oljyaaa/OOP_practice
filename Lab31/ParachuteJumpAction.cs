using Lab31.Domain;

namespace Lab31;

public sealed class ParachuteJumpAction : IPersonAction
{
    public string MenuTitle => "Продемонструвати стрибок із парашутом";

    public bool CanExecute(Person person) => person is IParachuteJump;

    public string Execute(Person person)
    {
        IParachuteJump parachutist = (IParachuteJump)person;
        parachutist.JumpWithParachute();
        return $"{person.FirstName} {person.LastName} виконав(ла) стрибок із парашутом.";
    }
}
