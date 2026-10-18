// Генерує пояснювальну записку docs/Пояснювальна_записка.docx.
// Фрагменти коду беруться безпосередньо з вихідних файлів, тож лістинги завжди відповідають проєкту.
// Запуск: NODE_PATH=<папка з node_modules/docx> node docs/tools/report.js
const fs = require('fs');
const path = require('path');
const {
  Document, Packer, Paragraph, TextRun, HeadingLevel, AlignmentType, ImageRun, PageBreak,
  Table, TableRow, TableCell, WidthType, BorderStyle, ShadingType, TableOfContents,
  Footer, PageNumber, LevelFormat, LineRuleType, PageOrientation,
} = require('docx');

const ROOT = path.resolve(__dirname, '..', '..');
const DOCS = path.join(ROOT, 'docs');
const OUT = path.join(DOCS, 'Пояснювальна_записка.docx');

// ---------- Дані титульного листа (заглушки виділено жовтим) ----------
const STUDENT = '[ПІБ студента]';
const GROUP = '[група]';
const TEACHER = '[посада, ПІБ викладача]';
const YEAR = '2026';

// ---------- Розмітка сторінки (ДСТУ 3008: поля 30/15/20/20 мм) ----------
const MM = 56.7; // DXA в 1 мм
const PAGE = { width: 11906, height: 16838 };
const MARGIN = { left: Math.round(30 * MM), right: Math.round(15 * MM), top: Math.round(20 * MM), bottom: Math.round(20 * MM) };
const CONTENT_WIDTH = PAGE.width - MARGIN.left - MARGIN.right; // ≈ 9354 DXA
const MAX_IMG_W = 620; // px (≈ 165 мм)
const MAX_IMG_H = 860;

const FONT = 'Times New Roman';
const CODE_FONT = 'Consolas';

// ---------- Допоміжні функції ----------
let figureNo = {};
let tableNo = {};
let listingNo = {};
let currentSection = 0;

function runs(text, opts = {}) {
  // Підтримка **жирного** і `коду` всередині абзацу
  const parts = text.split(/(\*\*[^*]+\*\*|`[^`]+`)/g).filter(Boolean);
  return parts.map(p => {
    if (p.startsWith('**')) return new TextRun({ text: p.slice(2, -2), bold: true, ...opts });
    if (p.startsWith('`')) return new TextRun({ text: p.slice(1, -1), font: CODE_FONT, size: 24, ...opts });
    return new TextRun({ text: p, ...opts });
  });
}

const P = (text, opts = {}) => new Paragraph({
  children: runs(text),
  alignment: AlignmentType.JUSTIFIED,
  indent: { firstLine: Math.round(12.5 * MM) },
  ...opts,
});

const Bullet = text => new Paragraph({
  children: runs(text),
  numbering: { reference: 'dash', level: 0 },
  alignment: AlignmentType.JUSTIFIED,
});

const Numbered = (text, ref) => new Paragraph({
  children: runs(text),
  numbering: { reference: ref, level: 0 },
  alignment: AlignmentType.JUSTIFIED,
});

function H1(text, { numbered = true } = {}) {
  if (numbered) {
    currentSection++;
    figureNo[currentSection] = 0;
    tableNo[currentSection] = 0;
    listingNo[currentSection] = 0;
  }
  return new Paragraph({
    heading: HeadingLevel.HEADING_1,
    pageBreakBefore: true,
    alignment: AlignmentType.CENTER,
    children: [new TextRun(numbered ? `${currentSection} ${text.toUpperCase()}` : text.toUpperCase())],
  });
}

let subNo = 0;
function H2(text) {
  subNo = (H2.last === currentSection) ? subNo + 1 : 1;
  H2.last = currentSection;
  return new Paragraph({
    heading: HeadingLevel.HEADING_2,
    indent: { firstLine: Math.round(12.5 * MM) },
    children: [new TextRun(`${currentSection}.${subNo} ${text}`)],
  });
}

function pngSize(file) {
  const b = fs.readFileSync(file);
  return { w: b.readUInt32BE(16), h: b.readUInt32BE(20), data: b };
}

function Figure(relPath, caption, { maxW = MAX_IMG_W, maxH = MAX_IMG_H, dpr = 1, landscape = false } = {}) {
  if (landscape) { maxW = 950; maxH = 600; }
  const { w, h, data } = pngSize(path.join(DOCS, relPath));
  // dpr = 2 для знімків, відрендерених з подвійною щільністю: не збільшуємо їх понад природний розмір
  const scale = Math.min(maxW / w, maxH / h, 1 / dpr);
  const n = ++figureNo[currentSection];
  const paragraphs = [
    new Paragraph({
      alignment: AlignmentType.CENTER,
      spacing: { before: 120, after: 0 },
      keepNext: true,
      children: [new ImageRun({ type: 'png', data, transformation: { width: Math.round(w * scale), height: Math.round(h * scale) } })],
    }),
    new Paragraph({
      alignment: AlignmentType.CENTER,
      spacing: { after: 240 },
      children: [new TextRun(`Рисунок ${currentSection}.${n} – ${caption}`)],
    }),
  ];
  // Широка діаграма виноситься на окрему альбомну сторінку (див. збирання секцій нижче)
  return landscape ? [{ landscape: paragraphs }] : paragraphs;
}

function cell(text, width, { header = false, align = AlignmentType.LEFT } = {}) {
  return new TableCell({
    width: { size: width, type: WidthType.DXA },
    margins: { top: 40, bottom: 40, left: 90, right: 90 },
    shading: header ? { type: ShadingType.CLEAR, color: 'auto', fill: 'E7E6E6' } : undefined,
    children: [new Paragraph({
      alignment: header ? AlignmentType.CENTER : align,
      spacing: { line: 240 },
      children: runs(text, { size: 24, bold: header || undefined }),
    })],
  });
}

