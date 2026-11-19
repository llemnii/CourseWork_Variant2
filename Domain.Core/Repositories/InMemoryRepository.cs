using Domain.Core.Common;
using Domain.Core.Interfaces;

namespace Domain.Core.Repositories;

public class InMemoryRepository<T> : IRepository<T> where T : class, IEntity
{
    private readonly List<T> _items = new();

    public void Add(T item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (_items.Any(existing => existing.Id == item.Id))
            throw new InvalidOperationException("Об’єкт із таким ідентифікатором уже існує.");

        _items.Add(item);
    }

    public bool Remove(Guid id)
    {
        var item = GetById(id);
        return item is not null && _items.Remove(item);
    }

    public T? GetById(Guid id) => _items.FirstOrDefault(item => item.Id == id);

    public IReadOnlyList<T> GetAll() => _items.ToList();

    public IReadOnlyList<T> Find(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return _items.Where(predicate).ToList();
    }

    public void Replace(IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        _items.Clear();
        _items.AddRange(items);
    }
}
