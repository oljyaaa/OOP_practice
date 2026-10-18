# UML-діаграми

Джерела — файли `.mmd` (Mermaid) у [uml/](uml/); GitHub рендерить їх прямо в цьому файлі. PNG для звіту лежать поруч.

## Моделі предметної області

![Моделі предметної області](uml/1_models.png)

<details><summary>Код Mermaid</summary>

```mermaid
classDiagram
    direction TB
    class EntityBase {
        <<abstract>>
        -Guid _id
        +Guid Id
        +GetSummary()* string
        +ToString() string
    }
    class Person {
        <<abstract>>
        -string _firstName
        -string _lastName
        -string _email
        +string FirstName
        +string LastName
        +string Email
        +string FullName
        +string Role*
        -ValidateName(string, string)$ string
    }
    class Author {
        +decimal MaxRoyaltyPercent$
        -decimal _royaltyPercent
        +decimal RoyaltyPercent
        +string Role
        +GetSummary() string
    }
    class Editor {
        +decimal MaxRatePerPage$
        -decimal _ratePerPage
        +decimal RatePerPage
        +Genre Specialization
        +string Role
        +GetSummary() string
    }
    class Manuscript {
        +int MaxPageCount$
        -string _title
        -int _pageCount
        -Guid _authorId
        +string Title
        +Genre Genre
        +int PageCount
        +Guid AuthorId
        +Guid? EditorId
        +ManuscriptStage Stage
        +DateTime SubmittedOn
        +string ReviewComment
        +bool CanBeReviewed
        +CompareTo(Manuscript) int
        +GetSummary() string
    }
    class PrintRun {
        -int _quantity
        -decimal _retailPrice
        -decimal _expectedSellThroughPercent
        +Guid ManuscriptId
        +int Quantity
        +PaperType PaperType
        +CoverType CoverType
        +decimal RetailPrice
        +decimal ExpectedSellThroughPercent
        +DateTime CreatedOn
        +GetSummary() string
    }
    class IReviewable {
        <<interface>>
        +bool CanBeReviewed
        +string ReviewComment
    }
    class IComparable~Manuscript~ {
        <<interface>>
        +CompareTo(Manuscript) int
    }
    class ManuscriptStage {
        <<enumeration>>
        Submitted
        UnderReview
        Editing
        Proofreading
        Layout
        ReadyForPrint
        Printed
        Rejected
    }
    class Genre {
        <<enumeration>>
        Fiction
        NonFiction
        Science
        Children
        Poetry
        Textbook
    }
    class PaperType {
        <<enumeration>>
        Newsprint
        Offset
        Coated
    }
    class CoverType {
        <<enumeration>>
        Paperback
        Hardcover
    }

    EntityBase <|-- Person
    Person <|-- Author
    Person <|-- Editor
    EntityBase <|-- Manuscript
    EntityBase <|-- PrintRun
    IReviewable <|.. Manuscript
    IComparable~Manuscript~ <|.. Manuscript
    Manuscript "0..*" --> "1" Author : AuthorId
    Manuscript "0..*" --> "0..1" Editor : EditorId
    PrintRun "0..*" --> "1" Manuscript : ManuscriptId
    Manuscript ..> ManuscriptStage
    Manuscript ..> Genre
    Editor ..> Genre
    PrintRun ..> PaperType
    PrintRun ..> CoverType
```

</details>

## Сервіси, репозиторії, компаратори

![Сервіси, репозиторії, компаратори](uml/2_services.png)

<details><summary>Код Mermaid</summary>

