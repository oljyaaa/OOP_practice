namespace Lab5.BLL;

/// <summary>Підприємець — має бізнес, готує тістечка та вміє стрибати з парашутом.</summary>
public class Entrepreneur : Person, ICakeMaker
{
    public string Business { get; }

    /// <summary>Створює підприємця з назвою бізнесу.</summary>
    public Entrepreneur(string lastName, string firstName, string business) : base(lastName, firstName)
    {
        if (string.IsNullOrWhiteSpace(business))
        {
            throw new ArgumentException("Назва бізнесу не може бути порожньою.");
        }

        Business = business.Trim();
    }

    /// <summary>Готує тістечко для продажу у своєму бізнесі.</summary>
    public string MakeCake(params string[] ingredients)
    {
        return CakeRecipe.Make($"Підприємець {FirstName} ({Business})", ingredients);
    }
}
