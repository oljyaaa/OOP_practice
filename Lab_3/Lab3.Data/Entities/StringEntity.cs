namespace Lab3.Data.Entities;

// Сутність "Рядок" (частина 1, варіант 8): значення та довжина
public class StringEntity : IEntity
{
    public string Value { get; set; } = "";
    public int Length { get; set; }

    // Повертає поля рядка для бінарного та користувацького формату
    public Dictionary<string, string> ToFields()
    {
        Dictionary<string, string> fields = new Dictionary<string, string>();
        fields["Value"] = Value;
        fields["Length"] = Length.ToString();
        return fields;
    }

    // Відновлює поля рядка зі словника
    public void FromFields(Dictionary<string, string> fields)
    {
        Value = fields["Value"];
        Length = int.Parse(fields["Length"]);
    }

    // Рядок коректний, якщо значення не порожнє і довжина з файлу збігається з реальною
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(Value) && Length == Value.Length;
    }
}
