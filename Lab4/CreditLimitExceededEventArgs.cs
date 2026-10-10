namespace Lab4;

public class CreditLimitExceededEventArgs : EventArgs
{
    public decimal RequestedAmount { get; }
    public decimal Balance { get; }
    public decimal CreditLimit { get; }
    public decimal ExcessAmount { get; }

    /// <summary>Зберігає дані про спробу перевищити кредитний ліміт.</summary>
    public CreditLimitExceededEventArgs(
        decimal requestedAmount,
        decimal balance,
        decimal creditLimit,
        decimal excessAmount)
    {
        RequestedAmount = requestedAmount;
        Balance = balance;
        CreditLimit = creditLimit;
        ExcessAmount = excessAmount;
    }
}
