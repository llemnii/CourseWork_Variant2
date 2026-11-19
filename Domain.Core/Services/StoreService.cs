using Domain.Core.Exceptions;
using Domain.Core.Comparers;
using Domain.Core.Models;
using Domain.Core.Repositories;

namespace Domain.Core.Services;

public class StoreService
{
    private readonly string _dataDirectory;

    public InMemoryRepository<Product> Products { get; } = new();
    public InMemoryRepository<Category> Categories { get; } = new();
    public InMemoryRepository<Customer> Customers { get; } = new();
    public InMemoryRepository<Order> Orders { get; } = new();

    public StoreService(string dataDirectory)
    {
        _dataDirectory = dataDirectory;
    }

    public void LoadData()
    {
        Products.Replace(new JsonFileService<Product>(Path.Combine(_dataDirectory, "products.json")).Load());
        Categories.Replace(new JsonFileService<Category>(Path.Combine(_dataDirectory, "categories.json")).Load());
        Customers.Replace(new JsonFileService<Customer>(Path.Combine(_dataDirectory, "customers.json")).Load());
        Orders.Replace(new JsonFileService<Order>(Path.Combine(_dataDirectory, "orders.json")).Load());
    }

    public void SaveData()
    {
        new JsonFileService<Product>(Path.Combine(_dataDirectory, "products.json")).Save(Products.GetAll());
        new JsonFileService<Category>(Path.Combine(_dataDirectory, "categories.json")).Save(Categories.GetAll());
        new JsonFileService<Customer>(Path.Combine(_dataDirectory, "customers.json")).Save(Customers.GetAll());
        new JsonFileService<Order>(Path.Combine(_dataDirectory, "orders.json")).Save(Orders.GetAll());
    }

    public void SeedData()
    {
        if (Categories.GetAll().Count == 0)
        {
            Categories.Add(new Category("Смартфони"));
            Categories.Add(new Category("Ноутбуки"));
            Categories.Add(new Category("Аксесуари"));
        }

        if (Products.GetAll().Count == 0)
        {
            var categories = Categories.GetAll();
            Products.Add(new Product("Galaxy A55", "Samsung", categories[0].Id, 15999, 5));
            Products.Add(new Product("iPhone 15", "Apple", categories[0].Id, 32999, 3));
            Products.Add(new Product("IdeaPad 5", "Lenovo", categories[1].Id, 27999, 4));
            Products.Add(new Product("Wireless Mouse", "Logitech", categories[2].Id, 899, 12));
        }

        if (Customers.GetAll().Count == 0)
        {
            Customers.Add(new Customer("Олександр Іваненко", "ivanenko@example.com"));
            Customers.Add(new Customer("Марія Петренко", "petrenko@example.com"));
        }
    }

    public IReadOnlyList<Product> FindProductsByBrand(string? brand)
    {
        var products = string.IsNullOrWhiteSpace(brand)
            ? Products.GetAll().ToList()
            : Products.Find(product =>
                    product.Brand.Contains(brand.Trim(), StringComparison.OrdinalIgnoreCase))
                .ToList();

        products.Sort(new ProductNameComparer());
        return products;
    }

    public Order CreateOrder(Guid customerId)
    {
        if (Customers.GetById(customerId) is null)
            throw new ArgumentException("Клієнта не знайдено.");

        var order = new Order { CustomerId = customerId };
        Orders.Add(order);
        return order;
    }

    public void AddProductToOrder(Guid orderId, Guid productId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Кількість повинна бути більшою за нуль.");

        var order = Orders.GetById(orderId) ?? throw new ArgumentException("Замовлення не знайдено.");
        var product = Products.GetById(productId) ?? throw new ArgumentException("Товар не знайдено.");

        if (product.StockQuantity < quantity)
            throw new OutOfStockException(product.Name);

        product.StockQuantity -= quantity;
        order.AddItem(product, quantity);
    }

    public void ApplyCoupon(Guid orderId, string couponCode)
    {
        var order = Orders.GetById(orderId) ?? throw new ArgumentException("Замовлення не знайдено.");
        if (string.IsNullOrWhiteSpace(couponCode))
            throw new InvalidDiscountException("Купон не може бути порожнім.");

        var discounts = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
        {
            ["WELCOME10"] = 10,
            ["TECH15"] = 15
        };

        if (!discounts.TryGetValue(couponCode.Trim(), out var discount))
            throw new InvalidDiscountException("Такого купона не існує.");

        order.DiscountPercent = discount;
    }

    public decimal CalculateOrderTotal(Guid orderId)
    {
        var order = Orders.GetById(orderId) ?? throw new ArgumentException("Замовлення не знайдено.");
        var discount = order.GetSubtotal() * order.DiscountPercent / 100;
        return order.GetSubtotal() - discount;
    }
}
