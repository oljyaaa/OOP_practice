namespace Lab3.Business.Models;

// Підприємець (BLL-модель)
public class Entrepreneur : Person
{
    public string Business { get; set; }

    // Створює підприємця
    public Entrepreneur(string surname, string firstName, string business) : base(surname, firstName)
    {
        Business = business;
    }

    // Повертає інформацію про підприємця
    public override string GetInfo()
    {
        return Surname + " " + FirstName + ", бізнес: " + Business;
    }
}