function TableBlock(caption, headers, rows, fractions) {
  const n = ++tableNo[currentSection];
  const widths = fractions.map(f => Math.floor(CONTENT_WIDTH * f));
  widths[widths.length - 1] += CONTENT_WIDTH - widths.reduce((a, b) => a + b, 0);
  const border = { style: BorderStyle.SINGLE, size: 4, color: '000000' };
  return [
    new Paragraph({
      keepNext: true,
      spacing: { before: 120 },
      children: [new TextRun(`Таблиця ${currentSection}.${n} – ${caption}`)],
    }),
    new Table({
      width: { size: CONTENT_WIDTH, type: WidthType.DXA },
      columnWidths: widths,
      borders: { top: border, bottom: border, left: border, right: border, insideHorizontal: border, insideVertical: border },
      rows: [
        new TableRow({ tableHeader: true, children: headers.map((t, i) => cell(t, widths[i], { header: true })) }),
        ...rows.map(r => new TableRow({ cantSplit: true, children: r.map((t, i) => cell(t, widths[i])) })),
      ],
    }),
    new Paragraph({ spacing: { after: 120 }, children: [] }),
  ];
}

// ---------- Витяг коду з вихідних файлів ----------
function readSource(rel) {
  return fs.readFileSync(path.join(ROOT, rel), 'utf8').replace(/^﻿/, '').replace(/\r/g, '').split('\n');
}

