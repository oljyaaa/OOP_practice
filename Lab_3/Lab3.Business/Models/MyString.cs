namespace Lab3.Business.Models;

// Клас "Рядок" (частина 1, варіант 8): значення та довжина
public class MyString
{
    public string Value { get; private set; }

    // Довжина завжди відповідає поточному значенню
    public int Length
    {
        get { return Value.Length; }
    }

    // Створює рядок із заданим значенням
    public MyString(string value)
    {
        Value = value;
    }

    // Пошук заданого символу: повертає індекс першого входження або -1
    public int FindSymbol(char symbol)
    {
        for (int i = 0; i < Value.Length; i++)
        {
            if (Value[i] == symbol)
            {
                return i;
            }
        }
        return -1;
    }

    // Зміна порядку символів на протилежний
    public void Reverse()
    {
        char[] symbols = Value.ToCharArray();
        Array.Reverse(symbols);
        Value = new string(symbols);
    }

    // Додавання нового рядка до існуючого
    public void Append(string addition)
    {
        Value = Value + addition;
    }

    // Виведення рядка: повертає значення і довжину
    public string GetInfo()
    {
        return "\"" + Value + "\" (довжина " + Length + ")";
    }
}
