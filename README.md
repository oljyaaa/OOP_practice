# OOP_practice

Консольний застосунок C# для практичних робіт з ООП.

## Запуск проєкту

1. Встановіть [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0) або новішу сумісну версію.
2. Відкрийте папку `Lab31` у VS Code.
3. У вбудованому терміналі виконайте:

```bash
dotnet run
```

## Збірка у VS Code

Відкрийте термінал у папці `Lab31` і виконайте:

```bash
dotnet build
```

Або скористайтеся командою **Terminal → Run Build Task** (`Ctrl+Shift+B`). Результат збірки буде в папці `Lab31/bin`.

## Створення нової практичної роботи

У кореневій папці репозиторію створіть новий консольний проєкт і відкрийте його у VS Code:

```bash
dotnet new console -n LabXX
code LabXX
```

Замініть `LabXX` на номер потрібної роботи. Для запуску нового проєкту:

```bash
cd LabXX
dotnet run
```

## Збереження змін у GitHub

Після змін у коді виконайте в корені репозиторію:

```bash
git add .
git commit -m "Опис змін"
git push origin main
```