/** Повертає блок, що починається з рядка з `startPattern`, до закриваючої дужки того ж рівня відступу. */
function extractBlock(rel, startPattern, { withDocComment = true } = {}) {
  const lines = readSource(rel);
  let start = lines.findIndex(l => l.includes(startPattern));
  if (start < 0) throw new Error(`Не знайдено «${startPattern}» у ${rel}`);
  const indent = lines[start].match(/^\s*/)[0];
  let end = start;
  // однорядковий член (=> ... ;) або блок у фігурних дужках
  if (!lines[start].trimEnd().endsWith(';')) {
    end = lines.findIndex((l, i) => i > start && (l === `${indent}}` || l === `${indent}};`));
    if (end < 0) end = start;
  }
  if (withDocComment) {
    while (start > 0 && /^\s*(\/\/\/|\[)/.test(lines[start - 1])) start--;
  }
  return dedent(lines.slice(start, end + 1));
}

/** Увесь файл без using-директив і обгортки namespace. */
function extractFile(rel) {
  const lines = readSource(rel);
  const nsIndex = lines.findIndex(l => l.startsWith('namespace'));
  const body = lines.slice(nsIndex + 2, lines.lastIndexOf('}'));
  return dedent(body);
}

function dedent(lines) {
  const min = Math.min(...lines.filter(l => l.trim()).map(l => l.match(/^\s*/)[0].length));
  return lines.map(l => l.slice(min)).join('\n').replace(/\n+$/, '');
}

function Listing(caption, file, code) {
  const n = ++listingNo[currentSection];
  const out = [new Paragraph({
    keepNext: true,
    spacing: { before: 160, after: 60 },
    children: [new TextRun(`Лістинг ${currentSection}.${n} – ${caption} (`), new TextRun({ text: file, italics: true }), new TextRun(')')],
  })];
  const codeLines = code.split('\n');
  codeLines.forEach((line, i) => out.push(new Paragraph({
    keepLines: true,
    keepNext: i < codeLines.length - 1 && codeLines.length < 25,
    spacing: { line: 240, lineRule: LineRuleType.AUTO, before: 0, after: 0 },
    shading: { type: ShadingType.CLEAR, color: 'auto', fill: 'F4F4F4' },
    indent: { left: 0 },
    children: [new TextRun({ text: line.length ? line.replace(/\t/g, '    ') : ' ', font: CODE_FONT, size: 18 })],
  })));
  out.push(new Paragraph({ spacing: { after: 120 }, children: [] }));
  return out;
}

// ---------- Титульний лист ----------
function center(text, opts = {}) {
  return new Paragraph({ alignment: AlignmentType.CENTER, spacing: { line: 300, after: 0 }, children: [new TextRun({ text, ...opts })] });
}
function placeholder(label, value) {
  return new Paragraph({
    alignment: AlignmentType.LEFT,
    indent: { left: Math.round(85 * MM) },
    spacing: { line: 300, after: 0 },
    children: [new TextRun(label), new TextRun({ text: value, highlight: value.startsWith('[') ? 'yellow' : undefined })],
  });
}
const blank = (n = 1) => Array.from({ length: n }, () => new Paragraph({ spacing: { line: 300, after: 0 }, children: [] }));

const titlePage = [
  center('МІНІСТЕРСТВО ОСВІТИ І НАУКИ УКРАЇНИ'),
  center('НАЦІОНАЛЬНИЙ УНІВЕРСИТЕТ «КИЇВСЬКИЙ АВІАЦІЙНИЙ ІНСТИТУТ»'),
  center('Кафедра інженерії програмного забезпечення'),
  ...blank(6),
  center('КУРСОВА РОБОТА', { bold: true, size: 32 }),
  center('з дисципліни «Об’єктно-орієнтоване програмування»'),
  ...blank(1),
  center('на тему:'),
  center('«Книжкове видавництво (Publishing House)»', { bold: true }),
  center('Варіант 23'),
  ...blank(6),
  placeholder('Виконала: студентка групи ', GROUP),
  placeholder('', STUDENT),
  ...blank(1),
  placeholder('Перевірив: ', TEACHER),
  ...blank(1),
  placeholder('Оцінка: ', '____________'),
  ...blank(6),
  center(`Київ – ${YEAR}`),
];

// ---------- Зміст ----------
const tocPage = [
  new Paragraph({ alignment: AlignmentType.CENTER, pageBreakBefore: true, spacing: { after: 240 }, children: [new TextRun({ text: 'ЗМІСТ', bold: true })] }),
  new TableOfContents('Зміст', { hyperlink: true, headingStyleRange: '1-2' }),
  new Paragraph({ children: [new TextRun({ text: 'Якщо зміст порожній, оновіть його в Word: правою кнопкою на зміст → «Оновити поле».', italics: true, size: 20, color: '808080' })] }),
];

// ---------- Вступ ----------
const intro = [
  H1('Вступ', { numbered: false }),
  P('Об’єктно-орієнтоване програмування дає змогу описати предметну область системою класів, що відповідають реальним сутностям, і розподілити між ними відповідальність. Мета курсової роботи — спроєктувати та реалізувати консольне застосування мовою C# для накопичення та обробки даних книжкового видавництва із застосуванням інкапсуляції, наслідування, поліморфізму, інтерфейсів, узагальнених колекцій, власних виключень і серіалізації.'),
  P('Застосування моделює шлях книги від подання рукопису автором до друку тиражу: рецензування, редагування, коректуру, верстку, а також калькуляцію собівартості тиражу та прогнозованого прибутку. Дані зберігаються між запусками у JSON-файлі.'),
  P('Для отримання максимальної оцінки виконано всі вимоги, позначені «*», «**» та «***»: використано узагальнені колекції `List<T>` і `Dictionary<TKey, TValue>`, бізнес-логіку винесено в окрему бібліотеку DLL, створено власні класи виключень, реалізовано збереження та завантаження даних через серіалізацію `System.Text.Json`. Додатково написано 70 модульних тестів MSTest.'),
];

// ---------- 1. Опис завдання ----------
const section1 = [
  H1('Опис завдання'),
  H2('Варіант завдання'),
  P('Варіант 23 — «Книжкове видавництво (Publishing House)». Умова варіанта наведена в таблиці 1.1.'),
  ...TableBlock('Умова варіанта 23', ['Параметр', 'Зміст'], [
    ['Предметна область', 'Книжкове видавництво (Publishing House)'],
    ['Основні сутності', '`Manuscript`, `Author`, `Editor`, `PrintRun`'],
    ['Бізнес-логіка', 'Етапи підготовки книги до друку, калькуляція собівартості тиражу та прогнозованого прибутку'],
    ['Власні виключення', '`ManuscriptRejectedException` (рукопис не пройшов рецензування), `InvalidPrintRunSizeException`'],
  ], [0.3, 0.7]),
  H2('Загальні вимоги до курсової роботи'),
  P('Відповідно до завдання кафедри ІПЗ застосування має відповідати таким вимогам:'),
  ...[
    'консольне застосування з командним рядком;',
    'система класів, в основу якої покладено логічну структуру даних варіанта;',
    'зберігання даних в узагальнених колекціях (**);',
    'опис елементарних операцій через інтерфейси та абстрактні класи — стандартні (`IComparer<T>`, `IComparable<T>`) і власні;',
    'збереження накопичених даних у файл перед виходом і завантаження на початку роботи (*) через серіалізацію C# (***);',
    'обов’язковий контроль коректності введених даних;',
    'перевірка виняткових ситуацій, власні класи виключень (*);',
    'бізнес-логіка в окремому проєкті — DLL-бібліотеці, підключеній до проєкту інтерфейсу (**);',
    'правила «хорошого» стилю: мнемонічні ідентифікатори, жодних public полів, кожен клас в окремому файлі.',
  ].map(Bullet),
  H2('Розшифровка вимог і спосіб реалізації'),
  P('У таблиці 1.2 показано, якими засобами реалізовано кожну вимогу.'),
  ...TableBlock('Відповідність вимог і реалізації', ['№', 'Вимога', 'Реалізація'], [
    ['1', 'Консольне застосування', 'Проєкт `PublishingHouse.UI` (Console App, .NET 10) з текстовим меню'],
    ['2', 'Система класів домену', 'Абстрактні `EntityBase`, `Person`; класи `Author`, `Editor`, `Manuscript`, `PrintRun`; переліки `ManuscriptStage`, `Genre`, `PaperType`, `CoverType`'],
    ['3', 'Узагальнені колекції', '`Repository<T>` на основі `List<T>` з індексом `Dictionary<Guid, T>`; `List<T>` у `PublishingData`'],
    ['4', 'Інтерфейси та абстрактні класи', 'Власні: `IRepository<T>`, `ICostCalculator`, `IReviewable`. Стандартні: `IComparable<Manuscript>`, чотири реалізації `IComparer<T>`'],
    ['5', 'Файли та серіалізація', '`JsonFileService<T>` (`System.Text.Json`): завантаження при старті, збереження при виході, атомарний запис через тимчасовий файл'],
    ['6', 'Контроль вводу', 'Валідація у setter-ах властивостей моделей і `InputHelper` з повторним запитом'],
    ['7', 'Виняткові ситуації', 'Базове `PublishingException` і п’ять похідних виключень; перехоплення в `MenuBase` і `Program`'],
    ['8', 'Бізнес-логіка в DLL', '`PublishingHouse.Core` (Class Library), підключена до `PublishingHouse.UI`'],
    ['9', 'Стиль коду', 'Приватні поля + властивості, XML-коментарі, один клас — один файл, 0 попереджень компілятора'],
  ], [0.06, 0.3, 0.64]),
  H2('Предметна область'),
  P('Автор подає до видавництва рукопис. Рукопис проходить рецензування: у разі відмови він отримує стан «Відхилено», у разі схвалення — потрапляє до призначеного редактора. Далі рукопис послідовно проходить етапи редагування, коректури та верстки, після чого стає готовим до друку. Для готового рукопису створюється тираж, для якого розраховуються собівартість і прогнозований прибуток. Для одного рукопису можна надрукувати кілька тиражів.'),
  P('Допустимий розмір тиражу — від 100 до 100 000 примірників. Роялті автора — від 0 до 50 % виручки. Ставку редактора задано за одну сторінку рукопису.'),
];

// ---------- 2. Бізнес-частина ----------
const section2 = [
  H1('Опис бізнес-частини проєкту'),
  H2('Структура рішення'),
  P('Рішення `PublishingHouse.slnx` складається з трьох проєктів (рисунок 2.1):'),
  Bullet('`PublishingHouse.Core` — бібліотека класів (DLL) з моделями, інтерфейсами, репозиторіями, сервісами та виключеннями;'),
  Bullet('`PublishingHouse.UI` — консольне застосування, що посилається на DLL і реалізує меню, ввід і виведення;'),
  Bullet('`PublishingHouse.Tests` — модульні тести MSTest для бізнес-логіки.'),
  ...Figure('uml/6_components.png', 'Компоненти рішення'),
  ...TableBlock('Структура бібліотеки PublishingHouse.Core', ['Папка', 'Вміст'], [
    ['Models', '`EntityBase`, `Person`, `Author`, `Editor`, `Manuscript`, `PrintRun`, `PrintRunEstimate`, `PublishingData`'],
    ['Models/Enums', '`ManuscriptStage`, `Genre`, `PaperType`, `CoverType`'],
    ['Interfaces', '`IRepository<T>`, `ICostCalculator`, `IReviewable`'],
    ['Repositories', '`Repository<T>`'],
    ['Services', '`PublishingService`, `CostCalculator`, `JsonFileService<T>`'],
    ['Comparers', '`PersonByNameComparer`, `ManuscriptByPageCountComparer`, `ManuscriptByStageComparer`, `PrintRunByProfitComparer`'],
    ['Exceptions', '`PublishingException` і похідні виключення'],
  ], [0.25, 0.75]),
  H2('Моделі предметної області'),
  P('Діаграму класів моделей наведено на рисунку 2.2. Усі сутності успадковуються від абстрактного класу `EntityBase`, який містить унікальний ідентифікатор `Id` типу `Guid` та абстрактний метод `GetSummary()`. Перевизначений `ToString()` викликає `GetSummary()`, тож кожна сутність сама визначає свій короткий опис — це приклад поліморфізму.'),
  P('Абстрактний клас `Person` узагальнює спільні дані автора й редактора: ім’я, прізвище, email та обчислювану властивість `FullName`. Абстрактна властивість `Role` перевизначається в нащадках. `Author` додає відсоток роялті, `Editor` — ставку за сторінку та спеціалізацію (жанр).'),
  P('`Manuscript` зберігає назву, жанр, кількість сторінок, посилання на автора і редактора (за ідентифікаторами), поточний етап, дату подання та висновок рецензента. Клас реалізує стандартний інтерфейс `IComparable<Manuscript>` (природний порядок — за назвою) і власний інтерфейс `IReviewable`. `PrintRun` описує тираж: кількість примірників, тип паперу й обкладинки, роздрібну ціну та прогнозований відсоток продажу.'),
  ...Figure('uml/1_models.png', 'Діаграма класів моделей предметної області', { maxH: 820 }),
  P('**Інкапсуляція.** Жоден клас не має public полів. Дані зберігаються в приватних полях, доступ до них — через властивості, setter яких перевіряє значення і кидає `ArgumentException` (або `ArgumentOutOfRangeException`) у разі помилки. Таким чином об’єкт не може опинитися в некоректному стані, а ці самі правила спрацьовують і під час десеріалізації з файлу. Обмеження наведено в таблиці 2.2.'),
  ...TableBlock('Правила валідації властивостей моделей', ['Властивість', 'Правило'], [
    ['`Person.FirstName`, `LastName`', 'Не порожнє, до 50 символів, лише літери, дефіс, апостроф, пробіл'],
    ['`Person.Email`', 'Один символ «@», після нього домен із крапкою, без пробілів'],
    ['`Author.RoyaltyPercent`', '0–50 %'],
    ['`Editor.RatePerPage`', 'Більше 0, не більше 1000 грн'],
    ['`Manuscript.Title`', 'Не порожня, до 150 символів'],
    ['`Manuscript.PageCount`', '1–5000'],
    ['`Manuscript.AuthorId`, `PrintRun.ManuscriptId`, `EntityBase.Id`', 'Не `Guid.Empty`'],
    ['`PrintRun.Quantity`', 'Додатна (межі 100–100 000 перевіряє сервіс)'],
    ['`PrintRun.RetailPrice`', 'Більше 0, не більше 10 000 грн'],
    ['`PrintRun.ExpectedSellThroughPercent`', '0–100 %'],
  ], [0.45, 0.55]),
  H2('Інтерфейси, репозиторій і колекції'),
  P('Операції над даними описано інтерфейсами (рисунок 2.3). Узагальнений інтерфейс `IRepository<T>` з обмеженням `where T : EntityBase` визначає додавання, пошук за ідентифікатором, видалення, фільтрацію за предикатом `Predicate<T>` та повну заміну вмісту. Реалізація `Repository<T>` зберігає елементи в `List<T>` (порядок додавання) і паралельно в `Dictionary<Guid, T>`, що дає пошук за ідентифікатором за O(1).'),
  P('Метод `ReplaceAll`, яким користуються під час завантаження з файлу, спочатку будує новий індекс і лише після успішної перевірки на дублікати замінює дані — якщо файл містить помилку, поточні дані не втрачаються.'),
  P('Інтерфейс `ICostCalculator` відокремлює алгоритм калькуляції від сервісу: `PublishingService` отримує калькулятор через конструктор, тому його можна замінити іншою реалізацією (наприклад, з іншими тарифами) без зміни сервісу. Аналогічно сервіс приймає чотири репозиторії через інтерфейс `IRepository<T>`.'),
  P('Діаграму класів сервісного шару наведено на рисунку 2.3 (альбомна сторінка).'),
  ...Figure('uml/2_services.png', 'Діаграма класів сервісу, репозиторію та калькулятора', { landscape: true }),
  H2('Компаратори'),
  P('Для сортування реалізовано чотири класи стандартного інтерфейсу `IComparer<T>`:'),
  Bullet('`PersonByNameComparer` — за прізвищем, потім за ім’ям. Завдяки контраваріантності `IComparer<in T>` один клас сортує і `List<Author>`, і `List<Editor>`;'),
  Bullet('`ManuscriptByPageCountComparer` — за кількістю сторінок;'),
  Bullet('`ManuscriptByStageComparer` — за етапом, а в межах етапу — за датою подання;'),
  Bullet('`PrintRunByProfitComparer` — за прогнозованим прибутком за спаданням; прибуток обчислює делегат `Func<PrintRun, decimal>`, переданий у конструктор.'),
  P('Компаратори та узагальнений сервіс серіалізації показано на рисунку 2.4.'),
  ...Figure('uml/2b_comparers_json.png', 'Компаратори та сервіс JSON-серіалізації'),
  H2('Власні виключення'),
  P('Ієрархію виключень наведено на рисунку 2.5. Усі доменні виключення успадковуються від `PublishingException`, тому інтерфейс може перехопити будь-яку помилку бізнес-логіки одним блоком `catch`. Кожне виключення зберігає дані про ситуацію у властивостях, доступних лише для читання (таблиця 2.3).'),
  ...Figure('uml/3_exceptions.png', 'Ієрархія власних виключень'),
  ...TableBlock('Власні виключення', ['Виключення', 'Коли виникає', 'Властивості'], [
    ['`ManuscriptRejectedException`', 'Рецензент відхилив рукопис', '`ManuscriptId`, `ManuscriptTitle`, `Reason`'],
    ['`InvalidPrintRunSizeException`', 'Тираж менший за 100 або більший за 100 000 примірників', '`RequestedSize`, `MinSize`, `MaxSize`'],
    ['`InvalidStageTransitionException`', 'Недопустимий перехід між етапами', '`From`, `To`'],
    ['`EntityNotFoundException`', 'Запис із вказаним `Id` відсутній', '`EntityName`, `EntityId`'],
    ['`DataStorageException`', 'Помилка читання/запису або пошкоджений JSON', '`FilePath`, `InnerException`'],
    ['`PublishingException`', 'Порушення інших бізнес-правил (зайнятий email, видалення пов’язаних даних тощо)', '—'],
  ], [0.36, 0.38, 0.26]),
  H2('Етапи підготовки книги до друку'),
  P('Переходи між етапами контролює `PublishingService` (рисунок 2.6). Звичайні переходи «на наступний етап» описано статичним словником `Dictionary<ManuscriptStage, ManuscriptStage>`. Рішення після рецензії ухвалює окремий метод `Review`: при відмові рукопис переходить у стан `Rejected` і кидається `ManuscriptRejectedException`; схвалити рукопис можна лише після призначення редактора. Тираж створюється тільки для рукопису на етапі «Готовий до друку» або вже надрукованого (додатковий тираж).'),
  ...Figure('uml/5_stages.png', 'Діаграма станів рукопису', { maxH: 640 }),
  P('Крім етапів, сервіс забезпечує цілісність даних: email автора чи редактора має бути унікальним без урахування регістру; не можна видалити автора, у якого є рукописи, редактора, закріпленого за рукописом, і рукопис, за яким уже надруковано тираж. Під час імпорту з файлу перевіряється, що всі посилання між сутностями ведуть на існуючі записи.'),
  H2('Калькуляція собівартості та прибутку'),
  P('Розрахунок виконує клас `CostCalculator`, результат повертається як незмінний об’єкт `PrintRunEstimate`. Формули:'),
  Bullet('постійні витрати = ставка редактора × сторінки + 15 грн × сторінки (верстка) + 5 000 грн (дизайн обкладинки);'),
  Bullet('змінні витрати = тираж × (сторінки × (ціна паперу + 0,20 грн друк) + ціна обкладинки);'),
  Bullet('собівартість примірника = (постійні + змінні) / тираж;'),
  Bullet('прогнозована виручка = тираж × відсоток продажу × роздрібна ціна;'),
  Bullet('роялті = виручка × відсоток роялті автора;'),
  Bullet('прогнозований прибуток = виручка − собівартість − роялті.'),
  ...TableBlock('Тарифи калькуляції', ['Позиція', 'Значення'], [
    ['Папір газетний / офсетний / крейдований', '0,15 / 0,30 / 0,60 грн за сторінку'],
    ['Друк', '0,20 грн за сторінку'],
    ['Обкладинка м’яка / тверда', '12 / 45 грн за примірник'],
    ['Верстка', '15 грн за сторінку'],
    ['Дизайн обкладинки', '5 000 грн за тираж'],
  ], [0.55, 0.45]),
  P('**Приклад.** Рукопис 180 сторінок, редактор 10 грн/стор., тираж 2 000 примірників, офсетний папір, тверда обкладинка, ціна 450 грн, прогноз продажу 70 %, роялті 12,5 %. Постійні витрати: 1 800 + 2 700 + 5 000 = 9 500 грн. Змінні: 2 000 × (180 × 0,50 + 45) = 270 000 грн. Виручка: 1 400 × 450 = 630 000 грн, роялті 78 750 грн. Прибуток: 630 000 − 279 500 − 78 750 = 271 750 грн. Цей самий результат показує програма (рисунок 6.13).'),
];

// ---------- 3. Інтерфейсна частина ----------
const section3 = [
  H1('Опис інтерфейсної частини проєкту'),
  H2('Структура консольного меню'),
  P('Інтерфейс побудовано на абстрактному класі `MenuBase` (рисунок 3.1, альбомна сторінка). Він реалізує спільний цикл: виводить заголовок і пункти, зчитує вибір, виконує дію та перехоплює помилки. Нащадки лише перевизначають абстрактну властивість `Title` і метод `CreateItems()`, що повертає список пунктів `MenuItem` (текст + делегат `Action`). Це шаблонний метод: алгоритм роботи меню однаковий, змінюється лише набір пунктів.'),
  ...Figure('uml/4_ui.png', 'Діаграма класів інтерфейсної частини', { landscape: true }),
  ...TableBlock('Пункти меню', ['Меню', 'Пункти'], [
    ['Головне', 'Автори, Редактори, Рукописи, Тиражі, Звіти, Зберегти дані, Зберегти і вийти'],
    ['Автори', 'Список (за прізвищем), додати, редагувати, видалити, пошук за прізвищем'],
    ['Редактори', 'Список, додати, редагувати, видалити, фільтр за спеціалізацією'],
    ['Рукописи', 'Список з вибором сортування, подати, деталі, призначити редактора, наступний етап, рецензувати, редагувати, видалити, фільтри за етапом/жанром/автором, пошук за назвою'],
    ['Тиражі', 'Список, створити тираж (з виведенням калькуляції), калькуляція тиражу'],
    ['Звіти', 'Рейтинг тиражів за прибутком, загальний прогнозований прибуток, кількість рукописів за етапами'],
  ], [0.2, 0.8]),
  P('Значення переліків виводяться українською через клас `DisplayNames`, грошові суми форматуються з урахуванням культури `uk-UA`. Повідомлення виводяться кольором: успіх — зеленим, помилка — червоним, попередження — жовтим, заголовки меню — блакитним (`ConsoleHelper`).'),
  H2('Алгоритми валідації вводу'),
  P('Увесь ввід проходить через статичний клас `InputHelper`. Кожен метод повторює запит, доки користувач не введе коректне значення:'),
  Bullet('`ReadInt(prompt, min, max)` — ціле число в діапазоні (номери пунктів меню, кількість примірників);'),
  Bullet('`ReadEnum<T>()` — вибір значення переліку з нумерованого списку;'),
  Bullet('`Confirm()` — відповідь «т/так/y» або «н/ні/n»;'),
  Bullet('`Choose<T>()` — вибір сутності зі списку з можливістю скасування (0);'),
  Bullet('`ReadValid(prompt, apply)` — ключовий метод: передає введений рядок делегату, який присвоює значення властивості моделі. Якщо модель відхиляє значення (`ArgumentException`) або сервіс повідомляє про порушення правила (`PublishingException`, наприклад зайнятий email), користувач бачить причину і вводить значення повторно.'),
  P('Завдяки `ReadValid` правила валідації описано в одному місці — у моделях — і інтерфейс не дублює їх. Під час редагування використовується `ReadValidOptional`: порожній ввід залишає поточне значення. Дробові числа приймаються і з комою, і з крапкою.'),
  H2('Обробка помилок'),
  P('Метод `MenuBase.Execute` виконує дію пункту меню в блоці `try` і перехоплює виключення від найконкретнішого до загального:'),
  Numbered('`ManuscriptRejectedException` — виводиться як попередження, бо відхилення рукопису є очікуваним результатом рецензії;', 'catch'),
  Numbered('`InvalidStageTransitionException` — повідомлення формується з українськими назвами етапів;', 'catch'),
  Numbered('`PublishingException` — інші порушення бізнес-правил;', 'catch'),
  Numbered('`ArgumentException`, `InvalidOperationException` — некоректні аргументи.', 'catch'),
  P('Помилка в одній операції не завершує програму — користувач повертається до меню. На верхньому рівні `Program.Main` перехоплює `EndOfInputException` (закрито потік вводу, Ctrl+D) та будь-яку непередбачену помилку і в обох випадках зберігає дані перед завершенням.'),
];

// ---------- 4. Інші компоненти ----------
const section4 = [
  H1('Опис інших компонентів проєкту'),
  H2('Серіалізація та десеріалізація даних'),
  P('Збереження реалізовано узагальненим класом `JsonFileService<T>` з бібліотеки `System.Text.Json`. Усі дані видавництва збираються в об’єкт `PublishingData` з чотирма списками `List<T>` і серіалізуються в один файл `publishing_data.json`. Налаштування серіалізатора:'),
  Bullet('`WriteIndented = true` — файл зручно читати;'),
  Bullet('`JsonStringEnumConverter` — переліки записуються назвами («Poetry»), а не числами;'),
  Bullet('`JavaScriptEncoder.Create(UnicodeRanges.All)` — кирилиця зберігається як є, без `\\uXXXX`;'),
  Bullet('обчислювані властивості (`FullName`, `Role`, `CanBeReviewed`) позначено атрибутом `[JsonIgnore]`.'),
  P('Запис атомарний: дані спочатку пишуться у тимчасовий файл `.tmp`, який потім замінює основний, тому збій під час запису не пошкодить попередню копію. Помилки вводу-виводу, синтаксису JSON і валідації моделей під час десеріалізації загортаються у `DataStorageException`.'),
  P('Порядок роботи з файлом:'),
  Numbered('при старті `Program` перевіряє наявність файлу; якщо його немає — робота починається з порожньої бази;', 'load'),
  Numbered('дані десеріалізуються, `PublishingService.ImportData` перевіряє цілісність посилань і заповнює репозиторії;', 'load'),
  Numbered('якщо файл пошкоджений — виводиться помилка, створюється резервна копія `publishing_data.json.<дата>.bak`, і програма працює з порожньою базою, не перезаписуючи дані мовчки;', 'load'),
  Numbered('при виході (пункт «Зберегти і вийти» або закриття вводу) дані зберігаються; також їх можна зберегти вручну пунктом «Зберегти дані».', 'load'),
  H2('Модульні тести'),
  P('Проєкт `PublishingHouse.Tests` (MSTest) містить 70 тестів за схемою Arrange – Act – Assert. Покриття рядків бібліотеки `PublishingHouse.Core` становить близько 96 %. Розподіл тестів наведено в таблиці 4.1.'),
  ...TableBlock('Модульні тести', ['Клас тестів', 'Що перевіряється'], [
    ['`ModelValidationTests`', 'Валідація властивостей моделей, сортування рукописів за назвою'],
    ['`ExceptionTests`', 'Повідомлення та властивості власних виключень, спільний базовий клас'],
    ['`RepositoryTests`', 'Додавання, пошук, видалення, дублікати, атомарність `ReplaceAll`'],
    ['`ComparerTests`', 'Порядок сортування всіх чотирьох компараторів'],
    ['`CostCalculatorTests`', 'Формули калькуляції на розрахованих вручну прикладах'],
    ['`PublishingServiceTests`, `AdditionalServiceTests`', 'Повний цикл етапів, відхилення, межі тиражу, унікальність email, заборона видалення пов’язаних даних, імпорт/експорт'],
    ['`JsonFileServiceTests`', 'Збереження та відновлення всіх сутностей, порожній і пошкоджений файл, відсутність обчислюваних полів у JSON'],
  ], [0.4, 0.6]),
  P('Окремо виконано ручну перевірку за сценарієм із 16 кроків (файл `docs/MANUAL_TESTING.md`). Під час неї знайдено й виправлено дефект: видалення редактора, закріпленого за надрукованим рукописом, залишало у файлі посилання на неіснуючий запис, через що дані не завантажувалися при наступному запуску.'),
];

// ---------- 5. Лістинги ----------
const section5 = [
  H1('Лістинг важливих фрагментів коду'),
  P('Нижче наведено ключові фрагменти коду. Повний вихідний код додано окремо (каталог `coursework_2026`).'),
  H2('Моделі'),
  ...Listing('Абстрактний базовий клас сутностей', 'Models/EntityBase.cs', extractFile('PublishingHouse.Core/Models/EntityBase.cs')),
  ...Listing('Валідація email у класі Person', 'Models/Person.cs', extractBlock('PublishingHouse.Core/Models/Person.cs', 'public string Email')),
  ...Listing('Властивість PageCount і порівняння рукописів', 'Models/Manuscript.cs',
    extractBlock('PublishingHouse.Core/Models/Manuscript.cs', 'public int PageCount') + '\n\n' +
    extractBlock('PublishingHouse.Core/Models/Manuscript.cs', 'public int CompareTo')),
  H2('Інтерфейси та репозиторій'),
  ...Listing('Узагальнений інтерфейс репозиторію', 'Interfaces/IRepository.cs', extractFile('PublishingHouse.Core/Interfaces/IRepository.cs')),
  ...Listing('Узагальнений репозиторій', 'Repositories/Repository.cs', extractFile('PublishingHouse.Core/Repositories/Repository.cs')),
  ...Listing('Компаратор тиражів за прибутком', 'Comparers/PrintRunByProfitComparer.cs', extractFile('PublishingHouse.Core/Comparers/PrintRunByProfitComparer.cs')),
  H2('Виключення'),
  ...Listing('Виключення недопустимого розміру тиражу', 'Exceptions/InvalidPrintRunSizeException.cs', extractFile('PublishingHouse.Core/Exceptions/InvalidPrintRunSizeException.cs')),
  H2('Бізнес-логіка'),
  ...Listing('Переходи між етапами', 'Services/PublishingService.cs',
    extractBlock('PublishingHouse.Core/Services/PublishingService.cs', 'private static readonly Dictionary<ManuscriptStage') + '\n\n' +
    extractBlock('PublishingHouse.Core/Services/PublishingService.cs', 'public ManuscriptStage AdvanceStage')),
  ...Listing('Рецензування рукопису', 'Services/PublishingService.cs', extractBlock('PublishingHouse.Core/Services/PublishingService.cs', 'public void Review(')),
  ...Listing('Створення тиражу', 'Services/PublishingService.cs', extractBlock('PublishingHouse.Core/Services/PublishingService.cs', 'public PrintRun CreatePrintRun(')),
  ...Listing('Калькуляція тиражу', 'Services/CostCalculator.cs', extractBlock('PublishingHouse.Core/Services/CostCalculator.cs', 'public PrintRunEstimate Estimate(')),
  H2('Серіалізація'),
  ...Listing('Збереження та завантаження JSON', 'Services/JsonFileService.cs', extractFile('PublishingHouse.Core/Services/JsonFileService.cs')),
  H2('Інтерфейс користувача'),
  ...Listing('Базове меню з обробкою виключень', 'Menus/MenuBase.cs',
    extractBlock('PublishingHouse.UI/Menus/MenuBase.cs', 'public void Run()') + '\n\n' +
    extractBlock('PublishingHouse.UI/Menus/MenuBase.cs', 'private static void Execute(')),
  ...Listing('Ввід із повторним запитом', 'InputHelper.cs', extractBlock('PublishingHouse.UI/InputHelper.cs', 'public static void ReadValid(')),
  ...Listing('Додавання автора з валідацією кожного поля', 'Menus/AuthorMenu.cs', extractBlock('PublishingHouse.UI/Menus/AuthorMenu.cs', 'private void Add()')),
  ...Listing('Завантаження даних при старті', 'Program.cs', extractBlock('PublishingHouse.UI/Program.cs', 'private static void LoadData(')),
];

// ---------- 6. Скріншоти ----------
const shots = [
  ['r01_author_validation', 'Валідація імені, email і роялті під час додавання автора'],
  ['r02_authors_list', 'Список авторів, відсортований за прізвищем (PersonByNameComparer)'],
  ['r03_editor_email_rate', 'Зайнятий email і некоректна ставка під час додавання редактора'],
  ['r04_manuscript_submit', 'Подання рукопису: порожня назва та кількість сторінок поза межами'],
  ['r05_assign_editor', 'Призначення редактора: першими показано редакторів відповідної спеціалізації'],
  ['r06_invalid_transition', 'InvalidStageTransitionException: спроба оминути рецензію'],
  ['r07_review_approved', 'Схвалення рецензії, рукопис переходить на редагування'],
  ['r08_details', 'Детальна інформація про рукопис, готовий до друку'],
  ['r09_review_without_editor', 'Спроба схвалити рукопис без призначеного редактора'],
  ['r10_rejected', 'Відхилення рукопису: ManuscriptRejectedException'],
  ['r11_no_next_stage', 'Відхилений рукопис не має наступного етапу'],
  ['r12_invalid_size', 'Недопустимий розмір тиражу: InvalidPrintRunSizeException'],
  ['r13_print_run_estimate', 'Створення тиражу та калькуляція собівартості і прибутку'],
  ['r14_loss', 'Збитковий додатковий тираж'],
  ['r15_invalid_menu_choice', 'Некоректний вибір пункту меню'],
  ['r16_profit_ranking', 'Рейтинг тиражів за прибутком (PrintRunByProfitComparer) і загальний прибуток'],
  ['r17_stage_stats', 'Кількість рукописів за етапами'],
  ['r18_sort_by_pages', 'Сортування рукописів за кількістю сторінок (ManuscriptByPageCountComparer)'],
  ['r19_search', 'Пошук рукопису за частиною назви'],
  ['r20_delete_author', 'Заборона видалення автора, у якого є рукописи'],
  ['r21_delete_editor', 'Заборона видалення редактора, закріпленого за рукописом'],
  ['r22_restart', 'Повторний запуск: дані завантажено з JSON-файлу'],
  ['r23_corrupted', 'Пошкоджений файл даних: резервна копія та робота з порожньою базою'],
];
const section6 = [
  H1('Скріншоти результатів введення/виведення'),
  P('Скріншоти отримано під час послідовного запуску застосування, починаючи з порожньої бази даних; кожен показує одну операцію разом із меню, з якого її викликано. Введені користувачем значення виділено жирним шрифтом і підкресленням, повідомлення про успіх виводяться зеленим, про помилку — червоним, попередження — жовтим.'),
  ...shots.flatMap(([file, caption]) => Figure(`screenshots/report/${file}.png`, caption, { maxW: 600, maxH: 520, dpr: 2 })),
];

// ---------- Висновки ----------
const conclusions = [
  H1('Висновки', { numbered: false }),
  P('У курсовій роботі спроєктовано та реалізовано консольне застосування «Книжкове видавництво» мовою C# (.NET 10). Застосування дає змогу вести облік авторів, редакторів і рукописів, проводити рукопис через етапи рецензування, редагування, коректури та верстки, створювати тиражі й розраховувати їх собівартість і прогнозований прибуток.'),
  P('Під час роботи застосовано основні принципи ООП: інкапсуляцію (приватні поля з валідацією у властивостях), наслідування (`EntityBase` → `Person` → `Author`/`Editor`, ієрархії меню та виключень), поліморфізм (`GetSummary`, `Role`, `CreateItems`) та абстракцію (абстрактні класи й інтерфейси `IRepository<T>`, `ICostCalculator`, `IReviewable`). Використано узагальнені колекції, стандартні інтерфейси `IComparable<T>` і `IComparer<T>`, делегати `Predicate<T>`, `Func<T, TResult>`, `Action`.'),
  P('Бізнес-логіку винесено в окрему DLL-бібліотеку, яку підключає консольний проєкт, що відокремлює правила предметної області від інтерфейсу. Реалізовано власні виключення, зокрема `ManuscriptRejectedException` та `InvalidPrintRunSizeException` згідно з варіантом, контроль коректності всіх введених даних і збереження даних між запусками через JSON-серіалізацію з захистом від пошкодження файлу. Коректність бізнес-логіки підтверджено 70 модульними тестами та ручною перевіркою.'),
  P('Таким чином, виконано всі вимоги завдання, включно з позначеними «*», «**» та «***».'),
];

const sources = [
  H1('Список використаних джерел', { numbered: false }),
  ...[
    'Завдання для курсових робіт з дисципліни «Об’єктно-орієнтоване програмування». Кафедра ІПЗ, 2024.',
    'Покрокове керівництво з виконання курсової роботи з ООП. Кафедра ІПЗ.',
    'C# documentation [Електронний ресурс]. Microsoft Learn. URL: https://learn.microsoft.com/dotnet/csharp/',
    'JSON serialization and deserialization in .NET (System.Text.Json) [Електронний ресурс]. Microsoft Learn. URL: https://learn.microsoft.com/dotnet/standard/serialization/system-text-json/overview',
    'Generic collections in .NET [Електронний ресурс]. Microsoft Learn. URL: https://learn.microsoft.com/dotnet/standard/generics/collections',
    'Unit testing C# with MSTest and .NET [Електронний ресурс]. Microsoft Learn. URL: https://learn.microsoft.com/dotnet/core/testing/unit-testing-csharp-with-mstest',
  ].map(t => Numbered(t, 'sources')),
];

// ---------- Документ ----------
const pageFooter = () => ({
  default: new Footer({ children: [new Paragraph({ alignment: AlignmentType.RIGHT, children: [new TextRun({ children: [PageNumber.CURRENT] })] })] }),
});

/** Розбиває вміст на книжкові секції та окремі альбомні сторінки для широких рисунків. */
function buildSections(content) {
  const sections = [];
  let portrait = [];
  const flush = first => {
    if (!portrait.length) return;
    sections.push({
      properties: { page: { size: PAGE, margin: MARGIN, ...(first ? { pageNumbers: { start: 2 } } : {}) } },
      footers: pageFooter(),
      children: portrait,
    });
    portrait = [];
  };
  for (const item of content) {
    if (item && item.landscape) {
      flush(sections.length === 0);
      sections.push({
        properties: { page: { size: { ...PAGE, orientation: PageOrientation.LANDSCAPE }, margin: MARGIN } },
        footers: pageFooter(),
        children: item.landscape,
      });
    } else {
      portrait.push(item);
    }
  }
  flush(sections.length === 0);
  return sections;
}
const numberedConfig = ref => ({
  reference: ref,
  levels: [{ level: 0, format: LevelFormat.DECIMAL, text: '%1.', alignment: AlignmentType.LEFT, style: { paragraph: { indent: { left: 1069, hanging: 360 } } } }],
});

const doc = new Document({
  creator: STUDENT,
  title: 'Курсова робота: Книжкове видавництво',
  features: { updateFields: true },
  styles: {
    default: {
      document: { run: { font: FONT, size: 28 }, paragraph: { spacing: { line: 360, after: 0 } } },
    },
    paragraphStyles: [
      { id: 'Heading1', name: 'Heading 1', basedOn: 'Normal', next: 'Normal', quickFormat: true,
        run: { font: FONT, size: 28, bold: true }, paragraph: { spacing: { before: 0, after: 360 }, outlineLevel: 0, keepNext: true } },
      { id: 'Heading2', name: 'Heading 2', basedOn: 'Normal', next: 'Normal', quickFormat: true,
        run: { font: FONT, size: 28, bold: true }, paragraph: { spacing: { before: 240, after: 120 }, outlineLevel: 1, keepNext: true } },
    ],
  },
  numbering: {
    config: [
      { reference: 'dash', levels: [{ level: 0, format: LevelFormat.BULLET, text: '–', alignment: AlignmentType.LEFT, style: { paragraph: { indent: { left: 1069, hanging: 360 } } } }] },
      numberedConfig('catch'), numberedConfig('load'), numberedConfig('sources'),
    ],
  },
  sections: [
    { // титульний лист — без номера сторінки
      properties: { page: { size: PAGE, margin: MARGIN }, titlePage: true },
      children: titlePage,
    },
    ...buildSections([
      ...tocPage, ...intro, ...section1, ...section2, ...section3, ...section4, ...section5, ...section6, ...conclusions, ...sources,
    ]),
  ],
});

Packer.toBuffer(doc).then(buf => {
  fs.writeFileSync(OUT, buf);
  console.log('Записано', OUT, Math.round(buf.length / 1024), 'КБ');
});
