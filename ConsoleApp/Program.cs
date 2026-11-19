using Domain.Core.Exceptions;
using Domain.Core.Models;
using Domain.Core.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var dataDirectory = Path.Combine(AppContext.BaseDirectory, "data");
var store = new StoreService(dataDirectory);

try
{
    store.LoadData();
    store.SeedData();
}
catch (StorageException exception)
{
    Console.WriteLine($"Помилка завантаження даних: {exception.Message}");
    return;
}

var applicationIsRunning = true;

while (applicationIsRunning)
{
    PrintMenu();
    var command = Console.ReadLine();

    try
    {
        switch (command)
        {
            case "1":
                ShowProducts(store.FindProductsByBrand(null));
                break;
            case "2":
                Console.Write("Введіть бренд для пошуку: ");
                ShowProducts(store.FindProductsByBrand(Console.ReadLine()));
                break;
            case "3":
                CreateOrder(store);
                break;
            case "4":
                ShowOrders(store);
                break;
            case "5":
                store.SaveData();
                applicationIsRunning = false;
                Console.WriteLine("Дані збережено. До побачення!");
                break;
            default:
                Console.WriteLine("Невідома команда.");
                break;
        }
    }
    catch (OutOfStockException exception)
    {
        Console.WriteLine($"Помилка складу: {exception.Message}");
    }
    catch (InvalidDiscountException exception)
    {
        Console.WriteLine($"Помилка купона: {exception.Message}");
    }
    catch (ArgumentException exception)
    {
        Console.WriteLine($"Некоректні дані: {exception.Message}");
    }
    catch (StorageException exception)
    {
        Console.WriteLine($"Помилка файлу: {exception.Message}");
    }
}

static void PrintMenu()
{
    Console.WriteLine();
    Console.WriteLine("=== Інтернет-магазин електроніки ===");
    Console.WriteLine("1. Показати всі товари");
    Console.WriteLine("2. Знайти товари за брендом");
    Console.WriteLine("3. Створити замовлення");
    Console.WriteLine("4. Показати замовлення");
    Console.WriteLine("5. Зберегти дані та вийти");
    Console.Write("Оберіть пункт: ");
}

static void ShowProducts(IReadOnlyList<Product> products)
{
    Console.WriteLine();
    if (products.Count == 0)
    {
        Console.WriteLine("Товарів не знайдено.");
        return;
    }

    foreach (var product in products)
    {
        Console.WriteLine($"{product.Id} | {product.Name} | {product.Brand} | {product.Price:0.00} грн | залишок: {product.StockQuantity}");
    }
}

static void CreateOrder(StoreService store)
{
    Console.WriteLine();
    var customers = store.Customers.GetAll();
    for (var i = 0; i < customers.Count; i++)
        Console.WriteLine($"{i + 1}. {customers[i].FullName} ({customers[i].Email})");

    Console.Write("Оберіть клієнта: ");
    if (!int.TryParse(Console.ReadLine(), out var customerNumber) ||
        customerNumber < 1 || customerNumber > customers.Count)
    {
        throw new ArgumentException("Некоректний номер клієнта.");
    }

    var order = store.CreateOrder(customers[customerNumber - 1].Id);
    ShowProducts(store.FindProductsByBrand(null));
    Console.Write("Введіть назву товару: ");
    var productName = Console.ReadLine();
    var product = store.Products.GetAll().FirstOrDefault(item =>
        item.Name.Equals(productName?.Trim(), StringComparison.OrdinalIgnoreCase));

    if (product is null)
        throw new ArgumentException("Товар не знайдено.");

    Console.Write("Введіть кількість: ");
    if (!int.TryParse(Console.ReadLine(), out var quantity))
        throw new ArgumentException("Кількість повинна бути цілим числом.");

    store.AddProductToOrder(order.Id, product.Id, quantity);

    Console.Write("Введіть купон або натисніть Enter, щоб пропустити: ");
    var coupon = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(coupon))
        store.ApplyCoupon(order.Id, coupon);

    Console.WriteLine($"Замовлення створено. До сплати: {store.CalculateOrderTotal(order.Id):0.00} грн.");
}

static void ShowOrders(StoreService store)
{
    Console.WriteLine();
    foreach (var order in store.Orders.GetAll())
    {
        var customer = store.Customers.GetById(order.CustomerId);
        Console.WriteLine($"Замовлення {order.Id}: {customer?.FullName}, сума {store.CalculateOrderTotal(order.Id):0.00} грн.");
    }
}
