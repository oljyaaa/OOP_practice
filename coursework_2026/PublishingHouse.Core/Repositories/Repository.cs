using PublishingHouse.Core.Exceptions;
using PublishingHouse.Core.Interfaces;
using PublishingHouse.Core.Models;

namespace PublishingHouse.Core.Repositories
{
    /// <summary>
    /// Узагальнений репозиторій на основі List&lt;T&gt; з індексом Dictionary&lt;Guid, T&gt;
    /// для швидкого пошуку за ідентифікатором.
    /// </summary>
    public class Repository<T> : IRepository<T> where T : EntityBase
    {
        private readonly List<T> _items = new();
        private readonly Dictionary<Guid, T> _index = new();
        private readonly string _entityName;

        /// <param name="entityName">Назва сутності для повідомлень про помилки.</param>
        public Repository(string entityName)
        {
            if (string.IsNullOrWhiteSpace(entityName))
                throw new ArgumentException("Назва сутності не може бути порожньою.", nameof(entityName));
            _entityName = entityName;
        }

        public int Count => _items.Count;

        public void Add(T item)
        {
            ArgumentNullException.ThrowIfNull(item);
            if (_index.ContainsKey(item.Id))
                throw new InvalidOperationException($"{_entityName} з ідентифікатором {item.Id} вже існує.");

            _items.Add(item);
            _index.Add(item.Id, item);
        }

        public T GetById(Guid id)
        {
            if (!_index.TryGetValue(id, out T? item))
                throw new EntityNotFoundException(_entityName, id);
            return item;
        }

        public bool TryGetById(Guid id, out T? item) => _index.TryGetValue(id, out item);

        public bool Remove(Guid id)
        {
            if (!_index.Remove(id, out T? item))
                return false;
            _items.Remove(item);
            return true;
        }

        public IReadOnlyList<T> GetAll() => _items.AsReadOnly();

        public List<T> Find(Predicate<T> match)
        {
            ArgumentNullException.ThrowIfNull(match);
            return _items.FindAll(match);
        }

        public void ReplaceAll(IEnumerable<T> items)
        {
            ArgumentNullException.ThrowIfNull(items);

            // Спершу будуємо новий індекс, щоб у разі помилки не втратити поточні дані
            var newIndex = new Dictionary<Guid, T>();
            foreach (T item in items)
            {
                ArgumentNullException.ThrowIfNull(item);
                if (!newIndex.TryAdd(item.Id, item))
                    throw new InvalidOperationException($"{_entityName} з ідентифікатором {item.Id} зустрічається двічі.");
            }

            _items.Clear();
            _items.AddRange(newIndex.Values);
            _index.Clear();
            foreach (var pair in newIndex)
                _index.Add(pair.Key, pair.Value);
        }
    }
}