```mermaid
classDiagram
    direction TB
    class IRepository~T~ {
        <<interface>>
        +int Count
        +Add(T item)
        +GetById(Guid id) T
        +TryGetById(Guid id, out T item) bool
        +Remove(Guid id) bool
        +GetAll() IReadOnlyList~T~
        +Find(Predicate~T~ match) List~T~
        +ReplaceAll(IEnumerable~T~ items)
    }
    class Repository~T~ {
        -List~T~ _items
        -Dictionary~Guid, T~ _index
        -string _entityName
        +Repository(string entityName)
    }
    class ICostCalculator {
        <<interface>>
        +Estimate(PrintRun, Manuscript, Author, Editor?) PrintRunEstimate
    }
    class CostCalculator {
        +decimal LayoutCostPerPage$
        +decimal CoverDesignCost$
        +decimal PrintingCostPerPage$
        +CalculatePaperCostPerPage(PaperType) decimal
        +CalculateCoverCostPerCopy(CoverType) decimal
        +Estimate(PrintRun, Manuscript, Author, Editor?) PrintRunEstimate
    }
    class PublishingService {
        +int MinPrintRunSize$
        +int MaxPrintRunSize$
        -Dictionary~ManuscriptStage, ManuscriptStage~ NextStage$
        -IRepository~Author~ _authors
        -IRepository~Editor~ _editors
        -IRepository~Manuscript~ _manuscripts
        -IRepository~PrintRun~ _printRuns
        -ICostCalculator _calculator
        +AddAuthor(Author)
        +AddEditor(Editor)
        +ChangeEmail(Person, string)
        +RemoveAuthor(Guid)
        +RemoveEditor(Guid)
        +SubmitManuscript(Guid, string, Genre, int) Manuscript
        +RemoveManuscript(Guid)
        +AssignEditor(Guid, Guid)
        +AdvanceStage(Guid) ManuscriptStage
        +Review(Guid, bool, string)
        +CreatePrintRun(Guid, int, PaperType, CoverType, decimal, decimal) PrintRun
        +Estimate(PrintRun) PrintRunEstimate
        +EstimatePrintRun(Guid) PrintRunEstimate
        +GetTotalExpectedProfit() decimal
        +ExportData() PublishingData
        +ImportData(PublishingData)
        +EnsureEmailAvailable(string, Guid)
    }
    class JsonFileService~T~ {
        -JsonSerializerOptions Options$
        -string _filePath
        +string FilePath
        +bool FileExists
        +Save(T data)
        +Load() T
    }
    class IComparer~T~ {
        <<interface>>
        +Compare(T x, T y) int
    }
    class PersonByNameComparer
    class ManuscriptByPageCountComparer
    class ManuscriptByStageComparer
    class PrintRunByProfitComparer {
        -Func~PrintRun, decimal~ _profitSelector
    }

    class PrintRunEstimate {
        +decimal FixedCosts
        +decimal VariableCosts
        +int Quantity
        +decimal ExpectedRevenue
        +decimal Royalty
        +decimal TotalCost
        +decimal CostPerCopy
        +decimal ExpectedProfit
        +bool IsProfitable
    }
    class PublishingData {
        +List~Author~ Authors
        +List~Editor~ Editors
        +List~Manuscript~ Manuscripts
        +List~PrintRun~ PrintRuns
    }
    class Author
    class Editor
    class Manuscript
    class PrintRun

    IRepository~T~ <|.. Repository~T~
    ICostCalculator <|.. CostCalculator
    PublishingService o-- "4" IRepository~T~
    PublishingService o-- ICostCalculator
    PublishingService ..> PublishingData : Export / Import
    ICostCalculator ..> PrintRunEstimate : створює
    PublishingData o-- Author
    PublishingData o-- Editor
    PublishingData o-- Manuscript
    PublishingData o-- PrintRun
    JsonFileService~T~ ..> PublishingData : T = PublishingData
    IComparer~T~ <|.. PersonByNameComparer
    IComparer~T~ <|.. ManuscriptByPageCountComparer
    IComparer~T~ <|.. ManuscriptByStageComparer
    IComparer~T~ <|.. PrintRunByProfitComparer
```

</details>

## Ієрархія власних виключень

![Ієрархія власних виключень](uml/3_exceptions.png)

<details><summary>Код Mermaid</summary>

```mermaid
classDiagram
    direction TB
    class Exception {
        <<System>>
        +string Message
        +Exception InnerException
    }
    class PublishingException {
        +PublishingException(string message)
        +PublishingException(string message, Exception inner)
    }
    class ManuscriptRejectedException {
        +Guid ManuscriptId
        +string ManuscriptTitle
        +string Reason
    }
    class InvalidPrintRunSizeException {
        +int RequestedSize
        +int MinSize
        +int MaxSize
    }
    class InvalidStageTransitionException {
        +ManuscriptStage From
        +ManuscriptStage To
    }
    class EntityNotFoundException {
        +string EntityName
        +Guid EntityId
    }
    class DataStorageException {
        +string FilePath
    }
    Exception <|-- PublishingException
    PublishingException <|-- ManuscriptRejectedException
    PublishingException <|-- InvalidPrintRunSizeException
    PublishingException <|-- InvalidStageTransitionException
    PublishingException <|-- EntityNotFoundException
    PublishingException <|-- DataStorageException
```

