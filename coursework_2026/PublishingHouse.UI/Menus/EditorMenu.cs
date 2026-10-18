using PublishingHouse.Core.Comparers;
using PublishingHouse.Core.Models;
using PublishingHouse.Core.Models.Enums;
using PublishingHouse.Core.Services;

namespace PublishingHouse.UI.Menus
{
    internal sealed class EditorMenu : MenuBase
    {
        public EditorMenu(PublishingService service)
            : base(service)
        {
        }

        protected override string Title => "Редактори";

        protected override IReadOnlyList<MenuItem> CreateItems() => new List<MenuItem>
        {
            new("Список редакторів", ShowAll),
            new("Додати редактора", Add),
            new("Редагувати редактора", Edit),
            new("Видалити редактора", Remove),
            new("Фільтр за спеціалізацією", FilterBySpecialization)
        };

        public static string Format(Editor e) =>
            $"{e.FullName} <{e.Email}>, {DisplayNames.Of(e.Specialization)}, {ConsoleHelper.Money(e.RatePerPage)}/стор.";

        private List<Editor> GetSorted()
        {
            var editors = Service.Editors.GetAll().ToList();
            editors.Sort(new PersonByNameComparer());
            return editors;
        }

        private void ShowAll()
        {
            List<Editor> editors = GetSorted();
            if (editors.Count == 0)
            {
                ConsoleHelper.Warning("Редакторів ще немає.");
                return;
            }
            foreach (Editor e in editors)
            {
                int active = Service.Manuscripts.Find(m => m.EditorId == e.Id
                    && m.Stage != ManuscriptStage.Printed && m.Stage != ManuscriptStage.Rejected).Count;
                Console.WriteLine($"  • {Format(e)}, рукописів у роботі: {active}");
            }
        }

        private void Add()
        {
            var editor = new Editor();
            InputHelper.ReadValid("Ім'я: ", s => editor.FirstName = s);
            InputHelper.ReadValid("Прізвище: ", s => editor.LastName = s);
            InputHelper.ReadValid("Email: ", s => editor.Email = s);
            InputHelper.ReadValid("Ставка за сторінку, грн: ", s => editor.RatePerPage = InputHelper.ParseDecimal(s));
            editor.Specialization = InputHelper.ReadEnum<Genre>("Спеціалізація:");

            Service.AddEditor(editor);
            ConsoleHelper.Success($"Редактора {editor.FullName} додано.");
        }

        private void Edit()
        {
            Editor? editor = InputHelper.Choose(GetSorted(), Format, "Оберіть редактора:");
            if (editor is null) return;

            Console.WriteLine("Залиште поле порожнім, щоб не змінювати його.");
            InputHelper.ReadValidOptional("Ім'я", editor.FirstName, s => editor.FirstName = s);
            InputHelper.ReadValidOptional("Прізвище", editor.LastName, s => editor.LastName = s);
            InputHelper.ReadValidOptional("Email", editor.Email, s => Service.ChangeEmail(editor, s));
            InputHelper.ReadValidOptional("Ставка за сторінку, грн", editor.RatePerPage.ToString(),
                s => editor.RatePerPage = InputHelper.ParseDecimal(s));
            if (InputHelper.Confirm("Змінити спеціалізацію?"))
                editor.Specialization = InputHelper.ReadEnum<Genre>("Спеціалізація:");
            ConsoleHelper.Success("Дані редактора оновлено.");
        }

        private void Remove()
        {
            Editor? editor = InputHelper.Choose(GetSorted(), Format, "Оберіть редактора для видалення:");
            if (editor is null || !InputHelper.Confirm($"Видалити {editor.FullName}?")) return;

            Service.RemoveEditor(editor.Id);
            ConsoleHelper.Success("Редактора видалено.");
        }

        private void FilterBySpecialization()
        {
            Genre genre = InputHelper.ReadEnum<Genre>("Спеціалізація:");
            List<Editor> found = Service.Editors.Find(e => e.Specialization == genre);

            if (found.Count == 0)
                ConsoleHelper.Warning("Редакторів з такою спеціалізацією немає.");
            foreach (Editor e in found)
                Console.WriteLine($"  • {Format(e)}");
        }
    }
}
