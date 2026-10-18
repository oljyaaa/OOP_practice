using PublishingHouse.Core.Models;

namespace PublishingHouse.Core.Interfaces
{
    /// <summary>
    /// Узагальнене сховище сутностей у пам'яті.
    /// </summary>
    public interface IRepository<T> where T : EntityBase
    {
        int Count { get; }

        void Add(T item);

        /// <summary>
        /// Повертає сутність за ідентифікатором або кидає EntityNotFoundException.
        /// </summary>
        T GetById(Guid id);

        bool TryGetById(Guid id, out T? item);

        bool Remove(Guid id);

        IReadOnlyList<T> GetAll();

        List<T> Find(Predicate<T> match);

        /// <summary>
        /// Замінює весь вміст сховища (використовується при завантаженні з файлу).
        /// </summary>
        void ReplaceAll(IEnumerable<T> items);
    }
}
