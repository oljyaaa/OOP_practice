class Program
{
    static void Main()
    {
        FileManager fileManager =
            new FileManager("data.txt");

        ConsoleMenu menu =
            new ConsoleMenu(fileManager);

        menu.Start();
    }
}