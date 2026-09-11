using System.Reflection;

namespace Lab31;

public sealed class PersonActionRegistry
{
    private readonly IPersonAction[] actions;

    public PersonActionRegistry()
    {
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();
        IPersonAction[] buffer = new IPersonAction[types.Length];
        int count = 0;

        for (int i = 0; i < types.Length; i++)
        {
            if (types[i].IsAbstract || !typeof(IPersonAction).IsAssignableFrom(types[i]))
                continue;

            buffer[count++] = (IPersonAction)Activator.CreateInstance(types[i])!;
        }

        actions = new IPersonAction[count];
        Array.Copy(buffer, actions, count);
    }

    public IPersonAction[] GetAll() => actions;
}
