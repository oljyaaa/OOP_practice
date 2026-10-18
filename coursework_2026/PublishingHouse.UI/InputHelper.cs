using System.Globalization;
using PublishingHouse.Core.Exceptions;

namespace PublishingHouse.UI
{
    /// <summary>
    /// Зчитування та перевірка даних, введених користувачем.
    /// Усі методи повторюють запит, доки не буде введено коректне значення.
    /// </summary>
    internal static class InputHelper
    {
        public static string ReadLine()
        {
            string? line = Console.ReadLine();
            if (line is null)
                throw new EndOfInputException();
            return line.Trim();
        }

        public static int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(ReadLine(), out int value) && value >= min && value <= max)
                    return value;
                ConsoleHelper.Error($"Введіть ціле число від {min} до {max}.");
            }
        }

        public static string ReadNonEmpty(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string value = ReadLine();
                if (value.Length > 0)
                    return value;
                ConsoleHelper.Error("Значення не може бути порожнім.");
            }
        }

        public static bool Confirm(string prompt)
        {
            while (true)
            {
                Console.Write($"{prompt} (т/н): ");
                switch (ReadLine().ToLowerInvariant())
                {
                    case "т": case "так": case "y": case "yes":
                        return true;
                    case "н": case "ні": case "n": case "no":
                        return false;
                }
                ConsoleHelper.Error("Введіть «т» (так) або «н» (ні).");
            }
        }

        public static T ReadEnum<T>(string prompt) where T : struct, Enum
        {
            T[] values = Enum.GetValues<T>();
            Console.WriteLine(prompt);
            for (int i = 0; i < values.Length; i++)
                Console.WriteLine($"  {i + 1}. {DisplayNames.Of(values[i])}");
            return values[ReadInt("Ваш вибір: ", 1, values.Length) - 1];
        }

        /// <summary>
        /// Зчитує рядок і передає його в <paramref name="apply"/>, який зазвичай
        /// присвоює значення властивості моделі. Якщо модель відхиляє значення
        /// (ArgumentException або PublishingException), користувач бачить причину і вводить ще раз.
        /// </summary>
        public static void ReadValid(string prompt, Action<string> apply)
        {
            while (true)
            {
                Console.Write(prompt);
                try
                {
                    apply(ReadLine());
                    return;
                }
                catch (ArgumentException ex)
                {
                    ConsoleHelper.Error(CleanMessage(ex));
                }
                catch (PublishingException ex)
                {
                    ConsoleHelper.Error(ex.Message);
                }
            }
        }

        /// <summary>
        /// Як <see cref="ReadValid"/>, але порожній ввід залишає поточне значення.
        /// </summary>
        public static void ReadValidOptional(string prompt, string currentValue, Action<string> apply)
        {
            ReadValid($"{prompt} [{currentValue}]: ", input =>
            {
                if (input.Length > 0)
                    apply(input);
            });
        }

        public static decimal ParseDecimal(string input)
        {
            string normalized = input.Replace(',', '.').Replace(" ", string.Empty);
            if (!decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value))
                throw new ArgumentException($"«{input}» не є числом.");
            return value;
        }

        public static int ParseInt(string input)
        {
            if (!int.TryParse(input, out int value))
                throw new ArgumentException($"«{input}» не є цілим числом.");
            return value;
        }

        /// <summary>
        /// Показує нумерований список і повертає обраний елемент або null (0 — скасувати).
        /// </summary>
        public static T? Choose<T>(IReadOnlyList<T> items, Func<T, string> format, string title) where T : class
        {
            if (items.Count == 0)
            {
                ConsoleHelper.Warning("Список порожній.");
                return null;
            }

            Console.WriteLine(title);
            for (int i = 0; i < items.Count; i++)
                Console.WriteLine($"  {i + 1}. {format(items[i])}");
            Console.WriteLine("  0. Скасувати");

            int choice = ReadInt("Ваш вибір: ", 0, items.Count);
            return choice == 0 ? null : items[choice - 1];
        }

        /// <summary>
        /// ArgumentException додає до повідомлення « (Parameter 'x')» — для користувача це зайве.
        /// </summary>
        public static string CleanMessage(Exception ex)
        {
            string message = ex.Message;
            int index = message.IndexOf(" (Parameter", StringComparison.Ordinal);
            return index >= 0 ? message[..index] : message;
        }
    }
}
