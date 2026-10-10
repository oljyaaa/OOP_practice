namespace Lab4;

public class BankAccountEventHandler
{
    /// <summary>Виводить повідомлення про перевищення кредитного ліміту.</summary>
    public void HandleCreditLimitExceeded(object? sender, CreditLimitExceededEventArgs e)
    {
        if (sender is not BankAccount account)
            return;

        Console.WriteLine(
            $"Подія: рахунок {account.Owner}, запитано {e.RequestedAmount:F2} грн, " +
            $"перевищення ліміту - {e.ExcessAmount:F2} грн.");
    }
}
