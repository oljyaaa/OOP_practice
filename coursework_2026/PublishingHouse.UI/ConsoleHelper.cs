namespace PublishingHouse.UI
{
    /// <summary>
    /// Оформлене виведення в консоль.
    /// </summary>
    internal static class ConsoleHelper
    {
        public static void Header(string title)
        {
            Console.WriteLine();
            WriteColored($"===== {title} =====", ConsoleColor.Cyan);
        }

        public static void Success(string message) => WriteColored($"✔ {message}", ConsoleColor.Green);

        public static void Error(string message) => WriteColored($"✖ {message}", ConsoleColor.Red);

        public static void Warning(string message) => WriteColored($"! {message}", ConsoleColor.Yellow);

        public static void Info(string message) => Console.WriteLine(message);

        public static string Money(decimal amount) => $"{amount:N2} грн";

        private static void WriteColored(string text, ConsoleColor color)
        {
            ConsoleColor previous = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = previous;
        }
    }
}
