# Лабораторна робота 3

## Частина 1 - клас «Рядок» (`MyString`)

Чотири початкові рядки. Операції варіанта 8: пошук заданого символу, зміна порядку символів на протилежний, додавання нового рядка до існуючого, виведення рядка (значення і довжина).

У меню рядків можна серіалізувати масив `MyString[]` і відновити його в новий масив, серіалізувати й відновити колекцію `List<MyString>`, а також порівняти масив і колекцію після десеріалізації. Формат та ім'я файлу обирає користувач.

## Частина 2 - студенти, пекарі, підприємці

Студент: прізвище, ім'я, курс, студентський квиток, дата народження у форматі `ХХ.ХХ.ХХХХ`. Операція варіанта 8 - кількість студентів **4-го курсу, які народилися навесні**, дані читаються з обраного файлу.

Додаткові сутності `Baker` і `Entrepreneur`. Усі люди наслідують `Person`, який реалізує `ISkydiver` - вміння «стрибати з парашутом».

## Архітектура

```text
Lab3.App              Program.Main() -> Menu.MainMenu()
        |
Lab3.Presentation     PL: Menu, StudentInput, BakerInput, EntrepreneurInput
        |
Lab3.Business         BLL: EntityService, власні моделі, власні винятки
        |
Lab3.Data             DAL: EntityContext, DataProvider + 4 провайдери, *Entity
```

- `*Entity` існують тільки в DAL; BLL і PL мають власні моделі.
- `EntityService` не використовує `Console`, `Stream` чи `File`.
- `EntityContext`: `Create`, `Open`, `Update`, `Close`, `Delete` (узагальнені методи).
- Винятки `EntityValidationException`, `EntityNotFoundException`, `StorageOperationException` обробляються в `Menu.MainMenu()`.

## Формати серіалізації

1. `BinaryDataProvider` - `BinaryWriter` / `BinaryReader` (`BinaryFormatter` у .NET 9+ видалено);
2. `XmlDataProvider` - `XmlSerializer`;
3. `JsonDataProvider` - `System.Text.Json`;
4. `CustomDataProvider` - власний формат: рядки `Назва=Значення`, об'єкти розділені порожнім рядком.

## Запуск

```bash
cd Lab_3
dotnet build Lab_3.slnx
dotnet run --project Lab3.App
```
