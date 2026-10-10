using System.Text;

namespace Lab4;

internal class Program
{
    /// <summary>Запускає демонстрацію делегатів і подій.</summary>
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        DemonstrateDelegates();
        DemonstrateBankAccount();
    }

    /// <summary>Демонструє анонімний метод і лямбда-вираз через делегат.</summary>
    private static void DemonstrateDelegates()
    {
        int[,] matrix =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        };

        RowSumsOperation anonymousMethod = delegate(int[,] source)
        {
            return RowSumCalculator.Calculate(source);
        };

        RowSumsOperation lambdaExpression = source => RowSumCalculator.Calculate(source);

        Console.WriteLine("=== ДЕЛЕГАТИ ===");
        PrintArray("Анонімний метод", anonymousMethod(matrix));
        PrintArray("Лямбда-вираз", lambdaExpression(matrix));
    }

    /// <summary>Демонструє операції з рахунком і обробку події.</summary>
    private static void DemonstrateBankAccount()
    {
        BankAccount account = new("Олена", 500m, 300m);
        BankAccountEventHandler handler = new();
        account.CreditLimitExceeded += handler.HandleCreditLimitExceeded;

        Console.WriteLine("\n=== БАНКІВСЬКИЙ РАХУНОК ===");
        Console.WriteLine($"Рахунок відкрито. Баланс: {account.Balance:F2} грн.");

        account.Deposit(200m);
        Console.WriteLine($"Після поповнення: {account.Balance:F2} грн.");

        account.Withdraw(800m);
        Console.WriteLine($"Після зняття з овердрафтом: {account.Balance:F2} грн.");

        bool withdrawn = account.Withdraw(500m);
        Console.WriteLine(withdrawn
            ? $"Після зняття: {account.Balance:F2} грн."
            : $"Операцію відхилено. Баланс: {account.Balance:F2} грн.");

        account.Deposit(100m);
        bool closed = account.Close();
        Console.WriteLine(closed ? "Рахунок закрито." : "Рахунок не можна закрити.");

        account.CreditLimitExceeded -= handler.HandleCreditLimitExceeded;
    }

    /// <summary>Виводить одновимірний масив.</summary>
    private static void PrintArray(string title, int[] values)
    {
        Console.WriteLine($"{title}: [{string.Join(", ", values)}]");
    }
}
