using PublishingHouse.Core.Models;

namespace PublishingHouse.Core.Comparers
{
    /// <summary>
    /// Порівнює тиражі за прогнозованим прибутком (за спаданням — найприбутковіші першими).
    /// Прибуток обчислює переданий делегат, бо для нього потрібні рукопис, автор і редактор.
    /// </summary>
    public class PrintRunByProfitComparer : IComparer<PrintRun>
    {
        private readonly Func<PrintRun, decimal> _profitSelector;

        public PrintRunByProfitComparer(Func<PrintRun, decimal> profitSelector)
        {
            _profitSelector = profitSelector ?? throw new ArgumentNullException(nameof(profitSelector));
        }

        public int Compare(PrintRun? x, PrintRun? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return 1;
            if (y is null) return -1;
            return _profitSelector(y).CompareTo(_profitSelector(x));
        }
    }
}
