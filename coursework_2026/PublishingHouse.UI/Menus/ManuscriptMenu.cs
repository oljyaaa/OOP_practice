using PublishingHouse.Core.Comparers;
using PublishingHouse.Core.Models;
using PublishingHouse.Core.Models.Enums;
using PublishingHouse.Core.Services;

namespace PublishingHouse.UI.Menus
{
    internal sealed class ManuscriptMenu : MenuBase
    {
        public ManuscriptMenu(PublishingService service)
            : base(service)
        {
        }

        protected override string Title => "Рукописи";

        protected override IReadOnlyList<MenuItem> CreateItems() => new List<MenuItem>
        {
            new("Список рукописів (з сортуванням)", ShowAll),
            new("Подати рукопис", Submit),
            new("Детальна інформація", ShowDetails),
            new("Призначити редактора", AssignEditor),
            new("Перевести на наступний етап", Advance),
            new("Рецензувати рукопис", Review),
            new("Редагувати дані рукопису", Edit),
            new("Видалити рукопис", Remove),
            new("Фільтр за етапом", FilterByStage),
            new("Фільтр за жанром", FilterByGenre),
            new("Рукописи автора", FilterByAuthor),
            new("Пошук за назвою", SearchByTitle)
        };

        public static string Format(Manuscript m) =>
            $"«{m.Title}» — {DisplayNames.Of(m.Genre)}, {m.PageCount} стор., етап: {DisplayNames.Of(m.Stage)}";

        private List<Manuscript> GetAllSortedByTitle()
        {
            var list = Service.Manuscripts.GetAll().ToList();
            list.Sort(); // IComparable<Manuscript> — за назвою
            return list;
        }

        private Manuscript? ChooseManuscript(string title, Predicate<Manuscript>? filter = null)
        {
            List<Manuscript> list = GetAllSortedByTitle();
            if (filter is not null)
                list = list.FindAll(filter);
            return InputHelper.Choose(list, Format, title);
        }

        private void Print(List<Manuscript> list, string emptyMessage)
        {
            if (list.Count == 0)
            {
                ConsoleHelper.Warning(emptyMessage);
                return;
            }
            foreach (Manuscript m in list)
                Console.WriteLine($"  • {Format(m)}, автор: {AuthorName(m)}");
        }

        private string AuthorName(Manuscript m) =>
            Service.Authors.TryGetById(m.AuthorId, out Author? a) && a is not null ? a.FullName : "—";

        private string EditorName(Manuscript m) =>
            m.EditorId is Guid id && Service.Editors.TryGetById(id, out Editor? e) && e is not null
                ? e.FullName
                : "не призначено";

        private void ShowAll()
        {
            Console.WriteLine("Сортувати за: 1 — назвою, 2 — кількістю сторінок, 3 — етапом");
            int sort = InputHelper.ReadInt("Ваш вибір: ", 1, 3);

            var list = Service.Manuscripts.GetAll().ToList();
            switch (sort)
            {
                case 1: list.Sort(); break;
                case 2: list.Sort(new ManuscriptByPageCountComparer()); break;
                case 3: list.Sort(new ManuscriptByStageComparer()); break;
            }
            Print(list, "Рукописів ще немає.");
        }

        private void Submit()
        {
            var authors = Service.Authors.GetAll().ToList();
            authors.Sort(new PersonByNameComparer());
            Author? author = InputHelper.Choose(authors, AuthorMenu.Format, "Оберіть автора:");
            if (author is null) return;

            // Тимчасовий об'єкт використовується лише для перевірки введених значень правилами моделі
            var draft = new Manuscript();
            InputHelper.ReadValid("Назва: ", s => draft.Title = s);
            Genre genre = InputHelper.ReadEnum<Genre>("Жанр:");
            InputHelper.ReadValid($"Кількість сторінок (1–{Manuscript.MaxPageCount}): ",
                s => draft.PageCount = InputHelper.ParseInt(s));

            Manuscript created = Service.SubmitManuscript(author.Id, draft.Title, genre, draft.PageCount);
            ConsoleHelper.Success($"Рукопис «{created.Title}» подано. Етап: {DisplayNames.Of(created.Stage)}.");
        }

        private void ShowDetails()
        {
            Manuscript? m = ChooseManuscript("Оберіть рукопис:");
            if (m is null) return;

            Console.WriteLine($"  Назва:      {m.Title}");
            Console.WriteLine($"  Жанр:       {DisplayNames.Of(m.Genre)}");
            Console.WriteLine($"  Сторінок:   {m.PageCount}");
            Console.WriteLine($"  Автор:      {AuthorName(m)}");
            Console.WriteLine($"  Редактор:   {EditorName(m)}");
            Console.WriteLine($"  Етап:       {DisplayNames.Of(m.Stage)}");
            Console.WriteLine($"  Подано:     {m.SubmittedOn:dd.MM.yyyy}");
            Console.WriteLine($"  Рецензія:   {(m.ReviewComment.Length > 0 ? m.ReviewComment : "—")}");
        }

