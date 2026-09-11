using System.Reflection;

namespace Lab31.Domain;

public sealed class PersonFactoryRegistry : IPersonFactoryRegistry
{
    private readonly IPersonFactory[] factories;

    public PersonFactoryRegistry()
    {
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();
        IPersonFactory[] buffer = new IPersonFactory[types.Length];
        int count = 0;

        for (int i = 0; i < types.Length; i++)
        {
            if (types[i].IsAbstract || !typeof(IPersonFactory).IsAssignableFrom(types[i]))
                continue;

            buffer[count++] = (IPersonFactory)Activator.CreateInstance(types[i])!;
        }

        factories = new IPersonFactory[count];
        Array.Copy(buffer, factories, count);
    }

    public Person Create(PersonRecord record)
    {
        for (int i = 0; i < factories.Length; i++)
        {
            if (string.Equals(factories[i].RecordTypeName, record.TypeName, StringComparison.OrdinalIgnoreCase))
                return factories[i].Create(record);
        }

        throw new InvalidDataException($"Невідомий тип запису: {record.TypeName}.");
    }
}
