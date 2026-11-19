using Domain.Core.Common;

namespace Domain.Core.Models;

public class Product : EntityBase, IComparable<Product>
{
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }

    public Product()
    {
    }

    public Product(string name, string brand, Guid categoryId, decimal price, int stockQuantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Назва товару не може бути порожньою.");
        if (string.IsNullOrWhiteSpace(brand))
            throw new ArgumentException("Бренд товару не може бути порожнім.");
        if (price <= 0)
            throw new ArgumentException("Ціна товару повинна бути більшою за нуль.");
        if (stockQuantity < 0)
            throw new ArgumentException("Кількість товару не може бути від’ємною.");

        Name = name.Trim();
        Brand = brand.Trim();
        CategoryId = categoryId;
        Price = price;
        StockQuantity = stockQuantity;
        Validate();
    }

    public override void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Назва товару не може бути порожньою.");
        if (string.IsNullOrWhiteSpace(Brand))
            throw new ArgumentException("Бренд товару не може бути порожнім.");
        if (Price <= 0)
            throw new ArgumentException("Ціна товару повинна бути більшою за нуль.");
        if (StockQuantity < 0)
            throw new ArgumentException("Кількість товару не може бути від’ємною.");
    }

    public int CompareTo(Product? other)
    {
        if (other is null)
            return 1;

        return string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
    }
}
