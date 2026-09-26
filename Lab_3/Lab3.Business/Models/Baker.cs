namespace Lab3.Business.Models;

public sealed class Baker
{
    public string Surname { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string BakeryName { get; init; } = string.Empty;
    public string JumpWithParachute() => $"Пекар {Surname} {FirstName} стрибає з парашутом.";
}
