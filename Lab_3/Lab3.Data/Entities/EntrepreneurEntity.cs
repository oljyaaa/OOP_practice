namespace Lab3.Data.Entities;

// Сутність "Підприємець" (додаткова сутність варіанта 8)
public class EntrepreneurEntity : IEntity
{
    public string Surname { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string Business { get; set; } = "";

    // Повертає поля підприємця для бінарного та користувацького формату
    public Dictionary<string, string> ToFields()
    {
        Dictionary<string, string> fields = new Dictionary<string, string>();
        fields["Surname"] = Surname;
        fields["FirstName"] = FirstName;
        fields["Business"] = Business;
        return fields;
    }

    // Відновлює поля підприємця зі словника
    public void FromFields(Dictionary<string, string> fields)
    {
        Surname = fields["Surname"];
        FirstName = fields["FirstName"];
        Business = fields["Business"];
    }

    // Підприємець коректний, якщо заповнені всі поля
    public bool IsValid()
    {
        return Surname != "" && FirstName != "" && Business != "";
    }
}
