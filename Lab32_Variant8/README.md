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


## Концептуальна схема проєкту

```mermaid
flowchart LR
    Project[Lab32_variant8] --> Program[Program.cs]
    Project --> ProjectFile[Lab32_Variant8.csproj]
    Project --> StringFile[MyString.cs]
    Project --> TreeFile[BinaryTree.cs]
    Project --> EnumeratorFile[PostOrderEnumerator.cs]

    StringFile -->|оголошує| MyString[MyString]
    TreeFile -->|оголошує| Tree[BinaryTree]
    EnumeratorFile -->|оголошує| Enumerator[PostOrderEnumerator]

    Program -->|створює та використовує| MyString[MyString]
    Program -->|працює з| Collections[Масив, ArrayList, List]
    Collections -->|зберігають| MyString
    Program -->|створює| Tree
    Tree -->|зберігає значення| MyString

    MyString -->|реалізує| Comparable[IComparable]
    Comparable -->|задає порядок для| Tree

    Tree -->|містить| Node[Node]
    Tree -->|реалізує| Enumerable[IEnumerable]
    Tree -->|створює для foreach| Enumerator
    Enumerator -->|отримує кореневий| Node
    Enumerator -->|реалізує| IEnumerator[IEnumerator]
```

`Program.cs` є точкою входу: він створює об'єкти `MyString`, демонструє роботу
колекцій і створює `BinaryTree`. Дерево зберігає `MyString` у вузлах `Node`.
`PostOrderEnumerator` забезпечує обхід дерева в порядку postorder під час
`foreach`.

```bash
dotnet --version
dotnet build
dotnet run
```
