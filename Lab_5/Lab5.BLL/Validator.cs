namespace Lab5.BLL;

/// <summary>Спільні перевірки даних (контроль даних на рівні BLL).</summary>
public static class Validator
{
    /// <summary>Перевіряє, що ім'я не порожнє і складається з літер, апострофа чи дефіса.</summary>
    public static string RequireName(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{fieldName} не може бути порожнім.");
        }

        foreach (char c in value)
        {
            if (!char.IsLetter(c) && c != '\'' && c != '-')
            {
                throw new ArgumentException($"{fieldName} містить недопустимий символ '{c}'.");
            }
        }

        return value.Trim();
    }
}
