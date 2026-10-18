# Курсова робота з ООП — 2026

**Варіант 23. Книжкове видавництво (Publishing House)**

| Поле | Значення |
|---|---|
| Основні сутності | `Manuscript`, `Author`, `Editor`, `PrintRun` |
| Бізнес-логіка | Етапи підготовки книги до друку, калькуляція собівартості тиражу та прогнозованого прибутку |
| Власні виключення | `ManuscriptRejectedException` (рукопис не пройшов рецензування), `InvalidPrintRunSizeException` |

Мета — консольне застосування на C# (.NET 10) на оцінку **«Відмінно» + додаткові бали** (вимоги `*`, `**`, `***`).

## Відповідність вимогам

| № | Вимога | Як реалізуємо |
|---|---|---|
| 1 | Консольне застосування | `PublishingHouse.UI` — Console App з текстовим меню |
| 2 | Система класів домену | Моделі `Author`, `Editor`, `Manuscript`, `PrintRun` + базовий абстрактний клас `Person` / `EntityBase` |
| 3 | Узагальнені колекції `**` | `List<T>`, `Dictionary<Guid, T>` у generic-репозиторіях |
| 4 | Інтерфейси та абстрактні класи | Власні: `IRepository<T>`, `ICostCalculator`, `IReviewable`; стандартні: `IComparable<T>`, `IComparer<T>` |
| 5 | Збереження/завантаження `*` + серіалізація `***` | `JsonFileService<T>` (System.Text.Json): load при старті, save при виході |
| 6 | Контроль коректності вводу | Валідація у setter-ах властивостей + `InputHelper` (ReadInt/ReadDecimal/ReadString/ReadDate) в UI |
| 7 | Виняткові ситуації `*` | `ManuscriptRejectedException`, `InvalidPrintRunSizeException`, `InvalidStageTransitionException`, `EntityNotFoundException`, `DataStorageException`; try-catch в UI |
| 8 | Бізнес-логіка в DLL `**` | `PublishingHouse.Core` (Class Library), підключена до `PublishingHouse.UI` |
| 9 | Хороший стиль | Жодних public полів, кожен клас в окремому файлі, мнемонічні імена, XML-коментарі |
| 10 | Пояснювальна записка | `docs/` — звіт `.docx` за структурою кафедри |
| 11 | Вихідний код | Цей каталог (solution) |

## Планована структура

```
coursework_2026/
├── PublishingHouse.slnx
├── PublishingHouse.Core/            # DLL — бізнес-логіка
│   ├── Models/
│   │   ├── EntityBase.cs            # абстрактний: Id
│   │   ├── Person.cs                # абстрактний: ПІБ, email
│   │   ├── Author.cs                # гонорар (роялті %)
│   │   ├── Editor.cs                # ставка, спеціалізація
│   │   ├── Manuscript.cs            # назва, жанр, сторінки, автор, редактор, етап
│   │   ├── PrintRun.cs              # тираж, папір, обкладинка, ціна продажу
│   │   └── Enums/ (ManuscriptStage, Genre, CoverType, PaperType)
│   ├── Interfaces/ (IRepository<T>, ICostCalculator, IReviewable)
│   ├── Repositories/ (Repository<T>)
│   ├── Services/
│   │   ├── PublishingService.cs     # перехід етапів, рецензування
│   │   ├── CostCalculator.cs        # собівартість і прибуток
│   │   └── JsonFileService.cs       # серіалізація
│   ├── Comparers/ (за назвою, за прибутком, за кількістю сторінок)
│   └── Exceptions/
├── PublishingHouse.UI/              # Console App
│   ├── Program.cs
│   ├── Menus/                       # MenuBase + меню авторів, редакторів, рукописів, тиражів, звітів
│   ├── InputHelper.cs               # зчитування і перевірка вводу
│   ├── ConsoleHelper.cs             # кольорове виведення
│   └── DisplayNames.cs              # українські назви енумів
├── PublishingHouse.Tests/           # MSTest: моделі, репозиторій, компаратори, сервіси, JSON
└── docs/                            # пояснювальна записка, скріншоти, UML
```

## Предметна логіка

**Етапи підготовки книги** (`ManuscriptStage`):
`Submitted → UnderReview → Editing → Proofreading → Layout → ReadyForPrint → Printed`
(+ `Rejected`). Неприпустимий перехід → `InvalidStageTransitionException`; негативна рецензія → `ManuscriptRejectedException`.

**Калькуляція тиражу** (`CostCalculator`):
- собівартість = фіксовані витрати (редагування, верстка, дизайн обкладинки) + змінні (папір × сторінки + друк + обкладинка) × кількість примірників;
- собівартість одного примірника = загальна / тираж;
- виручка = тираж × ціна × прогнозований відсоток продажу;
- прибуток = виручка − собівартість − роялті автора.
- тираж поза межами 100–100 000 → `InvalidPrintRunSizeException`.

Тарифи: редагування — ставка редактора × сторінки; верстка 15 грн/стор.; дизайн обкладинки 5 000 грн;
папір 0,15 / 0,30 / 0,60 грн/стор. (газетний / офсетний / крейдований); друк 0,20 грн/стор.;
обкладинка 12 грн (м'яка) / 45 грн (тверда) за примірник.

## Функції меню

1. Автори / редактори: додати, редагувати, видалити, переглянути.
2. Рукописи: додати, призначити редактора, рецензувати, перевести на наступний етап.
3. Тиражі: створити для рукопису `ReadyForPrint`, розрахувати собівартість і прибуток.
4. Пошук і фільтрація (за автором, жанром, етапом), сортування через `IComparer<T>`.
5. Звіт: найприбутковіші тиражі, загальний прогнозований прибуток.
6. Збереження та вихід (JSON).

## Етапи роботи

- [x] 1. Створити solution: Core (DLL) + UI (Console) + Tests (MSTest)
- [x] 2. Моделі, енуми, абстрактні класи
- [x] 3. Власні виключення
- [x] 4. Інтерфейси, generic-репозиторій, компаратори
- [x] 5. Сервіси: етапи, калькуляція, JSON-серіалізація
- [x] 6. Консольне меню + валідація вводу + обробка винятків
- [x] 7. Unit-тести (70, покриття Core ≈ 96%) і ручна перевірка — [docs/MANUAL_TESTING.md](docs/MANUAL_TESTING.md)
- [ ] 8. UML-діаграма класів, скріншоти
- [ ] 9. Пояснювальна записка (.docx): титульний, зміст, опис завдання, бізнес-частина, UI, інші компоненти, лістинги, скріншоти

## Запуск

```bash
cd coursework_2026
dotnet run --project PublishingHouse.UI
```

Дані зберігаються у `publishing_data.json` у поточній папці: завантажуються при старті, зберігаються при виході
(або пунктом «Зберегти дані»). Якщо файл пошкоджений, програма робить його резервну копію `.bak` і стартує з порожньою базою.

```bash
dotnet test
```