</details>

## Інтерфейсна частина (UI)

![Інтерфейсна частина (UI)](uml/4_ui.png)

<details><summary>Код Mermaid</summary>

```mermaid
classDiagram
    direction TB
    class Program {
        <<static>>
        -string DataFileName$
        -Main()$
        -LoadData(JsonFileService, PublishingService)$
        -BackUpCorruptedFile(string)$
        -SaveData(JsonFileService, PublishingService)$
    }
    class MenuBase {
        <<abstract>>
        #PublishingService Service
        #string Title*
        #string ExitText
        #CreateItems()* IReadOnlyList~MenuItem~
        +Run()
        -Execute(MenuItem)$
    }
    class MenuItem {
        +string Text
        +Action Action
    }
    class MainMenu {
        -JsonFileService~PublishingData~ _storage
        -Save()
    }
    class AuthorMenu
    class EditorMenu
    class ManuscriptMenu
    class PrintRunMenu {
        +PrintEstimate(PrintRunEstimate)$
    }
    class ReportMenu
    class InputHelper {
        <<static>>
        +ReadInt(string, int, int)$ int
        +ReadNonEmpty(string)$ string
        +Confirm(string)$ bool
        +ReadEnum~T~(string)$ T
        +ReadValid(string, Action~string~)$
        +ReadValidOptional(string, string, Action~string~)$
        +Choose~T~(IReadOnlyList~T~, Func, string)$ T?
        +ParseDecimal(string)$ decimal
        +ParseInt(string)$ int
    }
    class ConsoleHelper {
        <<static>>
        +Header(string)$
        +Success(string)$
        +Error(string)$
        +Warning(string)$
        +Money(decimal)$ string
    }
    class DisplayNames {
        <<static>>
        +Of~T~(T value)$ string
    }
    class EndOfInputException
    class PublishingService {
        <<PublishingHouse.Core>>
    }

    MenuBase <|-- MainMenu
    MenuBase <|-- AuthorMenu
    MenuBase <|-- EditorMenu
    MenuBase <|-- ManuscriptMenu
    MenuBase <|-- PrintRunMenu
    MenuBase <|-- ReportMenu
    MenuBase *-- MenuItem
    MenuBase --> PublishingService
    MainMenu ..> AuthorMenu
    MainMenu ..> EditorMenu
    MainMenu ..> ManuscriptMenu
    MainMenu ..> PrintRunMenu
    MainMenu ..> ReportMenu
    Program ..> MainMenu
    Program ..> PublishingService
    MenuBase ..> InputHelper
    MenuBase ..> ConsoleHelper
    InputHelper ..> EndOfInputException : throws
```

</details>

## Діаграма станів рукопису

![Діаграма станів рукопису](uml/5_stages.png)

<details><summary>Код Mermaid</summary>

```mermaid
stateDiagram-v2
    [*] --> Submitted : SubmitManuscript
    Submitted --> UnderReview : AdvanceStage
    UnderReview --> Editing : Review(схвалено)\nпотрібен редактор
    UnderReview --> Rejected : Review(відхилено)\nManuscriptRejectedException
    Editing --> Proofreading : AdvanceStage
    Proofreading --> Layout : AdvanceStage
    Layout --> ReadyForPrint : AdvanceStage
    ReadyForPrint --> Printed : CreatePrintRun\n100–100 000 прим.
    Printed --> Printed : CreatePrintRun\n(додатковий тираж)
    Rejected --> [*]
    Printed --> [*]
```

</details>

## Компоненти рішення

![Компоненти рішення](uml/6_components.png)

<details><summary>Код Mermaid</summary>

```mermaid
flowchart TB
    user([Користувач])
    subgraph UI["PublishingHouse.UI — Console App"]
        direction LR
        Program --> Menus["MainMenu та підменю"] --> Input["InputHelper / ConsoleHelper"]
    end
    subgraph Core["PublishingHouse.Core — Class Library (DLL)"]
        direction LR
        Json["JsonFileService&lt;T&gt;"]
        Service[PublishingService] --> Repos["Repository&lt;T&gt; × 4"]
        Service --> Calc[CostCalculator]
        Models["Моделі, інтерфейси, виключення"]
    end
    File[("publishing_data.json")]
    Tests["PublishingHouse.Tests — MSTest"]
    user --> UI
    UI -- "посилання на DLL" --> Core
    Json <--> File
    Tests --> Core
```

</details>
