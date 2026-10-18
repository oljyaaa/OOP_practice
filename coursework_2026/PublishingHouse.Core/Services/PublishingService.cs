using PublishingHouse.Core.Exceptions;
using PublishingHouse.Core.Interfaces;
using PublishingHouse.Core.Models;
using PublishingHouse.Core.Models.Enums;
using PublishingHouse.Core.Repositories;

namespace PublishingHouse.Core.Services
{
    /// <summary>
    /// Головний сервіс видавництва: облік авторів і редакторів,
    /// проходження рукописом етапів підготовки, створення та калькуляція тиражів.
    /// </summary>
    public class PublishingService
    {
        public const int MinPrintRunSize = 100;
        public const int MaxPrintRunSize = 100_000;

        /// <summary>
        /// Етапи, на які рукопис переходить звичайним «наступним кроком».
        /// Рішення після рецензії ухвалюється окремо методом Review.
        /// </summary>
        private static readonly Dictionary<ManuscriptStage, ManuscriptStage> NextStage = new()
        {
            [ManuscriptStage.Submitted] = ManuscriptStage.UnderReview,
            [ManuscriptStage.Editing] = ManuscriptStage.Proofreading,
            [ManuscriptStage.Proofreading] = ManuscriptStage.Layout,
            [ManuscriptStage.Layout] = ManuscriptStage.ReadyForPrint
        };

        private readonly IRepository<Author> _authors;
        private readonly IRepository<Editor> _editors;
        private readonly IRepository<Manuscript> _manuscripts;
        private readonly IRepository<PrintRun> _printRuns;
        private readonly ICostCalculator _calculator;

        public PublishingService()
            : this(new Repository<Author>("Автор"), new Repository<Editor>("Редактор"),
                   new Repository<Manuscript>("Рукопис"), new Repository<PrintRun>("Тираж"),
                   new CostCalculator())
        {
        }

        public PublishingService(IRepository<Author> authors, IRepository<Editor> editors,
            IRepository<Manuscript> manuscripts, IRepository<PrintRun> printRuns, ICostCalculator calculator)
        {
            _authors = authors ?? throw new ArgumentNullException(nameof(authors));
            _editors = editors ?? throw new ArgumentNullException(nameof(editors));
            _manuscripts = manuscripts ?? throw new ArgumentNullException(nameof(manuscripts));
            _printRuns = printRuns ?? throw new ArgumentNullException(nameof(printRuns));
            _calculator = calculator ?? throw new ArgumentNullException(nameof(calculator));
        }

        public IRepository<Author> Authors => _authors;

        public IRepository<Editor> Editors => _editors;

        public IRepository<Manuscript> Manuscripts => _manuscripts;

        public IRepository<PrintRun> PrintRuns => _printRuns;

        // ---------- Автори та редактори ----------

        public void AddAuthor(Author author)
        {
            EnsureEmailIsUnique(author.Email, author.Id);
            _authors.Add(author);
        }

        public void AddEditor(Editor editor)
        {
            EnsureEmailIsUnique(editor.Email, editor.Id);
            _editors.Add(editor);
        }

        /// <summary>
        /// Змінює email автора чи редактора з перевіркою унікальності.
        /// </summary>
        public void ChangeEmail(Person person, string newEmail)
        {
            ArgumentNullException.ThrowIfNull(person);
            EnsureEmailIsUnique(newEmail, person.Id);
            person.Email = newEmail;
        }

        public void RemoveAuthor(Guid authorId)
        {
            Author author = _authors.GetById(authorId);
            if (_manuscripts.Find(m => m.AuthorId == authorId).Count > 0)
                throw new PublishingException($"Неможливо видалити автора {author.FullName}: у нього є рукописи.");
            _authors.Remove(authorId);
        }

        public void RemoveEditor(Guid editorId)
        {
            Editor editor = _editors.GetById(editorId);
            bool hasActiveWork = _manuscripts.Find(m => m.EditorId == editorId
                && m.Stage != ManuscriptStage.Printed && m.Stage != ManuscriptStage.Rejected).Count > 0;
            if (hasActiveWork)
                throw new PublishingException($"Неможливо видалити редактора {editor.FullName}: він працює над рукописами.");
            _editors.Remove(editorId);
        }

        // ---------- Рукописи та етапи ----------

        public Manuscript SubmitManuscript(Guid authorId, string title, Genre genre, int pageCount)
        {
            _authors.GetById(authorId);

            var manuscript = new Manuscript
            {
                AuthorId = authorId,
                Title = title,
                Genre = genre,
                PageCount = pageCount,
                Stage = ManuscriptStage.Submitted,
                SubmittedOn = DateTime.Now
            };
            _manuscripts.Add(manuscript);
            return manuscript;
        }

        public void RemoveManuscript(Guid manuscriptId)
        {
            Manuscript manuscript = _manuscripts.GetById(manuscriptId);
            if (_printRuns.Find(r => r.ManuscriptId == manuscriptId).Count > 0)
                throw new PublishingException($"Неможливо видалити рукопис «{manuscript.Title}»: за ним вже є тиражі.");
            _manuscripts.Remove(manuscriptId);
        }

        public void AssignEditor(Guid manuscriptId, Guid editorId)
        {
            Manuscript manuscript = _manuscripts.GetById(manuscriptId);
            _editors.GetById(editorId);

            if (manuscript.Stage is ManuscriptStage.Printed or ManuscriptStage.Rejected)
                throw new PublishingException($"Рукопису на етапі {manuscript.Stage} не можна призначити редактора.");

            manuscript.EditorId = editorId;
        }

