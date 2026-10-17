# Лабораторна робота 3.5 — Модульне тестування (варіант 8)

**Student:** Прізвище, Ім'я, Курс, Студентський квиток, Дата народження (ДД.ММ.РРРР).

- Кількість студентів 4-го курсу, народжених навесні (дані з файлу `students.json`).
- Збільшення віку студента в день народження.
- Додаткові сутності: `Baker`, `Entrepreneur` — готують тістечка з різних інгредієнтів.
- Додаткове вміння: стрибати з парашутом (`ISkydiver`).

Рівні: `Lab5.PL` (консоль) → `Lab5.BLL` (логіка й перевірка даних) → `Lab5.DAL` (JSON-файл).
Сховище передається в `StudentService` через інтерфейс `IStudentRepository` (DI), у тестах — заглушка `StubStudentRepository`.

Тести: MSTest, 37 тестів за принципом Arrange-Act-Assert. Покриття (coverlet): **100% рядків, 100% гілок**.

Схеми: [SCHEMES.md](SCHEMES.md).

## Запуск

```bash
cd Lab_5/Lab5.PL && dotnet run
```

```bash
cd Lab_5 && dotnet test Lab5.Tests --collect:"XPlat Code Coverage"
```