        private void AssignEditor()
        {
            Manuscript? m = ChooseManuscript("Оберіть рукопис:",
                x => x.Stage is not (ManuscriptStage.Printed or ManuscriptStage.Rejected));
            if (m is null) return;

            // Спершу показуємо редакторів відповідної спеціалізації
            var editors = Service.Editors.GetAll()
                .OrderByDescending(e => e.Specialization == m.Genre)
                .ThenBy(e => e.LastName)
                .ToList();
            Editor? editor = InputHelper.Choose(editors, EditorMenu.Format, "Оберіть редактора:");
            if (editor is null) return;

            if (editor.Specialization != m.Genre)
                ConsoleHelper.Warning("Спеціалізація редактора не збігається з жанром рукопису.");
            Service.AssignEditor(m.Id, editor.Id);
            ConsoleHelper.Success($"Редактора {editor.FullName} призначено.");
        }

        private void Advance()
        {
            Manuscript? m = ChooseManuscript("Оберіть рукопис:");
            if (m is null) return;

            ManuscriptStage next = Service.AdvanceStage(m.Id);
            ConsoleHelper.Success($"Рукопис «{m.Title}» переведено на етап «{DisplayNames.Of(next)}».");
        }

        private void Review()
        {
            Manuscript? m = ChooseManuscript("Рукописи на рецензії:", x => x.CanBeReviewed);
            if (m is null) return;

            bool approved = InputHelper.Confirm("Рукопис схвалено?");
            string comment = InputHelper.ReadNonEmpty("Коментар рецензента: ");

            Service.Review(m.Id, approved, comment);
            ConsoleHelper.Success($"Рукопис схвалено. Етап: {DisplayNames.Of(m.Stage)}.");
        }

        private void Edit()
        {
            Manuscript? m = ChooseManuscript("Оберіть рукопис:",
                x => x.Stage is not (ManuscriptStage.Printed or ManuscriptStage.Rejected));
            if (m is null) return;

            Console.WriteLine("Залиште поле порожнім, щоб не змінювати його.");
            InputHelper.ReadValidOptional("Назва", m.Title, s => m.Title = s);
            InputHelper.ReadValidOptional("Кількість сторінок", m.PageCount.ToString(),
                s => m.PageCount = InputHelper.ParseInt(s));
            if (InputHelper.Confirm("Змінити жанр?"))
                m.Genre = InputHelper.ReadEnum<Genre>("Жанр:");
            ConsoleHelper.Success("Рукопис оновлено.");
        }

        private void Remove()
        {
            Manuscript? m = ChooseManuscript("Оберіть рукопис для видалення:");
            if (m is null || !InputHelper.Confirm($"Видалити «{m.Title}»?")) return;

            Service.RemoveManuscript(m.Id);
            ConsoleHelper.Success("Рукопис видалено.");
        }

        private void FilterByStage()
        {
            ManuscriptStage stage = InputHelper.ReadEnum<ManuscriptStage>("Етап:");
            List<Manuscript> list = Service.Manuscripts.Find(m => m.Stage == stage);
            list.Sort();
            Print(list, "Рукописів на цьому етапі немає.");
        }

        private void FilterByGenre()
        {
            Genre genre = InputHelper.ReadEnum<Genre>("Жанр:");
            List<Manuscript> list = Service.Manuscripts.Find(m => m.Genre == genre);
            list.Sort();
            Print(list, "Рукописів цього жанру немає.");
        }

        private void FilterByAuthor()
        {
            var authors = Service.Authors.GetAll().ToList();
            authors.Sort(new PersonByNameComparer());
            Author? author = InputHelper.Choose(authors, AuthorMenu.Format, "Оберіть автора:");
            if (author is null) return;

            List<Manuscript> list = Service.Manuscripts.Find(m => m.AuthorId == author.Id);
            list.Sort();
            Print(list, "У цього автора немає рукописів.");
        }

        private void SearchByTitle()
        {
            string query = InputHelper.ReadNonEmpty("Частина назви: ");
            List<Manuscript> list = Service.Manuscripts.Find(m => m.Title.Contains(query, StringComparison.CurrentCultureIgnoreCase));
            list.Sort();
            Print(list, "Нічого не знайдено.");
        }
    }
}
