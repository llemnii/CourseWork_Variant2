using Domain.Core.Common;

namespace Domain.Core.Interfaces;

public interface IRepository<T> where T : class, IEntity
{
    void Add(T item);
    bool Remove(Guid id);
    T? GetById(Guid id);
    IReadOnlyList<T> GetAll();
    IReadOnlyList<T> Find(Func<T, bool> predicate);
    void Replace(IEnumerable<T> items);
}
