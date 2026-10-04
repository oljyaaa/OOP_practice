# Лабораторна робота 3.3, варіант 8 — блок-схеми

## 1. Трирівнева архітектура рішення

Кожен проєкт посилається тільки на рівень нижче.

```mermaid
flowchart LR
    APP["<b>Lab3.App</b><br/>Program.Main()<br/>викликає лише Menu.MainMenu()"]
    PL["<b>Lab3.Presentation (PL)</b><br/>Menu<br/>StudentInput, BakerInput, EntrepreneurInput"]
    BLL["<b>Lab3.Business (BLL)</b><br/>EntityService<br/>Student, Baker, Entrepreneur, MyString<br/>власні винятки"]
    DAL["<b>Lab3.Data (DAL)</b><br/>EntityContext<br/>DataProvider + 4 провайдери<br/>StudentEntity, BakerEntity, EntrepreneurEntity, StringEntity"]
    FILES[("Файли<br/>.bin / .xml / .json / .txt")]

    APP -->|посилається на| PL
    PL -->|посилається на| BLL
    BLL -->|посилається на| DAL
    DAL -->|читає / записує| FILES
```

## 2. Класи рівня доступу до даних (DAL)

```mermaid
classDiagram
    class IEntity {
        <<interface>>
        +ToFields() Dictionary
        +FromFields(fields)
        +IsValid() bool
    }
    class StudentEntity
    class BakerEntity
    class EntrepreneurEntity
    class StringEntity
    IEntity <|.. StudentEntity
    IEntity <|.. BakerEntity
    IEntity <|.. EntrepreneurEntity
    IEntity <|.. StringEntity

    class DataProvider {
        <<abstract>>
        +Write~T~(items, filePath)
        +Read~T~(filePath) List~T~
    }
    class BinaryDataProvider
    class XmlDataProvider
    class JsonDataProvider
    class CustomDataProvider
    DataProvider <|-- BinaryDataProvider
    DataProvider <|-- XmlDataProvider
    DataProvider <|-- JsonDataProvider
    DataProvider <|-- CustomDataProvider

    class EntityContext {
        +Create~T~()
        +Open~T~() / OpenArray~T~()
        +Update~T~() / UpdateArray~T~()
        +Close()
        +Delete()
        -GetProvider(format) DataProvider
    }
    EntityContext --> DataProvider : обирає за форматом
    DataProvider ..> IEntity : працює з
```

## 3. Класи рівня бізнес-логіки (BLL)

```mermaid
classDiagram
    class ISkydiver {
        <<interface>>
        +JumpWithParachute() string
    }
    class Person {
        <<abstract>>
        +Surname
        +FirstName
        +GetInfo() string
    }
    class Student {
        +Course
        +StudentCard
        +BirthDate
        +IsBornInSpring() bool
    }
    class Baker {
        +Bakery
    }
    class Entrepreneur {
        +Business
    }
    ISkydiver <|.. Person
    Person <|-- Student
    Person <|-- Baker
    Person <|-- Entrepreneur

    class MyString {
        +Value
        +Length
        +FindSymbol(symbol) int
        +Reverse()
        +Append(text)
        +GetInfo() string
    }

    class EntityService {
        -context EntityContext
        +Add / Find / Remove
        +Save / Load
        +GetSpringStudentsOfFourthCourse(options)
        +CompareArrayWithCollection(...)
    }
    EntityService --> Student
    EntityService --> Baker
    EntityService --> Entrepreneur
    EntityService --> MyString
```

## 4. Загальна блок-схема роботи програми

```mermaid
flowchart TD
    S([Старт]) --> M["Program.Main()<br/>Menu.MainMenu()"]
    M --> MM[/"Головне меню:<br/>1 Рядки, 2 Студенти, 3 Пекарі,<br/>4 Підприємці, 5 Файли, 0 Вихід"/]
    MM --> C{Вибір}
    C -->|0| E([Кінець])
    C -->|1| P1["Меню рядків (частина 1)"]
    C -->|2| P2["Меню студентів"]
    C -->|3| P3["Меню пекарів"]
    C -->|4| P4["Меню підприємців"]
    C -->|5| P5["Створення / видалення файлу"]
    P1 --> SVC["Виклик методу EntityService"]
    P2 --> SVC
    P3 --> SVC
    P4 --> SVC
    P5 --> SVC
    SVC --> OK{Виняток?}
    OK -->|ні| R[/"Вивести результат"/]
    OK -->|так| X[/"catch у MainMenu:<br/>вивести повідомлення про помилку"/]
    R --> MM
    X --> MM
```

## 5. Збереження даних у файл (як рівні взаємодіють)

```mermaid
sequenceDiagram
    actor U as Користувач
    participant M as Menu (PL)
    participant S as EntityService (BLL)
    participant C as EntityContext (DAL)
    participant P as DataProvider (DAL)
    participant F as Файл

    U->>M: формат і ім'я файлу
    M->>S: SaveStudents(StorageOptions)
    S->>S: Student → StudentEntity
    S->>C: Update(entities, filePath, format)
    C->>C: GetProvider(format)
    C->>P: Write(entities, filePath)
    P->>F: серіалізація
    S->>C: Close(filePath)
    S-->>M: успіх або StorageOperationException
    M-->>U: повідомлення
```

## 6. Алгоритм: студенти 4-го курсу, які народилися навесні (з файлу)

```mermaid
flowchart TD
    A([Початок]) --> B[/"Ввести формат та ім'я файлу"/]
    B --> C["EntityContext.Open: прочитати StudentEntity з файлу"]
    C --> D["Перетворити StudentEntity → Student"]
    D --> E["result = порожній список"]
    E --> F{"Є ще студент?"}
    F -->|так| G{"Course == 4<br/>і місяць народження 3, 4 або 5?"}
    G -->|так| H["Додати студента до result"]
    G -->|ні| F
    H --> F
    F -->|ні| I[/"Вивести result.Count і дані студентів"/]
    I --> J([Кінець])
```

## 7. Частина 1: серіалізація масиву і колекції, порівняння

```mermaid
flowchart TD
    A([Початок]) --> B["4 об'єкти MyString"]
    B --> C["Зберегти як масив MyString[] → файл 1"]
    B --> D["Зберегти як колекцію List&lt;MyString&gt; → файл 2"]
    C --> E["Відновити з файлу 1 у новий масив"]
    D --> F["Відновити з файлу 2 у нову колекцію"]
    E --> G{"Однакова кількість<br/>і значення всіх елементів?"}
    F --> G
    G -->|так| H[/"Дані однакові"/]
    G -->|ні| I[/"Дані відрізняються"/]
    H --> J([Кінець])
    I --> J
```

## 8. Вибір провайдера серіалізації

```mermaid
flowchart LR
    F{"Формат"} -->|1 Binary| B["BinaryDataProvider<br/>BinaryWriter / BinaryReader"]
    F -->|2 XML| X["XmlDataProvider<br/>XmlSerializer"]
    F -->|3 JSON| J["JsonDataProvider<br/>System.Text.Json"]
    F -->|4 Custom| C["CustomDataProvider<br/>рядки Назва=Значення"]
```
