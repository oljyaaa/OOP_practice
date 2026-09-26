namespace Lab3.Business.Models;

public sealed class Entrepreneur
{
    public string Surname { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string BusinessName { get; init; } = string.Empty;
    public string JumpWithParachute() => $"Підприємець {Surname} {FirstName} стрибає з парашутом.";
}
