using Lab31;
using Lab31.Domain;
using Lab31.FileStorage;

class Program
{
    static void Main()
    {
        string fileName = Path.Combine(Directory.GetCurrentDirectory(), "data.txt");

        if (!File.Exists(fileName))
            fileName = Path.Combine(Directory.GetCurrentDirectory(), "Lab31", "data.txt");

        if (!File.Exists(fileName))
            fileName = Path.Combine(AppContext.BaseDirectory, "data.txt");

        IPersonFactoryRegistry factoryRegistry = new PersonFactoryRegistry();
        IPersonRepository repository = new FileManager(fileName, factoryRegistry);
        PersonService service = new PersonService(repository);
        ConsoleMenu menu = new ConsoleMenu(
            service,
            new PersonInputHandlerRegistry(),
            new PersonActionRegistry());

        menu.Start();
    }
}
