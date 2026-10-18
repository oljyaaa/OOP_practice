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
| 7 | Виняткові ситуації `*` | `ManuscriptRejectedException`, `InvalidPrintRunSizeException`, `InvalidStageTransitionException`, `EntityNotFoundException`; try-catch в UI |
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
│   ├── Menus/                       # меню авторів, редакторів, рукописів, тиражів, звітів
│   └── InputHelper.cs
├── PublishingHouse.Tests/           # (опційно) MSTest для калькуляції та етапів
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
- тираж поза межами (напр. < 100 або > 100 000) → `InvalidPrintRunSizeException`.

## Функції меню

1. Автори / редактори: додати, редагувати, видалити, переглянути.
2. Рукописи: додати, призначити редактора, рецензувати, перевести на наступний етап.
3. Тиражі: створити для рукопису `ReadyForPrint`, розрахувати собівартість і прибуток.
4. Пошук і фільтрація (за автором, жанром, етапом), сортування через `IComparer<T>`.
5. Звіт: найприбутковіші тиражі, загальний прогнозований прибуток.
6. Збереження та вихід (JSON).

## Етапи роботи

- [x] 1. Створити solution: Core (DLL) + UI (Console) + Tests (MSTest)
- [ ] 2. Моделі, енуми, абстрактні класи
- [ ] 3. Власні виключення
- [ ] 4. Інтерфейси, generic-репозиторій, компаратори
- [ ] 5. Сервіси: етапи, калькуляція, JSON-серіалізація
- [ ] 6. Консольне меню + валідація вводу + обробка винятків
- [ ] 7. Unit-тести (опційно) і ручна перевірка
- [ ] 8. UML-діаграма класів, скріншоти
- [ ] 9. Пояснювальна записка (.docx): титульний, зміст, опис завдання, бізнес-частина, UI, інші компоненти, лістинги, скріншоти
