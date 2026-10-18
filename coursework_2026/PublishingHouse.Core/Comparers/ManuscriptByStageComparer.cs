using PublishingHouse.Core.Models;

namespace PublishingHouse.Core.Comparers
{
    /// <summary>
    /// Порівнює рукописи за етапом підготовки, а в межах етапу — за датою подання.
    /// </summary>
    public class ManuscriptByStageComparer : IComparer<Manuscript>
    {
        public int Compare(Manuscript? x, Manuscript? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;

            int result = x.Stage.CompareTo(y.Stage);
            return result != 0 ? result : x.SubmittedOn.CompareTo(y.SubmittedOn);
        }
    }
}
