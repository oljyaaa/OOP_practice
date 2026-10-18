using PublishingHouse.Core.Models;

namespace PublishingHouse.Core.Comparers
{
    /// <summary>
    /// Порівнює рукописи за кількістю сторінок (за зростанням).
    /// </summary>
    public class ManuscriptByPageCountComparer : IComparer<Manuscript>
    {
        public int Compare(Manuscript? x, Manuscript? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;
            return x.PageCount.CompareTo(y.PageCount);
        }
    }
}
