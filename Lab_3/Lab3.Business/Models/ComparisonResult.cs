namespace Lab3.Business.Models;

// Результат порівняння масиву і колекції після десеріалізації
public class ComparisonResult
{
    public MyString[] RestoredArray { get; }
    public List<MyString> RestoredCollection { get; }
    public bool AreEqual { get; }

    // Зберігає відновлені дані та результат порівняння
    public ComparisonResult(MyString[] restoredArray, List<MyString> restoredCollection, bool areEqual)
    {
        RestoredArray = restoredArray;
        RestoredCollection = restoredCollection;
        AreEqual = areEqual;
    }
}
