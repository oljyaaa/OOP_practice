# Діаграма класів

```mermaid
classDiagram
    class Person {
        <<abstract>>
        +string FirstName
        +string LastName
        +string UniqueId
        +ToRecord() PersonRecord
    }

    class Student {
        +int Course
        +string StudentId
        +DateTime BirthDate
        +Study()
    }

    class Baker {
        +string PersonId
    }

    class Entrepreneur {
        +string PersonId
    }

    class IParachuteJump {
        <<interface>>
        +JumpWithParachute()
    }

    class PersonRecord {
        +string TypeName
        +string ObjectName
        +string[] AttributeNames
        +string[] AttributeValues
    }

    class IPersonFactory {
        <<interface>>
        +string RecordTypeName
        +Create(PersonRecord) Person
    }

    class PersonFactoryRegistry {
        +Create(PersonRecord) Person
    }

    class FileManager {
        +ReadAll() Person[]
        +Append(Person)
        +OverwriteAll(Person[])
    }

    class IPersonInputHandler {
        <<interface>>
        +CreatePerson() Person
    }

    class IPersonAction {
        <<interface>>
        +CanExecute(Person) bool
        +Execute(Person) string
    }

    Person <|-- Student
    Person <|-- Baker
    Person <|-- Entrepreneur
    Student ..|> IParachuteJump
    Baker ..|> IParachuteJump
    Entrepreneur ..|> IParachuteJump
    Student --> PersonRecord
    Baker --> PersonRecord
    Entrepreneur --> PersonRecord
    IPersonFactory <|.. StudentFactory
    IPersonFactory <|.. BakerFactory
    IPersonFactory <|.. EntrepreneurFactory
    PersonFactoryRegistry --> IPersonFactory
    FileManager --> PersonRecord
    FileManager --> PersonFactoryRegistry
    IPersonInputHandler <|.. StudentInputHandler
    IPersonInputHandler <|.. BakerInputHandler
    IPersonInputHandler <|.. EntrepreneurInputHandler
    IPersonAction <|.. ParachuteJumpAction
```

## Розширення без зміни існуючих класів

Щоб додати новий тип особи, потрібно створити лише новий клас-нащадок `Person`, його фабрику
`IPersonFactory` та обробник введення `IPersonInputHandler`. Реєстри знаходять їх автоматично,
тому `FileManager`, `ConsoleMenu` і вже створені класи не потрібно змінювати.

Щоб додати нову дію, потрібно створити новий клас `IPersonAction`. Меню також знайде його
автоматично та покаже окремим пунктом.
