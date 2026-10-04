namespace Lab3.Business.Models;

// Пекар (BLL-модель)
public class Baker : Person
{
    public string Bakery { get; set; }

    // Створює пекаря
    public Baker(string surname, string firstName, string bakery) : base(surname, firstName)
    {
        Bakery = bakery;
    }

    // Повертає інформацію про пекаря
    public override string GetInfo()
    {
        return Surname + " " + FirstName + ", пекарня: " + Bakery;
    }
}
