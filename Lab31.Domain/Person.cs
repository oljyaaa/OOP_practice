namespace Lab31.Domain;

public abstract class Person : IIdentifiable
{
    public string FirstName { get; }
    public string LastName { get; }
    public abstract string UniqueId { get; }

    protected Person(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}
