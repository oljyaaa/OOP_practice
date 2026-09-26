using System.Reflection;

namespace Lab3.Data.Providers;

internal static class SerializableProperties
{
    public static PropertyInfo[] For<T>() => typeof(T).GetProperties()
        .Where(property => property.CanRead && property.CanWrite)
        .OrderBy(property => property.Name, StringComparer.Ordinal)
        .ToArray();
}
