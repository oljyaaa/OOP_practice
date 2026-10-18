using PublishingHouse.Core.Comparers;
using PublishingHouse.Core.Models;
using PublishingHouse.Core.Services;

namespace PublishingHouse.UI.Menus
{
    internal sealed class AuthorMenu : MenuBase
    {
        public AuthorMenu(PublishingService service)
            : base(service)
        {
        }

        protected override string Title => "Автори";

        protected override IReadOnlyList<MenuItem> CreateItems() => new List<MenuItem>
        {
            new("Список авторів", ShowAll),
            new("Додати автора", Add),
            new("Редагувати автора", Edit),
            new("Видалити автора", Remove),
            new("Пошук за прізвищем", Search)
        };

        public static string Format(Author a) => $"{a.FullName} <{a.Email}>, роялті {a.RoyaltyPercent}%";

        private List<Author> GetSorted()
        {
            var authors = Service.Authors.GetAll().ToList();
            authors.Sort(new PersonByNameComparer());
            return authors;
        }

        private void ShowAll()
        {
            List<Author> authors = GetSorted();
            if (authors.Count == 0)
            {
                ConsoleHelper.Warning("Авторів ще немає.");
                return;
            }
            foreach (Author a in authors)
            {
                int count = Service.Manuscripts.Find(m => m.AuthorId == a.Id).Count;
                Console.WriteLine($"  • {Format(a)}, рукописів: {count}");
            }
        }

        private void Add()
        {
            var author = new Author();
            InputHelper.ReadValid("Ім'я: ", s => author.FirstName = s);
            InputHelper.ReadValid("Прізвище: ", s => author.LastName = s);
            InputHelper.ReadValid("Email: ", s => author.Email = s);
            InputHelper.ReadValid($"Роялті, % (0–{Author.MaxRoyaltyPercent}): ",
                s => author.RoyaltyPercent = InputHelper.ParseDecimal(s));

            Service.AddAuthor(author);
            ConsoleHelper.Success($"Автора {author.FullName} додано.");
        }

        private void Edit()
        {
            Author? author = InputHelper.Choose(GetSorted(), Format, "Оберіть автора:");
            if (author is null) return;

            Console.WriteLine("Залиште поле порожнім, щоб не змінювати його.");
            InputHelper.ReadValidOptional("Ім'я", author.FirstName, s => author.FirstName = s);
            InputHelper.ReadValidOptional("Прізвище", author.LastName, s => author.LastName = s);
            InputHelper.ReadValidOptional("Email", author.Email, s => Service.ChangeEmail(author, s));
            InputHelper.ReadValidOptional("Роялті, %", author.RoyaltyPercent.ToString(),
                s => author.RoyaltyPercent = InputHelper.ParseDecimal(s));
            ConsoleHelper.Success("Дані автора оновлено.");
        }

        private void Remove()
        {
            Author? author = InputHelper.Choose(GetSorted(), Format, "Оберіть автора для видалення:");
            if (author is null || !InputHelper.Confirm($"Видалити {author.FullName}?")) return;

            Service.RemoveAuthor(author.Id);
            ConsoleHelper.Success("Автора видалено.");
        }

        private void Search()
        {
            string query = InputHelper.ReadNonEmpty("Частина прізвища: ");
            List<Author> found = Service.Authors.Find(a => a.LastName.Contains(query, StringComparison.CurrentCultureIgnoreCase));
            found.Sort(new PersonByNameComparer());

            if (found.Count == 0)
                ConsoleHelper.Warning("Нічого не знайдено.");
            foreach (Author a in found)
                Console.WriteLine($"  • {Format(a)}");
        }
    }
}
