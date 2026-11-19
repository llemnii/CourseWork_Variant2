namespace Domain.Core.Interfaces;

public interface IFileService<T>
{
    void Save(IEnumerable<T> data);
    List<T> Load();
}
