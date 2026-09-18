using System;

namespace Lab32Variant8;

public class MyString : IComparable<MyString>
{
    public string Value { get; private set; }

    public int Length => Value.Length;

    public MyString(string value)
    {
        Value = value ?? string.Empty;
    }

    // Пошук заданого символу. Повертає індекс або -1.
    public int FindCharacter(char symbol)
    {
        return Value.IndexOf(symbol);
    }

    // Зміна порядку символів на протилежний.
    public void Reverse()
    {
        char[] chars = Value.ToCharArray();
        Array.Reverse(chars);
        Value = new string(chars);
    }

    // Додавання нового рядка до існуючого.
    public void Append(string text)
    {
        Value += text ?? string.Empty;
    }

    // Виведення рядка.
    public void Print()
    {
        Console.WriteLine($"Значення: \"{Value}\", довжина: {Length}");
    }

    // Порівняння за довжиною, а при однаковій довжині — за значенням.
    public int CompareTo(MyString other)
    {
        if (other is null)
            return 1;

        int byLength = Length.CompareTo(other.Length);
        if (byLength != 0)
            return byLength;

        return string.Compare(Value, other.Value, StringComparison.Ordinal);
    }

    public override string ToString()
    {
        return Value;
    }
}
