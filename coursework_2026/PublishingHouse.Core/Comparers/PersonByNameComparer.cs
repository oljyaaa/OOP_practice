using PublishingHouse.Core.Models;

namespace PublishingHouse.Core.Comparers
{
    /// <summary>
    /// Порівнює осіб за прізвищем, потім за ім'ям.
    /// Завдяки контраваріантності IComparer&lt;in T&gt; підходить і для Author, і для Editor.
    /// </summary>
    public class PersonByNameComparer : IComparer<Person>
    {
        public int Compare(Person? x, Person? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;

            int result = string.Compare(x.LastName, y.LastName, StringComparison.CurrentCultureIgnoreCase);
            return result != 0
                ? result
                : string.Compare(x.FirstName, y.FirstName, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
