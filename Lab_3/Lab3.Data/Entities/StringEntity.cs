namespace Lab3.Data.Entities;

public sealed class StringEntity
{
    public string Value { get; set; } = string.Empty;
    public int Length => Value.Length;
}
