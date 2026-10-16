namespace Lab5.BLL;

/// <summary>Пекар — готує тістечка з різних інгредієнтів і вміє стрибати з парашутом.</summary>
public class Baker : Person, ICakeMaker
{
    /// <summary>Створює пекаря.</summary>
    public Baker(string lastName, string firstName) : base(lastName, firstName)
    {
    }

    /// <summary>Готує тістечко; потрібно щонайменше 2 різні інгредієнти.</summary>
    public string MakeCake(params string[] ingredients)
    {
        return CakeRecipe.Make($"Пекар {FirstName}", ingredients);
    }
}
