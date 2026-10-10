namespace Lab4;

public class BankAccount
{
    public string Owner { get; }
    public decimal Balance { get; private set; }
    public decimal CreditLimit { get; }
    public bool IsClosed { get; private set; }

    public event EventHandler<CreditLimitExceededEventArgs>? CreditLimitExceeded;

    /// <summary>Відкриває рахунок із початковим балансом і кредитним лімітом.</summary>
    public BankAccount(string owner, decimal initialBalance, decimal creditLimit)
    {
        if (string.IsNullOrWhiteSpace(owner))
            throw new ArgumentException("Власника рахунку не вказано.", nameof(owner));

        if (initialBalance < 0)
            throw new ArgumentOutOfRangeException(nameof(initialBalance));

        if (creditLimit < 0)
            throw new ArgumentOutOfRangeException(nameof(creditLimit));

        Owner = owner;
        Balance = initialBalance;
        CreditLimit = creditLimit;
    }

    /// <summary>Поповнює рахунок на вказану суму.</summary>
    public void Deposit(decimal amount)
    {
        EnsureAccountIsOpen();
        EnsureAmountIsPositive(amount);
        Balance += amount;
    }

    /// <summary>Знімає кошти або повідомляє про перевищення кредитного ліміту.</summary>
    public bool Withdraw(decimal amount)
    {
        EnsureAccountIsOpen();
        EnsureAmountIsPositive(amount);

        decimal newBalance = Balance - amount;
        if (newBalance < -CreditLimit)
        {
            decimal excessAmount = -CreditLimit - newBalance;
            OnCreditLimitExceeded(amount, excessAmount);
            return false;
        }

        Balance = newBalance;
        return true;
    }

    /// <summary>Закриває рахунок, якщо його баланс дорівнює нулю.</summary>
    public bool Close()
    {
        EnsureAccountIsOpen();

        if (Balance != 0)
            return false;

        IsClosed = true;
        return true;
    }

    /// <summary>Генерує подію перевищення кредитного ліміту.</summary>
    protected virtual void OnCreditLimitExceeded(decimal requestedAmount, decimal excessAmount)
    {
        CreditLimitExceeded?.Invoke(
            this,
            new CreditLimitExceededEventArgs(
                requestedAmount,
                Balance,
                CreditLimit,
                excessAmount));
    }

    /// <summary>Перевіряє, чи рахунок відкритий.</summary>
    private void EnsureAccountIsOpen()
    {
        if (IsClosed)
            throw new InvalidOperationException("Рахунок уже закритий.");
    }

    /// <summary>Перевіряє, чи сума є додатною.</summary>
    private static void EnsureAmountIsPositive(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Сума повинна бути більшою за нуль.");
    }
}
