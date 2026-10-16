namespace Lab5.BLL;

/// <summary>Спільна логіка приготування тістечка.</summary>
public static class CakeRecipe
{
    /// <summary>Перевіряє інгредієнти (мінімум 2 різні, без порожніх) і повертає опис тістечка.</summary>
    public static string Make(string cook, string[] ingredients)
    {
        if (ingredients == null || ingredients.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("Інгредієнти не можуть бути порожніми.");
        }

        string[] distinct = ingredients.Select(i => i.Trim().ToLower()).Distinct().ToArray();
        if (distinct.Length < 2)
        {
            throw new ArgumentException("Для тістечка потрібно щонайменше 2 різні інгредієнти.");
        }

        return $"{cook} приготував(ла) тістечко з: {string.Join(", ", distinct)}";
    }
}
