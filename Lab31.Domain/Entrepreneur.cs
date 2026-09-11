namespace Lab31.Domain;

public sealed class Entrepreneur : Person, IParachuteJump
{
    public string PersonId { get; }
    public bool HasJumpedWithParachute { get; private set; }

    public override string UniqueId => PersonId;

    public Entrepreneur(string firstName, string lastName, string personId)
        : base(firstName, lastName)
    {
        PersonId = personId;
    }

    public void JumpWithParachute()
    {
        HasJumpedWithParachute = true;
    }

    public override PersonRecord ToRecord()
    {
        return new PersonRecord(
            "Entrepreneur",
            $"{FirstName}{LastName}",
            ["firstname", "lastname", "personId"],
            [FirstName, LastName, PersonId]);
    }
}

public sealed class EntrepreneurFactory : IPersonFactory
{
    public string RecordTypeName => "Entrepreneur";

    public Person Create(PersonRecord record)
    {
        return new Entrepreneur(record.RequireValue("firstname"), record.RequireValue("lastname"), record.RequireValue("personId"));
    }
}