        /// <summary>
        /// Переводить рукопис на наступний етап підготовки.
        /// </summary>
        public ManuscriptStage AdvanceStage(Guid manuscriptId)
        {
            Manuscript manuscript = _manuscripts.GetById(manuscriptId);

            if (!NextStage.TryGetValue(manuscript.Stage, out ManuscriptStage next))
            {
                ManuscriptStage target = manuscript.Stage == ManuscriptStage.UnderReview
                    ? ManuscriptStage.Editing
                    : manuscript.Stage;
                throw new InvalidStageTransitionException(manuscript.Stage, target);
            }

            manuscript.Stage = next;
            return next;
        }

        /// <summary>
        /// Рецензування рукопису. У разі відмови рукопис переходить у стан Rejected
        /// і кидається ManuscriptRejectedException.
        /// </summary>
        public void Review(Guid manuscriptId, bool approved, string comment)
        {
            Manuscript manuscript = _manuscripts.GetById(manuscriptId);

            if (!manuscript.CanBeReviewed)
                throw new InvalidStageTransitionException(manuscript.Stage,
                    approved ? ManuscriptStage.Editing : ManuscriptStage.Rejected);
            if (string.IsNullOrWhiteSpace(comment))
                throw new ArgumentException("Рецензія повинна містити коментар.");
            if (approved && manuscript.EditorId is null)
                throw new PublishingException("Перед схваленням рукопису потрібно призначити редактора.");

            manuscript.ReviewComment = comment;

            if (!approved)
            {
                manuscript.Stage = ManuscriptStage.Rejected;
                throw new ManuscriptRejectedException(manuscript.Id, manuscript.Title, comment);
            }

            manuscript.Stage = ManuscriptStage.Editing;
        }

        // ---------- Тиражі та калькуляція ----------

        public PrintRun CreatePrintRun(Guid manuscriptId, int quantity, PaperType paperType,
            CoverType coverType, decimal retailPrice, decimal expectedSellThroughPercent)
        {
            Manuscript manuscript = _manuscripts.GetById(manuscriptId);

            if (manuscript.Stage is not (ManuscriptStage.ReadyForPrint or ManuscriptStage.Printed))
                throw new InvalidStageTransitionException(manuscript.Stage, ManuscriptStage.Printed);
            if (quantity < MinPrintRunSize || quantity > MaxPrintRunSize)
                throw new InvalidPrintRunSizeException(quantity, MinPrintRunSize, MaxPrintRunSize);

            var printRun = new PrintRun
            {
                ManuscriptId = manuscriptId,
                Quantity = quantity,
                PaperType = paperType,
                CoverType = coverType,
                RetailPrice = retailPrice,
                ExpectedSellThroughPercent = expectedSellThroughPercent,
                CreatedOn = DateTime.Now
            };
            _printRuns.Add(printRun);
            manuscript.Stage = ManuscriptStage.Printed;
            return printRun;
        }

        public PrintRunEstimate EstimatePrintRun(Guid printRunId) => Estimate(_printRuns.GetById(printRunId));

        public PrintRunEstimate Estimate(PrintRun printRun)
        {
            Manuscript manuscript = _manuscripts.GetById(printRun.ManuscriptId);
            Author author = _authors.GetById(manuscript.AuthorId);
            Editor? editor = null;
            if (manuscript.EditorId is Guid editorId)
                _editors.TryGetById(editorId, out editor);

            return _calculator.Estimate(printRun, manuscript, author, editor);
        }

        public decimal GetTotalExpectedProfit()
        {
            decimal total = 0m;
            foreach (PrintRun run in _printRuns.GetAll())
                total += Estimate(run).ExpectedProfit;
            return total;
        }

        // ---------- Збереження ----------

        public PublishingData ExportData() => new()
        {
            Authors = _authors.GetAll().ToList(),
            Editors = _editors.GetAll().ToList(),
            Manuscripts = _manuscripts.GetAll().ToList(),
            PrintRuns = _printRuns.GetAll().ToList()
        };

        /// <summary>
        /// Завантажує дані, попередньо перевіряючи цілісність зв'язків між сутностями.
        /// </summary>
        public void ImportData(PublishingData data)
        {
            ArgumentNullException.ThrowIfNull(data);

            var authorIds = data.Authors.Select(a => a.Id).ToHashSet();
            var editorIds = data.Editors.Select(e => e.Id).ToHashSet();
            var manuscriptIds = data.Manuscripts.Select(m => m.Id).ToHashSet();

            foreach (Manuscript m in data.Manuscripts)
            {
                if (!authorIds.Contains(m.AuthorId))
                    throw new PublishingException($"Пошкоджені дані: автора рукопису «{m.Title}» не знайдено.");
                if (m.EditorId is Guid id && !editorIds.Contains(id))
                    throw new PublishingException($"Пошкоджені дані: редактора рукопису «{m.Title}» не знайдено.");
            }
            foreach (PrintRun r in data.PrintRuns)
            {
                if (!manuscriptIds.Contains(r.ManuscriptId))
                    throw new PublishingException("Пошкоджені дані: тираж посилається на неіснуючий рукопис.");
            }

            _authors.ReplaceAll(data.Authors);
            _editors.ReplaceAll(data.Editors);
            _manuscripts.ReplaceAll(data.Manuscripts);
            _printRuns.ReplaceAll(data.PrintRuns);
        }

        private void EnsureEmailIsUnique(string email, Guid ownId)
        {
            bool taken = _authors.Find(a => a.Id != ownId && a.Email.Equals(email, StringComparison.OrdinalIgnoreCase)).Count > 0
                         || _editors.Find(e => e.Id != ownId && e.Email.Equals(email, StringComparison.OrdinalIgnoreCase)).Count > 0;
            if (taken)
                throw new PublishingException($"Email {email} вже використовується.");
        }
    }
}
