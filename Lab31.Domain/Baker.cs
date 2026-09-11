namespace Lab31.Domain;

public sealed class Baker : Person, IParachuteJump
{
    public string PersonId { get; }
    public bool HasJumpedWithParachute { get; private set; }

    public override string UniqueId => PersonId;

    public Baker(string firstName, string lastName, string personId)
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
            "Baker",
            $"{FirstName}{LastName}",
            ["firstname", "lastname", "personId"],
            [FirstName, LastName, PersonId]);
    }
}

public sealed class BakerFactory : IPersonFactory
{
    public string RecordTypeName => "Baker";

    public Person Create(PersonRecord record)
    {
        return new Baker(record.RequireValue("firstname"), record.RequireValue("lastname"), record.RequireValue("personId"));
    }
}
