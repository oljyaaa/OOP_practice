namespace Lab3.Data.Entities;

// Сутність "Пекар" (додаткова сутність варіанта 8)
public class BakerEntity : IEntity
{
    public string Surname { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string Bakery { get; set; } = "";

    // Повертає поля пекаря для бінарного та користувацького формату
    public Dictionary<string, string> ToFields()
    {
        Dictionary<string, string> fields = new Dictionary<string, string>();
        fields["Surname"] = Surname;
        fields["FirstName"] = FirstName;
        fields["Bakery"] = Bakery;
        return fields;
    }

    // Відновлює поля пекаря зі словника
    public void FromFields(Dictionary<string, string> fields)
    {
        Surname = fields["Surname"];
        FirstName = fields["FirstName"];
        Bakery = fields["Bakery"];
    }
}
