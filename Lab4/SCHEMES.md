# Схеми роботи програми 

## 1. Діаграма класів

```mermaid
classDiagram
    class Program {
        -Main()
        -DemonstrateDelegates()
        -DemonstrateBankAccount()
        -PrintArray(title, values)
    }

    class RowSumsOperation {
        <<delegate>>
        int[] Invoke(int[,] matrix)
    }

    class BankAccount {
        +string Owner
        +decimal Balance
        +decimal CreditLimit
        +bool IsClosed
        +event CreditLimitExceeded
        +BankAccount(owner, initialBalance, creditLimit)
        +Deposit(amount)
        +bool Withdraw(amount)
        +bool Close()
        #OnCreditLimitExceeded(requestedAmount, excessAmount)
    }

    class CreditLimitExceededEventArgs {
        +decimal RequestedAmount
        +decimal Balance
        +decimal CreditLimit
        +decimal ExcessAmount
    }

    class EventArgs
    class BankAccountEventHandler {
        +HandleCreditLimitExceeded(object? sender, CreditLimitExceededEventArgs e)
    }

    EventArgs <|-- CreditLimitExceededEventArgs
    Program ..> RowSumsOperation : анонімний метод і лямбда
    Program ..> BankAccount : створює і викликає операції
    Program ..> BankAccountEventHandler : підписує обробник (+=, -=)
    BankAccount ..> CreditLimitExceededEventArgs : створює при події
    BankAccountEventHandler ..> CreditLimitExceededEventArgs : читає дані події
```

## 2. Частина 1: делегат, анонімний метод і лямбда-вираз

```mermaid
flowchart LR
    M["Матриця 3x3<br/>{1,2,3}<br/>{4,5,6}<br/>{7,8,9}"] --> D{"Делегат<br/>RowSumsOperation"}
    D -->|"anonymousMethod(matrix)"| A["Анонімний метод<br/>два цикли for"]
    D -->|"lambdaExpression(matrix)"| L["Лямбда-вираз<br/>LINQ: Range + Sum"]
    A --> R1["[6, 15, 24]"]
    L --> R2["[6, 15, 24]"]
```

Алгоритм обчислення сум рядків (анонімний метод):

```mermaid
flowchart TD
    S([Початок]) --> C["sums = new int[кількість рядків]"]
    C --> R{"row < кількість рядків?"}
    R -- так --> K{"column < кількість стовпців?"}
    K -- так --> P["sums[row] += matrix[row, column]<br/>column++"] --> K
    K -- ні --> N["row++"] --> R
    R -- ні --> E([Повернути sums])
```

## 3. Частина 2: метод Withdraw і генерування події

```mermaid
flowchart TD
    S([Withdraw amount]) --> O{"Рахунок закритий?"}
    O -- так --> X1["InvalidOperationException"]
    O -- ні --> A{"amount > 0?"}
    A -- ні --> X2["ArgumentOutOfRangeException"]
    A -- так --> N["newBalance = Balance - amount"]
    N --> L{"newBalance < -CreditLimit?"}
    L -- так --> EX["excess = -CreditLimit - newBalance"]
    EX --> EV["OnCreditLimitExceeded(...)<br/>генерує подію"]
    EV --> F([return false, баланс не змінюється])
    L -- ні --> B["Balance = newBalance<br/>(може бути від'ємним = овердрафт)"]
    B --> T([return true])
```

## 4. Взаємодія об'єктів під час події

```mermaid
sequenceDiagram
    participant P as Program
    participant A as BankAccount
    participant H as BankAccountEventHandler

    P->>A: new BankAccount("Олена", 500, 300)
    P->>H: new BankAccountEventHandler()
    P->>A: CreditLimitExceeded += handler.HandleCreditLimitExceeded
    P->>A: Deposit(200)
    Note over A: Balance = 700
    P->>A: Withdraw(800)
    Note over A: Balance = -100 (овердрафт у межах ліміту)
    P->>A: Withdraw(500)
    Note over A: -600 < -300, ліміт перевищено
    A->>A: OnCreditLimitExceeded(500, 300)
    A->>H: CreditLimitExceeded?.Invoke(this, args)
    H-->>P: Console: "Подія: рахунок Олена ..."
    A-->>P: return false
    P->>A: Deposit(100)
    Note over A: Balance = 0
    P->>A: Close()
    A-->>P: true
    P->>A: CreditLimitExceeded -= handler.HandleCreditLimitExceeded
```

## 5. Стани банківського рахунку

```mermaid
stateDiagram-v2
    [*] --> Відкритий: конструктор
    Відкритий --> Відкритий: Deposit / Withdraw у межах ліміту
    Відкритий --> Відкритий: Withdraw понад ліміт / подія CreditLimitExceeded
    Відкритий --> Закритий: Close() при Balance = 0
    Закритий --> [*]
```
