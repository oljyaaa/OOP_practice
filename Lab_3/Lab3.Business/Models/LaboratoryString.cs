namespace Lab3.Business.Models;

// BLL model for the first part, variant 8.
public sealed class LaboratoryString
{
    public string Value { get; private set; }
    public int Length => Value.Length;

    public LaboratoryString(string value) => Value = value ?? throw new ArgumentNullException(nameof(value));

    public int FindCharacter(char character) => Value.IndexOf(character);
    public void Reverse() => Value = new string(Value.Reverse().ToArray());
    public void Append(string suffix) => Value += suffix ?? throw new ArgumentNullException(nameof(suffix));
    public override string ToString() => Value;
}
