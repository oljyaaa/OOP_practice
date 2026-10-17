# Lab 5 — Схеми роботи програми

## 1. Діаграма класів

```mermaid
classDiagram
    direction TB

    class ISkydiver {
        <<interface>>
        +string JumpWithParachute()
    }
    class ICakeMaker {
        <<interface>>
        +string MakeCake(params string[] ingredients)
    }

    class Person {
        <<abstract>>
        +string LastName
        +string FirstName
        #Person(lastName, firstName)
        +string JumpWithParachute()
        +string ToString()
    }

    class Student {
        +int Course
        +string StudentCard
        +DateTime BirthDate
        +int Age
        +Student(lastName, firstName, course, studentCard, birthDate, today)
        +ParseBirthDate(text)$ DateTime
        +CalculateAge(birthDate, today)$ int
        +bool IsBornInSpring()
        +bool IsBirthday(day)
        +bool CelebrateBirthday(day)
    }

    class Baker {
        +string MakeCake(ingredients)
    }
    class Entrepreneur {
        +string Business
        +string MakeCake(ingredients)
    }
    class CakeRecipe {
        <<static>>
        +Make(cook, ingredients)$ string
    }
    class Validator {
        <<static>>
        +RequireName(value, fieldName)$ string
    }

    class StudentService {
        -IStudentRepository _repository
        +StudentService(IStudentRepository repository)
        +List~Student~ GetAll(today)
        +List~Student~ GetFourthCourseSpringStudents(today)
        +int CountFourthCourseSpringStudents(today)
        +void Add(Student student)
        +ToStudent(record, today)$ Student
        +ToRecord(student)$ StudentRecord
    }

    class IStudentRepository {
        <<interface>>
        +List~StudentRecord~ Load()
        +void Save(List~StudentRecord~ students)
    }
    class JsonStudentRepository {
        -string _filePath
        +Load()
        +Save(students)
    }
    class StudentRecord {
        +string LastName
        +string FirstName
        +int Course
        +string StudentCard
        +string BirthDate
    }
    class StubStudentRepository {
        <<test stub>>
        +List~StudentRecord~ Records
        +int SaveCalls
    }
    class Program {
        <<PL>>
        -Main()
    }

    ISkydiver <|.. Person
    Person <|-- Student
    Person <|-- Baker
    Person <|-- Entrepreneur
    ICakeMaker <|.. Baker
    ICakeMaker <|.. Entrepreneur
    Baker ..> CakeRecipe
    Entrepreneur ..> CakeRecipe
    Person ..> Validator

    Program --> StudentService
    Program ..> JsonStudentRepository : створює
    StudentService --> IStudentRepository : DI
    StudentService ..> Student
    IStudentRepository <|.. JsonStudentRepository
    IStudentRepository <|.. StubStudentRepository
    IStudentRepository ..> StudentRecord
```

Рівні:
- **PL** (`Lab5.PL`) — консольне меню `Program`.
- **BLL** (`Lab5.BLL`) — `Student`, `Baker`, `Entrepreneur`, `StudentService`, перевірка даних.
- **DAL** (`Lab5.DAL`) — `StudentRecord`, `IStudentRepository`, `JsonStudentRepository` (JSON-серіалізація).
- **Tests** (`Lab5.Tests`) — MSTest, заглушка `StubStudentRepository` підставляється через DI.

## 2. Схема роботи програми

```mermaid
sequenceDiagram
    actor U as Користувач
    participant PL as Program (PL)
    participant S as StudentService (BLL)
    participant R as JsonStudentRepository (DAL)
    participant F as students.json

    U->>PL: пункт 2
    PL->>S: GetFourthCourseSpringStudents(today)
    S->>R: Load()
    R->>F: ReadAllText
    F-->>R: JSON
    R-->>S: List<StudentRecord>
    S->>S: ToStudent() + перевірка даних
    S->>S: фільтр: Course == 4 && IsBornInSpring()
    S-->>PL: List<Student>
    PL-->>U: кількість і дані студентів

    U->>PL: пункт 3 (додати)
    PL->>S: Add(new Student(...))
    S->>R: Load(), перевірка унікальності квитка
    S->>R: Save(records)
    R->>F: WriteAllText
```

## 3. Блок-схеми

### 3.1 Головне меню

```mermaid
flowchart TD
    A([Початок]) --> B[Створити StudentService з JsonStudentRepository]
    B --> C[/Показати меню, зчитати вибір/]
    C --> D{Вибір}
    D -- 1 --> E[Показати всіх студентів]
    D -- 2 --> F[Студенти 4 курсу, народжені навесні]
    D -- 3 --> G[Додати студента]
    D -- 4 --> H[Перевірити день народження]
    D -- 5 --> I[Пекар і підприємець]
    D -- 0 --> Z([Кінець])
    E & F & G & H & I --> K{Помилка?}
    K -- так --> L[/Вивести повідомлення про помилку/]
    K -- ні --> C
    L --> C
```

### 3.2 Підрахунок студентів 4-го курсу, народжених навесні

```mermaid
flowchart TD
    A([Початок]) --> B[records = repository.Load]
    B --> C[count = 0]
    C --> D{Є ще запис?}
    D -- ні --> H[/Повернути count/]
    D -- так --> E[student = ToStudent з перевіркою]
    E --> F{Course == 4 і місяць народження 3..5?}
    F -- так --> G[count++]
    F -- ні --> D
    G --> D
    H --> Z([Кінець])
```

### 3.3 Збільшення віку в день народження (`CelebrateBirthday`)

```mermaid
flowchart TD
    A([Початок: day]) --> B{day.Month == BirthDate.Month і day.Day == BirthDate.Day?}
    B -- ні --> C[/return false/]
    B -- так --> D[Age = Age + 1]
    D --> E[/return true/]
    C --> Z([Кінець])
    E --> Z
```

### 3.4 Приготування тістечка (`CakeRecipe.Make`)

```mermaid
flowchart TD
    A([Початок]) --> B{Інгредієнти null або є порожній?}
    B -- так --> X[/ArgumentException/]
    B -- ні --> C[Унікальні інгредієнти в нижньому регістрі]
    C --> D{Кількість >= 2?}
    D -- ні --> X
    D -- так --> E[/Повернути опис тістечка/]
    E --> Z([Кінець])
    X --> Z
```
