# Лабораторна робота 3.2 — варіант 8

1. Клас `MyString`:
   - `Value` — значення рядка;
   - `Length` — довжина;
   - пошук заданого символу;
   - розворот рядка;
   - додавання нового рядка;
   - виведення рядка.

2. Створено 4 об'єкти та показано роботу з:
   - масивом `MyString[]`;
   - non-generic колекцією `ArrayList`;
   - Generic-колекцією `List<MyString>`.

   Також продемонстровано:
   - додавання;
   - видалення;
   - оновлення;
   - пошук;
   - прохід по елементах.

3. Реалізовано узагальнене бінарне дерево `BinaryTree<T>`.

4. Реалізовано:
   - `IComparable<MyString>`;
   - сортування через `List.Sort()`;
   - власний ітератор `PostOrderEnumerator<T>` через `IEnumerator<T>`;
   - `IEnumerable<T>` у дереві;
   - обхід дерева за варіантом 8: **postorder**.

## Структура:

```text
Lab32_Variant8/
├── Lab32_Variant8.csproj
├── Program.cs
├── MyString.cs
├── BinaryTree.cs
├── PostOrderEnumerator.cs
└── README.md
```


## Блок-схема роботи програми

```mermaid
flowchart TD
    A([Початок]) --> B[Створюємо 4 об'єкти MyString]
    B --> C[Викликаємо DemonstrateClassMethods]
    C --> C1[FindCharacter\nReverse\nAppend\nPrint]
    C1 --> D[Викликаємо DemonstrateArray]
    D --> D1[Створюємо MyString[]\nДодаємо\nОновлюємо\nШукаємо\nВидаляємо]
    D1 --> E[Викликаємо DemonstrateNonGenericCollection]
    E --> E1[Робота з ArrayList\nAdd/Update/Search/Remove]
    E1 --> F[Викликаємо DemonstrateGenericCollection]
    F --> F1[Робота з List<MyString>\nAdd/Update/Search/Remove]
    F1 --> G[Викликаємо DemonstrateSorting]
    G --> G1[Порівняння через IComparable<MyString>\nList.Sort()]
    G1 --> H[Викликаємо DemonstrateBinaryTree]
    H --> H1[Створюємо BinaryTree<MyString>\nДодаємо вузли]
    H1 --> I[PostOrderEnumerator обходить дерево]
    I --> I1[postorder: ліве → праве → корінь]
    I1 --> J[Виводимо результати в консоль]
    J --> K[Висновки про масив, ArrayList та List<MyString>]
    K --> L([Кінець])

    subgraph MyString
        C1
    end

    subgraph Collections
        D1
        E1
        F1
    end

    subgraph SortingAndTree
        G1
        H1
        I1
    end
```

```bash
dotnet --version
dotnet build
dotnet run
```
