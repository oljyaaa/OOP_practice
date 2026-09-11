using System.Reflection;

namespace Lab31;

public sealed class PersonInputHandlerRegistry
{
    private readonly IPersonInputHandler[] handlers;

    public PersonInputHandlerRegistry()
    {
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();
        IPersonInputHandler[] buffer = new IPersonInputHandler[types.Length];
        int count = 0;

        for (int i = 0; i < types.Length; i++)
        {
            if (types[i].IsAbstract || !typeof(IPersonInputHandler).IsAssignableFrom(types[i]))
                continue;

            buffer[count++] = (IPersonInputHandler)Activator.CreateInstance(types[i])!;
        }

        handlers = new IPersonInputHandler[count];
        Array.Copy(buffer, handlers, count);
    }

    public IPersonInputHandler[] GetAll() => handlers;
}
