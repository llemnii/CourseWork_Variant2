using Domain.Core.Common;

namespace Domain.Core.Models;

public class Category : EntityBase
{
    public string Name { get; set; } = string.Empty;

    public Category()
    {
    }

    public Category(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Назва категорії не може бути порожньою.");

        Name = name.Trim();
    }

    public override void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Назва категорії не може бути порожньою.");
    }
}
