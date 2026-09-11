namespace Lab31.Domain;

public sealed class PersonRecord
{
    public string TypeName { get; }
    public string ObjectName { get; }
    public string[] AttributeNames { get; }
    public string[] AttributeValues { get; }

    public PersonRecord(string typeName, string objectName, string[] attributeNames, string[] attributeValues)
    {
        if (attributeNames.Length != attributeValues.Length)
            throw new ArgumentException("Кількість назв і значень атрибутів має збігатися.");

        TypeName = typeName;
        ObjectName = objectName;
        AttributeNames = attributeNames;
        AttributeValues = attributeValues;
    }

    public string RequireValue(string attributeName)
    {
        for (int i = 0; i < AttributeNames.Length; i++)
        {
            if (string.Equals(AttributeNames[i], attributeName, StringComparison.OrdinalIgnoreCase))
                return AttributeValues[i];
        }

        throw new InvalidDataException($"Відсутній обов'язковий атрибут {attributeName}.");
    }
}
