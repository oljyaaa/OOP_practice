using System;
using System.Collections;
using System.Collections.Generic;

namespace Lab32Variant8;

internal class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // За варіантом потрібно 4 об'єкти.
        MyString s1 = new("Hello");
        MyString s2 = new("Programming");
        MyString s3 = new("CSharp");
        MyString s4 = new("Laboratory");

        DemonstrateClassMethods(s1);
        DemonstrateArray(s1, s2, s3, s4);
        DemonstrateNonGenericCollection(s1, s2, s3, s4);
        DemonstrateGenericCollection(s1, s2, s3, s4);
        DemonstrateSorting(s1, s2, s3, s4);
        DemonstrateBinaryTree(s1, s2, s3, s4);

        Console.WriteLine("\n=== КОРОТКЕ ПОРІВНЯННЯ ===");
        Console.WriteLine("Масив: фіксований розмір; для додавання/видалення треба створювати новий масив.");
        Console.WriteLine("ArrayList: розмір змінюється, але елементи зберігаються як object і немає типобезпечності.");
        Console.WriteLine("List<MyString>: розмір змінюється і колекція типобезпечна.");
    }

    private static void DemonstrateClassMethods(MyString sample)
    {
        Console.WriteLine("=== 1. КЛАС РЯДОК ===");
        sample.Print();

        char symbol = 'l';
        int index = sample.FindCharacter(symbol);
        Console.WriteLine(index >= 0
            ? $"Символ '{symbol}' знайдено на позиції {index}."
            : $"Символ '{symbol}' не знайдено.");

        sample.Reverse();
        Console.Write("Після зміни порядку символів: ");
        sample.Print();

        sample.Append(" World");
        Console.Write("Після додавання нового рядка: ");
        sample.Print();
    }

    private static void DemonstrateArray(MyString s1, MyString s2, MyString s3, MyString s4)
    {
        Console.WriteLine("\n=== 2. МАСИВ ===");

        MyString[] array = { s1, s2, s3, s4 };
        PrintArray(array, "Початковий масив:");

        // Додавання: масив фіксований, тому створюємо більший.
        array = AddToArray(array, new MyString("AddedToArray"));
        PrintArray(array, "Після додавання:");

        // Оновлення.
        array[0] = new MyString("UpdatedArrayItem");
        PrintArray(array, "Після оновлення першого елемента:");

        // Пошук.
        int foundIndex = FindInArray(array, "Programming");
        Console.WriteLine(foundIndex >= 0
            ? $"Пошук: Programming знайдено за індексом {foundIndex}."
            : "Пошук: Programming не знайдено.");

        // Видалення.
        array = RemoveAt(array, 1);
        PrintArray(array, "Після видалення елемента з індексом 1:");
    }

    private static void DemonstrateNonGenericCollection(MyString s1, MyString s2, MyString s3, MyString s4)
    {
        Console.WriteLine("\n=== 3. NON-GENERIC КОЛЕКЦІЯ ArrayList ===");

        ArrayList list = new();
        list.Add(s1);
        list.Add(s2);
        list.Add(s3);
        list.Add(s4);
        PrintArrayList(list, "Після додавання 4 об'єктів:");

        list.Add(new MyString("AddedToArrayList"));
        PrintArrayList(list, "Після додавання нового елемента:");

        list[0] = new MyString("UpdatedArrayListItem");
        PrintArrayList(list, "Після оновлення першого елемента:");

        int foundIndex = FindInArrayList(list, "Programming");
        Console.WriteLine(foundIndex >= 0
            ? $"Пошук: Programming знайдено за індексом {foundIndex}."
            : "Пошук: Programming не знайдено.");

        list.RemoveAt(1);
        PrintArrayList(list, "Після видалення елемента з індексом 1:");
    }

    private static void DemonstrateGenericCollection(MyString s1, MyString s2, MyString s3, MyString s4)
    {
        Console.WriteLine("\n=== 4. GENERIC КОЛЕКЦІЯ List<MyString> ===");

        List<MyString> list = new() { s1, s2, s3, s4 };
        PrintGenericList(list, "Початкова колекція:");

        list.Add(new MyString("AddedToList"));
        PrintGenericList(list, "Після додавання:");

        list[0] = new MyString("UpdatedListItem");
        PrintGenericList(list, "Після оновлення першого елемента:");

        int foundIndex = list.FindIndex(x => x.Value == "Programming");
        Console.WriteLine(foundIndex >= 0
            ? $"Пошук: Programming знайдено за індексом {foundIndex}."
            : "Пошук: Programming не знайдено.");

        list.RemoveAt(1);
        PrintGenericList(list, "Після видалення елемента з індексом 1:");
    }

    private static void DemonstrateSorting(MyString s1, MyString s2, MyString s3, MyString s4)
    {
        Console.WriteLine("\n=== 5. СОРТУВАННЯ ЧЕРЕЗ IComparable<MyString> ===");

        List<MyString> list = new()
        {
            new MyString(s1.Value),
            new MyString(s2.Value),
            new MyString(s3.Value),
            new MyString(s4.Value)
        };

        Console.WriteLine("До сортування:");
        foreach (MyString item in list)
            item.Print();

        list.Sort();

        Console.WriteLine("Після сортування за довжиною:");
        foreach (MyString item in list)
            item.Print();
    }

    private static void DemonstrateBinaryTree(MyString s1, MyString s2, MyString s3, MyString s4)
    {
        Console.WriteLine("\n=== 6. УЗАГАЛЬНЕНЕ БІНАРНЕ ДЕРЕВО ===");

        BinaryTree<MyString> tree = new();
        tree.Add(new MyString(s1.Value));
        tree.Add(new MyString(s2.Value));
        tree.Add(new MyString(s3.Value));
        tree.Add(new MyString(s4.Value));

        Console.WriteLine("Postorder: ліве піддерево -> праве піддерево -> корінь");
        foreach (MyString item in tree)
            item.Print();
    }

    private static MyString[] AddToArray(MyString[] source, MyString item)
    {
        MyString[] result = new MyString[source.Length + 1];
        Array.Copy(source, result, source.Length);
        result[^1] = item;
        return result;
    }

    private static MyString[] RemoveAt(MyString[] source, int index)
    {
        if (index < 0 || index >= source.Length)
            return source;

        MyString[] result = new MyString[source.Length - 1];
        int target = 0;

        for (int i = 0; i < source.Length; i++)
        {
            if (i == index)
                continue;

            result[target++] = source[i];
        }

        return result;
    }

    private static int FindInArray(MyString[] source, string value)
    {
        for (int i = 0; i < source.Length; i++)
        {
            if (source[i].Value == value)
                return i;
        }

        return -1;
    }

    private static int FindInArrayList(ArrayList source, string value)
    {
        for (int i = 0; i < source.Count; i++)
        {
            if (source[i] is MyString item && item.Value == value)
                return i;
        }

        return -1;
    }

    private static void PrintArray(MyString[] source, string title)
    {
        Console.WriteLine(title);
        foreach (MyString item in source)
            item.Print();
    }

    private static void PrintArrayList(ArrayList source, string title)
    {
        Console.WriteLine(title);
        foreach (object item in source)
        {
            if (item is MyString text)
                text.Print();
        }
    }

    private static void PrintGenericList(List<MyString> source, string title)
    {
        Console.WriteLine(title);
        foreach (MyString item in source)
            item.Print();
    }
}
