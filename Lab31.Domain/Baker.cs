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
}
