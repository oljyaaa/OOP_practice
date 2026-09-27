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

    public int FindCharacter(char symbol)  // Пошук заданого символу
    {
        return Value.IndexOf(symbol);
    }
    
    public void Reverse() // Зміна порядку символів на протилежний
    {
        char[] chars = Value.ToCharArray();
        Array.Reverse(chars);
        Value = new string(chars);
    }

    public void Append(string text)    // Додавання нового рядка до існуючого
    {
        Value += text ?? string.Empty;
    }


    public void Print()    // Виведення рядка
    {
        Console.WriteLine($"Значення: \"{Value}\", довжина: {Length}");
    }


    public int CompareTo(MyString other)    // Порівняння за довжиною, однакова довжина — значення
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
