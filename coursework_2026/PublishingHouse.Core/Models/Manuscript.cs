using PublishingHouse.Core.Models.Enums;

namespace PublishingHouse.Core.Models
{
    /// <summary>
    /// Рукопис книги, що проходить етапи підготовки до друку.
    /// </summary>
    public class Manuscript : EntityBase, IComparable<Manuscript>
    {
        public const int MaxTitleLength = 150;
        public const int MaxPageCount = 5000;

        private string _title = string.Empty;
        private int _pageCount;
        private Guid _authorId;
        private string _reviewComment = string.Empty;

        public string Title
        {
            get => _title;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Назва рукопису не може бути порожньою.");
                if (value.Trim().Length > MaxTitleLength)
                    throw new ArgumentException($"Назва не може бути довшою за {MaxTitleLength} символів.");
                _title = value.Trim();
            }
        }

        public Genre Genre { get; set; }

        /// <summary>
        /// Кількість сторінок (1–5000).
        /// </summary>
        public int PageCount
        {
            get => _pageCount;
            set
            {
                if (value <= 0 || value > MaxPageCount)
                    throw new ArgumentOutOfRangeException(nameof(PageCount),
                        $"Кількість сторінок має бути в межах 1–{MaxPageCount}.");
                _pageCount = value;
            }
        }

        /// <summary>
        /// Ідентифікатор автора рукопису.
        /// </summary>
        public Guid AuthorId
        {
            get => _authorId;
            set
            {
                if (value == Guid.Empty)
                    throw new ArgumentException("Рукопис повинен мати автора.");
                _authorId = value;
            }
        }

        /// <summary>
        /// Ідентифікатор призначеного редактора (null — ще не призначено).
        /// </summary>
        public Guid? EditorId { get; set; }

        /// <summary>
        /// Поточний етап підготовки. Переходи між етапами контролює сервіс.
        /// </summary>
        public ManuscriptStage Stage { get; set; } = ManuscriptStage.Submitted;

        public DateTime SubmittedOn { get; set; } = DateTime.Now;

        /// <summary>
        /// Висновок рецензента.
        /// </summary>
        public string ReviewComment
        {
            get => _reviewComment;
            set => _reviewComment = value?.Trim() ?? string.Empty;
        }

        public int CompareTo(Manuscript? other)
        {
            if (other is null) return 1;
            return string.Compare(Title, other.Title, StringComparison.CurrentCultureIgnoreCase);
        }

        public override string GetSummary() =>
            $"«{Title}» ({Genre}, {PageCount} стор.) — етап: {Stage}";
    }
}
